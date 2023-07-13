using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class DestroyerAI
    {
        public static bool BuffedDestroyerAI(NPC npc, Mod mod)
        {
            int num = 0;
            int num2 = 10;
            if (NPC.IsMechQueenUp && npc.type != NPCID.TheDestroyer)
            {
                int num3 = (int)npc.ai[1];
                while (num3 > 0 && num3 < Main.maxNPCs)
                {
                    if (Main.npc[num3].active && Main.npc[num3].type >= NPCID.TheDestroyer && Main.npc[num3].type <= NPCID.TheDestroyerTail)
                    {
                        num++;
                        if (Main.npc[num3].type == NPCID.TheDestroyer)
                            break;

                        if (num >= num2)
                        {
                            num = 0;
                            break;
                        }

                        num3 = (int)Main.npc[num3].ai[1];
                        continue;
                    }

                    num = 0;
                    break;
                }
            }

            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // 10 seconds of resistance to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < 600f)
                calamityGlobalNPC.newAI[1] += 1f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = calamityGlobalNPC.newAI[1] < 600f;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.85f;
            bool phase3 = lifeRatio < 0.7f;
            bool startFlightPhase = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < (death ? 0.4f : 0.25f);
            bool phase5 = lifeRatio < (death ? 0.2f : 0.1f);

            // Flight timer
            float newAISet = phase5 ? 900f : phase4 ? 450f : 0f;
            calamityGlobalNPC.newAI[3] += 1f;
            if (calamityGlobalNPC.newAI[3] >= 1800f)
            {
                calamityGlobalNPC.newAI[3] = newAISet;
                npc.TargetClosest();
            }

            // Set worm variable for worms
            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            bool increaseSpeed = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            // Get a new target if current target is too far away
            if (increaseSpeedMore && npc.type == NPCID.TheDestroyer)
                npc.TargetClosest();

            float enrageScale = bossRush ? 1f : 0f;
            if (Main.dayTime || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Phase for flying at the player
            bool flyAtTarget = (calamityGlobalNPC.newAI[3] >= 900f && startFlightPhase) || (calamityGlobalNPC.newAI[1] < 600f && calamityGlobalNPC.newAI[1] > 60f);

            // Dust on spawn and alpha effects
            if (npc.type == NPCID.TheDestroyer || (npc.type != NPCID.TheDestroyer && Main.npc[(int)npc.ai[1]].alpha < 128))
            {
                if (npc.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int spawnDust = Dust.NewDust(npc.position, npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                        Main.dust[spawnDust].noGravity = true;
                        Main.dust[spawnDust].noLight = true;
                    }
                }
                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            // Check if other segments are still alive, if not, die
            if (npc.type > NPCID.TheDestroyer)
            {
                bool shouldDespawn = true;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.TheDestroyer)
                    {
                        shouldDespawn = false;
                        break;
                    }
                }
                if (!shouldDespawn)
                {
                    if (npc.ai[1] <= 0f)
                        shouldDespawn = true;
                    else if (Main.npc[(int)npc.ai[1]].life <= 0)
                        shouldDespawn = true;
                }
                if (shouldDespawn)
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.checkDead();
                    npc.active = false;
                }
            }

            if (npc.type == NPCID.TheDestroyerBody)
            {
                // Enrage, fire more cyan lasers
                if (enrageScale > 0f && !bossRush)
                {
                    if (calamityGlobalNPC.newAI[2] < 480f)
                        calamityGlobalNPC.newAI[2] += 1f;
                }
                else
                {
                    if (calamityGlobalNPC.newAI[2] > 0f)
                        calamityGlobalNPC.newAI[2] -= 1f;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.type == NPCID.TheDestroyer)
                {
                    // Spawn segments from head
                    if (npc.ai[0] == 0f)
                    {
                        npc.ai[3] = npc.whoAmI;
                        npc.realLife = npc.whoAmI;
                        int index = npc.whoAmI;
                        int totalSegments = Main.getGoodWorld ? 100 : 80;
                        for (int j = 0; j <= totalSegments; j++)
                        {
                            int type = NPCID.TheDestroyerBody;
                            if (j == totalSegments)
                                type = NPCID.TheDestroyerTail;

                            int segment = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), type, npc.whoAmI);
                            Main.npc[segment].ai[3] = npc.whoAmI;
                            Main.npc[segment].realLife = npc.whoAmI;
                            Main.npc[segment].ai[1] = index;
                            Main.npc[index].ai[0] = segment;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0, 0, 0);
                            index = segment;
                        }
                    }

                    // Laser breath in Death Mode
                    if (death)
                    {
                        if (calamityGlobalNPC.newAI[0] < 600f)
                            calamityGlobalNPC.newAI[0] += 1f;

                        if (npc.SafeDirectionTo(player.Center).AngleBetween((npc.rotation - MathHelper.PiOver2).ToRotationVector2()) < MathHelper.ToRadians(18f) &&
                            calamityGlobalNPC.newAI[0] >= 600f && Vector2.Distance(npc.Center, player.Center) > 480f &&
                            Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            if (calamityGlobalNPC.newAI[0] % 30f == 0f)
                            {
                                float velocity = bossRush ? 6f : death ? 5.333f : 5f;
                                int type = ProjectileID.DeathLaser;
                                int damage = npc.GetProjectileDamage(type);
                                Vector2 projectileVelocity = Vector2.Normalize(player.Center - npc.Center) * velocity;
                                int numProj = calamityGlobalNPC.newAI[0] % 60f == 0f ? 7 : 4;
                                int spread = 54;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.Normalize(perturbedSpeed) * 5f, perturbedSpeed, type, damage, 0f, Main.myPlayer, 1f, 0f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }

                            calamityGlobalNPC.newAI[0] += 1f;
                            if (calamityGlobalNPC.newAI[0] > 660f)
                                calamityGlobalNPC.newAI[0] = 0f;
                        }
                    }
                }

                // Fire lasers
                if (npc.type == NPCID.TheDestroyerBody)
                {
                    // Laser rate of fire
                    calamityGlobalNPC.newAI[0] += 1f;
                    float shootProjectile = death ? 180 : 300;
                    float timer = npc.ai[0] * 30f;
                    float shootProjectileGateValue = timer + shootProjectile;

                    // Shoot lasers
                    // 50% chance to shoot harmless scrap if probe has been launched
                    bool probeLaunched = npc.ai[2] == 1f;
                    if (calamityGlobalNPC.newAI[0] >= shootProjectileGateValue)
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.TargetClosest();
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            // Laser speed
                            float projectileSpeed = 3.5f + Main.rand.NextFloat() * 1.5f;
                            projectileSpeed += enrageScale;

                            // Set projectile damage and type
                            int projectileType = ProjectileID.DeathLaser;
                            float laserSpawnDistance = 10f;
                            int random = phase3 ? 4 : phase2 ? 3 : 2;
                            switch (Main.rand.Next(random))
                            {
                                case 0:
                                case 1:
                                    break;
                                case 2:
                                    projectileType = ModContent.ProjectileType<DestroyerCursedLaser>();
                                    break;
                                case 3:
                                    projectileType = ModContent.ProjectileType<DestroyerElectricLaser>();
                                    break;
                            }

                            if (calamityGlobalNPC.newAI[2] > 0f || bossRush)
                            {
                                projectileType = ModContent.ProjectileType<DestroyerElectricLaser>();
                                laserSpawnDistance = 20f;
                            }

                            bool scrap = false;
                            if (probeLaunched && Main.rand.NextBool())
                            {
                                scrap = true;
                                projectileType = ProjectileID.SaucerScrap;
                                laserSpawnDistance = 0f;
                            }

                            // Get target vector
                            Vector2 projectileVelocity = Vector2.Normalize(player.Center - npc.Center) * projectileSpeed;
                            Vector2 projectileSpawn = npc.Center + projectileVelocity * laserSpawnDistance;

                            // Shoot projectile and set timeLeft if not a homing laser/metal scrap so lasers don't last for too long
                            int damage = scrap ? 0 : npc.GetProjectileDamage(projectileType);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, damage, 0f, Main.myPlayer, scrap ? 0f : 1f, 0f);
                            Main.projectile[proj].timeLeft = scrap ? 150 : 900;

                            npc.netUpdate = true;
                        }
                    }
                }
            }

            int num12 = (int)(npc.position.X / 16f) - 1;
            int num13 = (int)((npc.position.X + npc.width) / 16f) + 2;
            int num14 = (int)(npc.position.Y / 16f) - 1;
            int num15 = (int)((npc.position.Y + npc.height) / 16f) + 2;

            if (num12 < 0)
                num12 = 0;
            if (num13 > Main.maxTilesX)
                num13 = Main.maxTilesX;
            if (num14 < 0)
                num14 = 0;
            if (num15 > Main.maxTilesY)
                num15 = Main.maxTilesY;

            // Fly or not
            bool flag2 = flyAtTarget;
            if (!flag2)
            {
                for (int k = num12; k < num13; k++)
                {
                    for (int l = num14; l < num15; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = k * 16;
                            vector2.Y = l * 16;
                            if (npc.position.X + npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Start flying if target is not within a certain distance
            if (!flag2)
            {
                npc.localAI[1] = 1f;

                if (npc.type == NPCID.TheDestroyer)
                {
                    Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                    int num16 = 1000;
                    int heightReduction = death ? 400 : (int)(400f * (1f - lifeRatio));
                    int height = 1800 - heightReduction;
                    bool flag3 = true;

                    if (npc.position.Y > player.position.Y)
                    {
                        for (int m = 0; m < Main.maxPlayers; m++)
                        {
                            if (Main.player[m].active)
                            {
                                Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag3 = false;
                                    break;
                                }
                            }
                        }
                        if (flag3)
                            flag2 = true;
                    }
                }
            }
            else
                npc.localAI[1] = 0f;

            // Despawn
            float fallSpeed = 16f;
            if (player.dead)
            {
                flag2 = false;
                npc.velocity.Y += 2f;

                if (npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y += 2f;
                    fallSpeed = 32f;
                }

                if (npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int n = 0; n < Main.maxNPCs; n++)
                    {
                        if (Main.npc[n].aiStyle == npc.aiStyle)
                            Main.npc[n].active = false;
                    }
                }
            }

            float fallSpeedBoost = death ? 6.5f * (1f - lifeRatio) : 5f * (1f - lifeRatio);
            fallSpeed += fallSpeedBoost;
            fallSpeed += 4f * enrageScale;

            // Speed and movement
            float speedBoost = death ? (0.14f * (1f - lifeRatio)) : (0.1f * (1f - lifeRatio));
            float turnSpeedBoost = death ? (0.19f * (1f - lifeRatio)) : (0.15f * (1f - lifeRatio));
            float speed = 0.1f + speedBoost;
            float turnSpeed = 0.15f + turnSpeedBoost;
            speed += 0.04f * enrageScale;
            turnSpeed += 0.06f * enrageScale;

            if (flyAtTarget)
            {
                float speedMultiplier = phase5 ? 1.8f : phase4 ? 1.65f : 1.5f;
                speed *= speedMultiplier;
            }

            speed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;
            turnSpeed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;

            if (Main.getGoodWorld)
            {
                speed *= 1.2f;
                turnSpeed *= 1.2f;
            }

            Vector2 vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num20 = player.position.X + (player.width / 2);
            float num21 = player.position.Y + (player.height / 2);
            num20 = (int)(num20 / 16f) * 16;
            num21 = (int)(num21 / 16f) * 16;
            vector3.X = (int)(vector3.X / 16f) * 16;
            vector3.Y = (int)(vector3.Y / 16f) * 16;
            num20 -= vector3.X;
            num21 -= vector3.Y;
            float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);

            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                int num23 = (int)(44f * npc.scale);
                try
                {
                    vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num20 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector3.X;
                    num21 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector3.Y;
                }
                catch
                {
                }

                if (num > 0)
                {
                    float num25 = (float)num23 - (float)num23 * (((float)num - 1f) * 0.1f);
                    if (num25 < 0f)
                        num25 = 0f;

                    if (num25 > (float)num23)
                        num25 = num23;

                    num22 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) + num25 - vector3.Y;
                }

                npc.rotation = (float)Math.Atan2(num21, num20) + MathHelper.PiOver2;
                num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
                if (num > 0)
                    num23 = num23 / num2 * num;

                num22 = (num22 - num23) / num22;
                num20 *= num22;
                num21 *= num22;
                npc.velocity = Vector2.Zero;
                npc.position.X += num20;
                npc.position.Y += num21;
                return false;
            }

            if (!flag2)
            {
                npc.velocity.Y += 0.15f;
                if (npc.velocity.Y > fallSpeed)
                    npc.velocity.Y = fallSpeed;

                if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.4)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X -= speed * 1.1f;
                    else
                        npc.velocity.X += speed * 1.1f;
                }
                else if (npc.velocity.Y == fallSpeed)
                {
                    if (npc.velocity.X < num20)
                        npc.velocity.X += speed;
                    else if (npc.velocity.X > num20)
                        npc.velocity.X -= speed;
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X += speed * 0.9f;
                    else
                        npc.velocity.X -= speed * 0.9f;
                }
            }
            else
            {
                if (npc.soundDelay == 0)
                {
                    float num24 = num22 / 40f;
                    if (num24 < 10f)
                        num24 = 10f;
                    if (num24 > 20f)
                        num24 = 20f;

                    npc.soundDelay = (int)num24;
                    SoundEngine.PlaySound(SoundID.WormDig, npc.position);
                }

                num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
                float num25 = Math.Abs(num20);
                float num26 = Math.Abs(num21);
                float num27 = fallSpeed / num22;
                num20 *= num27;
                num21 *= num27;

                bool flag6 = false;
                if (flyAtTarget)
                {
                    if (((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f) || (npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > speed / 2f && num22 < 400f)
                    {
                        flag6 = true;

                        if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < fallSpeed)
                            npc.velocity *= 1.1f;
                    }

                    if (npc.position.Y > player.position.Y)
                    {
                        flag6 = true;

                        if (Math.Abs(npc.velocity.X) < fallSpeed / 2f)
                        {
                            if (npc.velocity.X == 0f)
                                npc.velocity.X -= npc.direction;

                            npc.velocity.X *= 1.1f;
                        }
                        else if (npc.velocity.Y > -fallSpeed)
                            npc.velocity.Y -= speed;
                    }
                }

                if (!flag6)
                {
                    if (!flyAtTarget)
                    {
                        if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
                        {
                            if (npc.velocity.X < num20)
                                npc.velocity.X += turnSpeed;
                            else if (npc.velocity.X > num20)
                                npc.velocity.X -= turnSpeed;
                            if (npc.velocity.Y < num21)
                                npc.velocity.Y += turnSpeed;
                            else if (npc.velocity.Y > num21)
                                npc.velocity.Y -= turnSpeed;
                        }
                    }

                    if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += speed;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= speed;
                        if (npc.velocity.Y < num21)
                            npc.velocity.Y += speed;
                        else if (npc.velocity.Y > num21)
                            npc.velocity.Y -= speed;

                        if (Math.Abs(num21) < fallSpeed * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += speed * 2f;
                            else
                                npc.velocity.Y -= speed * 2f;
                        }
                        if (Math.Abs(num20) < fallSpeed * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += speed * 2f;
                            else
                                npc.velocity.X -= speed * 2f;
                        }
                    }
                    else if (num25 > num26)
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += speed * 1.1f;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= speed * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += speed;
                            else
                                npc.velocity.Y -= speed;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num21)
                            npc.velocity.Y += speed * 1.1f;
                        else if (npc.velocity.Y > num21)
                            npc.velocity.Y -= speed * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += speed;
                            else
                                npc.velocity.X -= speed;
                        }
                    }
                }
            }

            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

            if (npc.type == NPCID.TheDestroyer)
            {
                if (flag2)
                {
                    if (npc.localAI[0] != 1f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 0f;
                }

                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                    npc.netUpdate = true;
            }

            if (NPC.IsMechQueenUp && npc.type == NPCID.TheDestroyer)
            {
                NPC nPC = Main.npc[NPC.mechQueen];
                Vector2 mechQueenCenter = nPC.GetMechQueenCenter();
                Vector2 vector4 = new Vector2(0f, 100f);
                Vector2 spinningpoint = mechQueenCenter + vector4;
                float num30 = nPC.velocity.X * 0.025f;
                spinningpoint = spinningpoint.RotatedBy(num30, mechQueenCenter);
                npc.position = spinningpoint - npc.Size / 2f + nPC.velocity;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.rotation = num30 * 0.75f + (float)Math.PI;
            }

            return false;
        }

        public static bool BuffedProbeAI(NPC npc, Mod mod)
        {
            bool bossRush = BossRushEvent.BossRushActive;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            NPCAimedTarget targetData = npc.GetTargetData();
            bool targetDead = false;
            if (targetData.Type == NPCTargetType.Player)
                targetDead = Main.player[npc.target].dead;

            float velocity = bossRush ? 8f : 6f;
            float acceleration = bossRush ? 0.1f : 0.05f;

            Vector2 vector = npc.Center;
            float num4 = targetData.Center.X;
            float num5 = targetData.Center.Y;
            num4 = (int)(num4 / 8f) * 8;
            num5 = (int)(num5 / 8f) * 8;
            vector.X = (int)(vector.X / 8f) * 8;
            vector.Y = (int)(vector.Y / 8f) * 8;
            num4 -= vector.X;
            num5 -= vector.Y;
            float distanceFromTarget = (float)Math.Sqrt(num4 * num4 + num5 * num5);
            float distance2 = distanceFromTarget;

            bool farAwayFromTarget = false;
            if (distanceFromTarget > 600f)
                farAwayFromTarget = true;

            if (distanceFromTarget == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                distanceFromTarget = velocity / distanceFromTarget;
                num4 *= distanceFromTarget;
                num5 *= distanceFromTarget;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type)
                {
                    Vector2 value42 = Main.npc[i].Center - npc.Center;
                    if (value42.Length() < (npc.width + npc.height))
                    {
                        value42.Normalize();
                        value42 *= -0.1f;
                        npc.velocity += value42;
                        Main.npc[i].velocity -= value42;
                    }
                }
            }

            if (distance2 > 100f)
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                    npc.velocity.Y += 0.023f;
                else
                    npc.velocity.Y -= 0.023f;

                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    npc.velocity.X += 0.023f;
                else
                    npc.velocity.X -= 0.023f;

                if (npc.ai[0] > 200f)
                    npc.ai[0] = -200f;
            }

            if (targetDead)
            {
                num4 = npc.direction * velocity / 2f;
                num5 = -velocity / 2f;
            }

            if (npc.ai[3] != 0f)
            {
                if (NPC.IsMechQueenUp)
                {
                    NPC nPC = Main.npc[NPC.mechQueen];
                    Vector2 vector2 = new Vector2(26f * npc.ai[3], 0f);
                    int num9 = (int)npc.ai[2];
                    if (num9 < 0 || num9 >= Main.maxNPCs)
                    {
                        num9 = NPC.FindFirstNPC(NPCID.TheDestroyer);
                        npc.ai[2] = num9;
                        npc.netUpdate = true;
                    }

                    if (num9 > -1)
                    {
                        NPC nPC2 = Main.npc[num9];
                        if (!nPC2.active || nPC2.type != NPCID.TheDestroyer)
                        {
                            npc.dontTakeDamage = false;
                            if (npc.ai[3] > 0f)
                                npc.netUpdate = true;

                            npc.ai[3] = 0f;
                        }
                        else
                        {
                            Vector2 spinningpoint = nPC2.Center + vector2;
                            spinningpoint = spinningpoint.RotatedBy(nPC2.rotation, nPC2.Center);
                            npc.Center = spinningpoint;
                            npc.velocity = nPC.velocity;
                            npc.dontTakeDamage = true;
                        }
                    }
                    else
                    {
                        npc.dontTakeDamage = false;
                        if (npc.ai[3] > 0f)
                            npc.netUpdate = true;

                        npc.ai[3] = 0f;
                    }
                }
                else
                {
                    npc.dontTakeDamage = false;
                    if (npc.ai[3] > 0f)
                        npc.netUpdate = true;

                    npc.ai[3] = 0f;
                }
            }
            else
            {
                npc.dontTakeDamage = false;

                if (npc.velocity.X < num4)
                    npc.velocity.X += acceleration;
                else if (npc.velocity.X > num4)
                    npc.velocity.X -= acceleration;

                if (npc.velocity.Y < num5)
                    npc.velocity.Y += acceleration;
                else if (npc.velocity.Y > num5)
                    npc.velocity.Y -= acceleration;
            }

            npc.localAI[0] += 1f;
            if (npc.justHit)
                npc.localAI[0] = 0f;

            float laserGateValue = NPC.IsMechQueenUp ? 360f : bossRush ? 150f : 240f;
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= laserGateValue)
            {
                npc.localAI[0] = 0f;
                if (targetData.Type != 0 && Collision.CanHit(npc.position, npc.width, npc.height, targetData.Position, targetData.Width, targetData.Height))
                {
                    int type = ProjectileID.PinkLaser;
                    int damage = npc.GetProjectileDamage(type);
                    int totalProjectiles = (CalamityWorld.death || bossRush) ? 3 : 1;
                    Vector2 vector3 = new Vector2(num4, num5);
                    if (NPC.IsMechQueenUp)
                    {
                        Vector2 v = targetData.Center - npc.Center - targetData.Velocity * 20f;
                        float projectileVelocity = 8f;
                        vector3 = v.SafeNormalize(Vector2.UnitY) * projectileVelocity;
                    }
                    for (int i = 0; i < totalProjectiles; i++)
                    {
                        float velocityMultiplier = 1f;
                        switch (i)
                        {
                            case 0:
                                break;
                            case 1:
                                velocityMultiplier = 0.9f;
                                break;
                            case 2:
                                velocityMultiplier = 0.8f;
                                break;
                        }
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector, vector3 * velocityMultiplier, type, damage, 0f, Main.myPlayer);
                    }

                    npc.netUpdate = true;
                }
            }

            if (num4 > 0f)
            {
                npc.spriteDirection = 1;
                npc.rotation = (float)Math.Atan2(num5, num4);
            }
            if (num4 < 0f)
            {
                npc.spriteDirection = -1;
                npc.rotation = (float)Math.Atan2(num5, num4) + MathHelper.Pi;
            }

            float num12 = -0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    npc.velocity.X = 2f;
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    npc.velocity.X = -2f;
            }

            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * num12;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5)
                    npc.velocity.Y = 2f;
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5)
                    npc.velocity.Y = -2f;
            }

            if (farAwayFromTarget)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < (NPC.IsMechQueenUp ? 5f : 12f))
                        npc.velocity.X *= 1.05f;
                }
                else
                    npc.velocity.X *= 0.9f;
            }

            if (NPC.IsMechQueenUp && npc.ai[2] == 0f)
            {
                Vector2 center = npc.GetTargetData().Center;
                Vector2 v2 = center - npc.Center;
                int num28 = 120;
                if (v2.Length() < (float)num28)
                    npc.Center = center - v2.SafeNormalize(Vector2.UnitY) * num28;
            }

            if (targetDead)
            {
                npc.velocity.Y -= acceleration * 2f;
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }

            return false;
        }
    }
}
