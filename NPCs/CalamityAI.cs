using CalamityMod.Dusts;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityAI
    {
        #region Astrum Aureus
        public static void AstrumAureusAI(NPC npc, Mod mod)
        {
            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f || CalamityWorld.bossRushActive;
            bool phase3 = lifeRatio < 0.5f || CalamityWorld.bossRushActive;

            // Variables
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            int shootBuff = (int)(2f * (1f - lifeRatio));
            float shootTimer = 1f + ((float)shootBuff);
            bool dayTime = Main.dayTime;
            Player player = Main.player[npc.target];
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

            // Despawn
            if (!player.active || player.dead || dayTime)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];

                if (!player.active || player.dead)
                {
                    npc.noTileCollide = true;
                    npc.velocity = new Vector2(0f, 10f);

                    if (npc.timeLeft > 150)
                        npc.timeLeft = 150;

                    return;
                }
            }
            else
            {
                if (npc.timeLeft < 1800)
                    npc.timeLeft = 1800;
            }

            // Emit light when not Idle
            if (npc.ai[0] != 1f)
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 2.55f, 1f, 0f);

            // Fire projectiles while walking, teleporting, or falling
            if (npc.ai[0] == 2f || npc.ai[0] >= 5f || (npc.ai[0] == 4f && npc.velocity.Y > 0f) ||
                npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += (npc.ai[0] == 2f || (npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode)) ? 4f : shootTimer;
                    if (npc.localAI[0] >= 180f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest(true);
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 33);
                        int laserDamage = expertMode ? 32 : 37;
                        if (NPC.downedMoonlord && revenge && !CalamityWorld.bossRushActive)
                            laserDamage *= 3;

                        // Fire astral flames while teleporting
                        if ((npc.ai[0] >= 5f && npc.ai[0] != 7) || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            int i;
                            float velocity = CalamityWorld.bossRushActive ? 10f : 7f;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity),
                                    (float)(Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<AstralFlame>(), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity),
                                    (float)(-Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<AstralFlame>(), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }

                        // Fire astral lasers while falling or walking
                        else if ((npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode) || npc.ai[0] == 2f)
                        {
                            float num179 = CalamityWorld.bossRushActive ? 24f : 18.5f;
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num185 = ModContent.ProjectileType<AstralLaser>();
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 5; num186++)
                            {
                                num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                num183 = num179 / num183;
                                num180 += (float)Main.rand.Next(-60, 61);
                                num182 += (float)Main.rand.Next(-60, 61);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }

            // Start up
            if (npc.ai[0] == 0f)
            {
                // If hit or after two seconds start Idle phase
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 120f)
                {
                    // Set AI to next phase (Idle) and reset other AI
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Idle
            else if (npc.ai[0] == 1f)
            {
                // Decrease defense
                npc.defense = 0;

                // Slow down
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;

                // Stay vulnerable for a maximum of 1.5 or 2.5 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((phase3 || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 90f : 150f))
                {
                    // Increase defense
                    npc.defense = 70;

                    // Stop colliding with tiles
                    npc.noGravity = true;
                    npc.noTileCollide = true;

                    // Set AI to next phase (Walk) and reset other AI
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Walk
            else if (npc.ai[0] == 2f)
            {
                // Set walking speed
                float num823 = (CalamityWorld.bossRushActive ? 8f : 5f) + (3f * (1f - lifeRatio));

                // Set walking direction
                if (Math.Abs(npc.Center.X - player.Center.X) < 200f)
                {
                    npc.velocity.X *= 0.9f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                        npc.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;

                    if (npc.direction > 0)
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    if (npc.direction < 0)
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }

                // Walk through tiles if colliding with tiles and player is out of reach
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);

                bool flag52 = false;
                if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width && npc.position.Y + (float)npc.height < player.position.Y + (float)player.height - 16f)
                    flag52 = true;

                if (flag52)
                    npc.velocity.Y += 0.5f;
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y = 0f;

                    if ((double)npc.velocity.Y > -0.2)
                        npc.velocity.Y -= 0.025f;
                    else
                        npc.velocity.Y -= 0.2f;

                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y = 0f;

                    if ((double)npc.velocity.Y < 0.1)
                        npc.velocity.Y += 0.025f;
                    else
                        npc.velocity.Y += 0.5f;
                }

                // Walk for a maximum of 6 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 360f)
                {
                    // Collide with tiles again
                    npc.noGravity = false;
                    npc.noTileCollide = false;

                    // Set AI to next phase (Jump) and reset other AI
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                // Limit downward velocity
                if (npc.velocity.Y > 10f)
                    npc.velocity.Y = 10f;
            }

            // Jump
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    // Slow down
                    npc.velocity.X *= 0.8f;

                    // Half second delay before jumping
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 30f)
                        npc.ai[1] = -20f;
                    else if (npc.ai[1] == -1f)
                    {
                        // Set jump velocity, reset and set AI to next phase (Stomp)
                        npc.TargetClosest(true);

                        float velocityX = (CalamityWorld.bossRushActive ? 9f : 6f) + (6f * (1f - lifeRatio));
                        npc.velocity.X = velocityX * (float)npc.direction;

                        if (revenge)
                        {
                            if (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                                npc.velocity.Y = -14.5f;
                            else
                                npc.velocity.Y = 1f;

                            npc.noTileCollide = true;
                        }
                        else
                            npc.velocity.Y = -14.5f;

                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            // Stomp
            else if (npc.ai[0] == 4f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Play stomp sound
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LegStomp"), (int)npc.position.X, (int)npc.position.Y);

                    // Stomp and jump again, if stomped twice then reset and set AI to next phase (Teleport or Idle)
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 3f)
                    {
                        npc.ai[0] = (phase2 || revenge) ? 5f : 1f;
                        npc.ai[2] = 0f;
                    }
                    else
                        npc.ai[0] = 3f;

                    // Spawn dust for visual effect
                    for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + (float)npc.height), npc.width + 20, 4, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }
                }
                else
                {
                    // Set velocities while falling, this happens before the stomp
                    npc.TargetClosest(true);

                    // Fall through
                    if (npc.target >= 0 && revenge && ((player.position.Y > npc.position.Y + (float)npc.height && npc.velocity.Y > 0f) || (player.position.Y < npc.position.Y + (float)npc.height && npc.velocity.Y < 0f)))
                        npc.noTileCollide = true;
                    else
                        npc.noTileCollide = false;

                    if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width)
                    {
                        npc.velocity.X *= 0.9f;

                        if (player.position.Y > npc.position.Y + (float)npc.height)
                        {
                            float fallSpeed = 0.8f + (0.8f * (1f - lifeRatio));
                            npc.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        if (npc.direction < 0)
                            npc.velocity.X -= 0.2f;
                        else if (npc.direction > 0)
                            npc.velocity.X += 0.2f;

                        float num626 = (CalamityWorld.bossRushActive ? 12f : 9f) + (6f * (1f - lifeRatio));
                        if (npc.velocity.X < -num626)
                            npc.velocity.X = -num626;
                        if (npc.velocity.X > num626)
                            npc.velocity.X = num626;
                    }
                }
            }

            // Teleport
            else if (npc.ai[0] == 5f)
            {
                // Slow down
                npc.velocity *= 0.95f;

                // Spawn slimes and start teleport
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
                    if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        npc.localAI[1] += 5f;

                    if (npc.localAI[1] >= 240f)
                    {
                        // Spawn slimes
                        bool spawnFlag = revenge;
                        if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                            spawnFlag = false;
                        if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

                        // Reset localAI and find a teleport destination
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        int num1249 = 0;
                        int num1250;
                        int num1251;

                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;
                            num1250 += Main.rand.Next(-30, 31);
                            num1251 += Main.rand.Next(-30, 31);

                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height))
                                break;

                            if (num1249 > 100)
                                goto Block;
                        }

                        // Set AI to next phase (Mid-teleport), set AI 2 and 3 to teleport coordinates X and Y respectively
                        npc.ai[0] = 6f;
                        npc.ai[2] = (float)num1250;
                        npc.ai[3] = (float)num1251;
                        npc.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }

            // Mid-teleport
            else if (npc.ai[0] == 6f)
            {
                // Become immune
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                // Turn invisible
                npc.alpha += 10;
                if (npc.alpha >= 255)
                {
                    // Set position to teleport destination
                    npc.position.X = npc.ai[2] * 16f - (float)(npc.width / 2);
                    npc.position.Y = npc.ai[3] * 16f - (float)(npc.height / 2);

                    // Reset alpha and set AI to next phase (End of teleport)
                    npc.alpha = 255;
                    npc.ai[0] = 7f;
                    npc.netUpdate = true;
                }

                // Play sound for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }

            // End of teleport
            else if (npc.ai[0] == 7f)
            {
                // Turn visible
                npc.alpha -= 10;
                if (npc.alpha <= 0)
                {
                    // Spawn slimes
                    bool spawnFlag = revenge;
                    if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                        spawnFlag = false;
                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

                    // Become vulnerable
                    npc.chaseable = true;
                    npc.dontTakeDamage = false;

                    // Reset alpha and set AI to next phase (Idle)
                    npc.alpha = 0;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }

                // Play sound at teleport destination for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }
        }
        #endregion
    }
}
