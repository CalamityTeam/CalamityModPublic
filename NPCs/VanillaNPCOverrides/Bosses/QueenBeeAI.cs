using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class QueenBeeAI
    {
        public static bool BuffedQueenBeeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            bool enrage = true;
            int num = (int)Main.player[npc.target].Center.X / 16;
            int num2 = (int)Main.player[npc.target].Center.Y / 16;

            Tile tile = Framing.GetTileSafely(num, num2);
            if (tile.WallType == WallID.HiveUnsafe)
                enrage = false;

            float enrageScale = death ? 0.25f : 0f;
            if (((npc.position.Y / 16f) < Main.worldSurface && enrage) || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 0.5f;
            }
            if (!Main.player[npc.target].ZoneJungle || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 0.5f;
            }

            if (enrageScale > 1f)
                enrageScale = 1f;

            if (Main.getGoodWorld)
                enrageScale += ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 1f : 0.5f);

            if (bossRush)
                enrageScale = 2f;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;
            bool phase5 = lifeRatio < 0.1f;

            // Despawn
            float num616 = Vector2.Distance(npc.Center, Main.player[npc.target].Center);
            if (npc.ai[0] != 5f)
            {
                if (npc.timeLeft < 60)
                    npc.timeLeft = 60;
                if (num616 > 3000f)
                    npc.ai[0] = 4f;
            }
            if (Main.player[npc.target].dead)
                npc.ai[0] = 5f;

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = npc.ai[0] == 0f;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<KamiFlu>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            // Always start in enemy spawning phase
            if (calamityGlobalNPC.newAI[3] == 0f)
            {
                calamityGlobalNPC.newAI[3] = 1f;
                npc.ai[0] = 2f;
                npc.netUpdate = true;
                npc.SyncExtraAI();
            }

            if (npc.ai[0] == 5f)
            {
                npc.velocity.Y *= 0.98f;

                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;

                if (npc.position.X < (Main.maxTilesX * 8))
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                    else
                        npc.localAI[0] = 1f;

                    npc.velocity.X -= 0.08f;
                }
                else
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                    else
                        npc.localAI[0] = 1f;

                    npc.velocity.X += 0.08f;
                }

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;
            }

            // Pick a random phase
            else if (npc.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    do phase = Main.rand.Next(4);
                    while (phase == npc.ai[1] || phase == 1);

                    npc.TargetClosest();
                    npc.ai[0] = phase;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }

            // Charging phase
            else if (npc.ai[0] == 0f)
            {
                // Number of charges
                int chargeAmt = (int)Math.Ceiling(2f + enrageScale);
                if (phase4)
                    chargeAmt++;

                // Switch to a random phase if chargeAmt has been exceeded
                if (npc.ai[1] > (2 * chargeAmt) && npc.ai[1] % 2f == 0f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                    return false;
                }

                // Charge velocity
                float speed = 16f;
                if (phase2)
                    speed += 2f;
                if (phase3)
                    speed += 2f;
                if (phase4)
                    speed += 2f;
                if (phase5)
                    speed += 2f;

                speed += 8f * enrageScale;

                // Line up and initiate charge
                if (npc.ai[1] % 2f == 0f)
                {
                    // Initiate charge
                    float num620 = 20f;
                    num620 += 20f * enrageScale;
                    if (Math.Abs(npc.position.Y + (npc.height / 2) - (Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2))) < num620)
                    {
                        // Set AI variables and speed
                        npc.localAI[0] = 1f;
                        npc.ai[1] += 1f;
                        npc.ai[2] = 0f;

                        // Get target location
                        Vector2 vector74 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                        float num599 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector74.X;
                        float num600 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector74.Y;
                        float num601 = (float)Math.Sqrt(num599 * num599 + num600 * num600);
                        num601 = speed / num601;
                        npc.velocity.X = num599 * num601;
                        npc.velocity.Y = num600 * num601;

                        // Face the correct direction and play charge sound
                        float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                        npc.direction = playerLocation < 0 ? 1 : -1;
                        npc.spriteDirection = npc.direction;

                        SoundEngine.PlaySound(SoundID.Roar, npc.position);
                        return false;
                    }

                    // Velocity variables
                    npc.localAI[0] = 0f;
                    float num602 = 12f;
                    float num603 = 0.15f;
                    if (phase2)
                    {
                        num602 += 1.5f;
                        num603 += 0.0625f;
                    }
                    if (phase3)
                    {
                        num602 += 1.5f;
                        num603 += 0.0625f;
                    }
                    if (phase4)
                    {
                        num602 += 1.5f;
                        num603 += 0.0625f;
                    }
                    if (phase5)
                    {
                        num602 += 1.5f;
                        num603 += 0.0625f;
                    }
                    num602 += 3f * enrageScale;
                    num603 += 0.5f * enrageScale;

                    // Velocity calculations
                    if (npc.position.Y + (npc.height / 2) < Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2))
                        npc.velocity.Y += num603;
                    else
                        npc.velocity.Y -= num603;

                    if (npc.velocity.Y < -num602)
                        npc.velocity.Y = -num602;
                    if (npc.velocity.Y > num602)
                        npc.velocity.Y = num602;

                    if (Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) > 500f)
                        npc.velocity.X += num603 * npc.direction;
                    else if (Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) < 300f)
                        npc.velocity.X -= num603 * npc.direction;
                    else
                        npc.velocity.X *= 0.8f;

                    // Limit velocity
                    if (npc.velocity.X < -num602)
                        npc.velocity.X = -num602;
                    if (npc.velocity.X > num602)
                        npc.velocity.X = num602;

                    // Face the correct direction
                    float playerLocation2 = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation2 < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
                else
                {
                    // Face the correct direction
                    if (npc.velocity.X < 0f)
                        npc.direction = -1;
                    else
                        npc.direction = 1;

                    npc.spriteDirection = npc.direction;

                    // Charging distance from player
                    int num604 = 450;
                    if (phase4)
                        num604 = 350;
                    else if (phase2)
                        num604 = 400;
                    num604 -= (int)(100f * enrageScale);

                    // Get which side of the player the boss is on
                    int num605 = 1;
                    if (npc.position.X + (npc.width / 2) < Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))
                        num605 = -1;

                    // If boss is in correct position, slow down, if not, reset
                    bool flag35 = false;
                    if (npc.direction == num605 && Math.Abs(npc.position.X + (npc.width / 2) - (Main.player[npc.target].position.X + (Main.player[npc.target].width / 2))) > num604)
                    {
                        npc.ai[2] = 1f;
                        flag35 = true;
                    }
                    if (Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > num604 * 1.5f)
                    {
                        npc.ai[2] = 1f;
                        flag35 = true;
                    }
                    if (enrageScale > 0f && flag35)
                        npc.velocity *= 0.5f;

                    // Keep moving
                    if (npc.ai[2] != 1f)
                    {
                        // Velocity fix if Queen Bee is slowed
                        if (npc.velocity.Length() < speed)
                            npc.velocity.X = speed * npc.direction;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > 90f)
                        {
                            npc.SyncExtraAI();
                            npc.velocity.X *= 1.01f;
                        }

                        npc.localAI[0] = 1f;
                        return false;
                    }

                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    // Slow down
                    npc.localAI[0] = 0f;
                    npc.velocity *= 0.9f;

                    float num606 = 0.1f;
                    if (phase3)
                    {
                        npc.velocity *= 0.9f;
                        num606 += 0.05f;
                    }
                    if (phase4)
                    {
                        npc.velocity *= 0.9f;
                        num606 += 0.05f;
                    }
                    if (phase5)
                    {
                        npc.velocity *= 0.9f;
                        num606 += 0.05f;
                    }
                    if (enrageScale > 0f)
                        npc.velocity *= 0.7f;

                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num606)
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        npc.SyncExtraAI();
                    }

                    npc.netUpdate = true;

                    if (npc.netSpam > 10)
                        npc.netSpam = 10;
                }
            }

            // Fly above target before bee spawning phase
            else if (npc.ai[0] == 2f)
            {
                float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                // Get target location
                float num608 = 0.1f;
                Vector2 vector75 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num609 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector75.X;
                float num610 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 200f - vector75.Y;
                float num611 = (float)Math.Sqrt(num609 * num609 + num610 * num610);

                // Go to bee spawn phase
                calamityGlobalNPC.newAI[0] += 1f;
                if (num611 < (death ? 400f : 300f) || calamityGlobalNPC.newAI[0] >= 180f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    npc.netUpdate = true;
                    npc.SyncExtraAI();
                    return false;
                }

                // Velocity calculations
                if (npc.velocity.X < num609)
                {
                    npc.velocity.X += num608;
                    if (npc.velocity.X < 0f && num609 > 0f)
                        npc.velocity.X += num608;
                }
                else if (npc.velocity.X > num609)
                {
                    npc.velocity.X -= num608;
                    if (npc.velocity.X > 0f && num609 < 0f)
                        npc.velocity.X -= num608;
                }
                if (npc.velocity.Y < num610)
                {
                    npc.velocity.Y += num608;
                    if (npc.velocity.Y < 0f && num610 > 0f)
                        npc.velocity.Y += num608;
                }
                else if (npc.velocity.Y > num610)
                {
                    npc.velocity.Y -= num608;
                    if (npc.velocity.Y > 0f && num610 < 0f)
                        npc.velocity.Y -= num608;
                }
            }

            // Bee spawn phase
            else if (npc.ai[0] == 1f)
            {
                npc.localAI[0] = 0f;

                // Get target location and spawn bees from ass
                Vector2 vector76 = new Vector2(npc.position.X + (npc.width / 2) + (Main.rand.Next(20) * npc.direction), npc.position.Y + npc.height * 0.8f);
                Vector2 vector77 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num612 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector77.X;
                float num613 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector77.Y;
                float num614 = (float)Math.Sqrt(num612 * num612 + num613 * num613);

                // Bee spawn timer
                npc.ai[1] += 1f;
                int num638 = 0;
                for (int num639 = 0; num639 < 255; num639++)
                {
                    if (Main.player[num639].active && !Main.player[num639].dead && (npc.Center - Main.player[num639].Center).Length() < 1000f)
                        num638++;
                }
                npc.ai[1] += num638 / 2;
                if (phase2)
                    npc.ai[1] += 0.25f;
                if (phase3)
                    npc.ai[1] += 0.25f;
                if (phase4)
                    npc.ai[1] += 0.25f;
                if (phase5)
                    npc.ai[1] += 0.25f;

                bool spawnBee = false;
                float num640 = 15f - 12f * enrageScale;
                if (npc.ai[1] > num640)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    spawnBee = true;
                }

                // Spawn bees or hornets
                if (Collision.CanHit(vector76, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && spawnBee)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit1, npc.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawnType = phase3 ? NPCID.Bee : Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                        if (Main.zenithWorld)
                        {
                            if (phase3)
                                spawnType = Main.rand.NextBool(3) ? ModContent.NPCType<PlagueChargerLarge>() : ModContent.NPCType<PlagueCharger>();
                            else
                                spawnType = NPCID.Hellbat;
                        }

                        int spawn = NPC.NewNPC(npc.GetSource_FromAI(), (int)vector76.X, (int)vector76.Y, spawnType);
                        Main.npc[spawn].velocity = Main.player[npc.target].Center - npc.Center;
                        Main.npc[spawn].velocity.Normalize();
                        Main.npc[spawn].velocity *= 5f;
                        if (!Main.zenithWorld)
                            Main.npc[spawn].localAI[0] = 60f;
                        Main.npc[spawn].netUpdate = true;
                    }
                }

                // Velocity calculations if target is too far away
                if (num614 > 400f || !Collision.CanHit(new Vector2(vector76.X, vector76.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    float num617 = 14f;
                    float num618 = 0.1f;
                    vector77 = vector76;
                    num612 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector77.X;
                    num613 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector77.Y;
                    num614 = (float)Math.Sqrt(num612 * num612 + num613 * num613);
                    num614 = num617 / num614;

                    if (npc.velocity.X < num612)
                    {
                        npc.velocity.X += num618;
                        if (npc.velocity.X < 0f && num612 > 0f)
                            npc.velocity.X += num618;
                    }
                    else if (npc.velocity.X > num612)
                    {
                        npc.velocity.X -= num618;
                        if (npc.velocity.X > 0f && num612 < 0f)
                            npc.velocity.X -= num618;
                    }
                    if (npc.velocity.Y < num613)
                    {
                        npc.velocity.Y += num618;
                        if (npc.velocity.Y < 0f && num613 > 0f)
                            npc.velocity.Y += num618;
                    }
                    else if (npc.velocity.Y > num613)
                    {
                        npc.velocity.Y -= num618;
                        if (npc.velocity.Y > 0f && num613 < 0f)
                            npc.velocity.Y -= num618;
                    }
                }
                else
                    npc.velocity *= 0.9f;

                // Face the correct direction
                float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                npc.direction = playerLocation < 0 ? 1 : -1;
                npc.spriteDirection = npc.direction;

                // Go to a random phase
                if (npc.ai[2] > 3f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 2f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Stinger phase
            else if (npc.ai[0] == 3f)
            {
                // Get target location and shoot from ass
                Vector2 vector78 = new Vector2(npc.position.X + (npc.width / 2) + (Main.rand.Next(20) * npc.direction), npc.position.Y + npc.height * 0.8f);
                Vector2 vector79 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num621 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector79.X;
                float num622 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - 300f - vector79.Y;
                float num623 = (float)Math.Sqrt(num621 * num621 + num622 * num622);

                npc.ai[1] += 1f;
                int num650 = phase4 ? 15 : phase3 ? 20 : phase2 ? 25 : 30;
                num650 -= (int)Math.Ceiling(5f * enrageScale);
                if (num650 < 3)
                    num650 = 3;

                // Fire stingers
                if (npc.ai[1] % num650 == (num650 - 1) && npc.position.Y + npc.height < Main.player[npc.target].position.Y && Collision.CanHit(vector78, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    SoundEngine.PlaySound(SoundID.Item17, npc.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num624 = 5f;
                        if (phase3)
                            num624 += 1f;
                        num624 += 2f * enrageScale;

                        float num625 = Main.player[npc.target].position.X + Main.player[npc.target].width * 0.5f - vector78.X;
                        float num626 = Main.player[npc.target].position.Y + Main.player[npc.target].height * 0.5f - vector78.Y;
                        float num627 = (float)Math.Sqrt(num625 * num625 + num626 * num626);
                        num627 = num624 / num627;
                        num625 *= num627;
                        num626 *= num627;
                        int type = Main.zenithWorld ? (phase3 ? ModContent.ProjectileType<PlagueStingerGoliathV2>() : ProjectileID.FlamingWood) : ProjectileID.QueenBeeStinger;
                        int projectile = Projectile.NewProjectile(npc.GetSource_FromAI(), vector78.X, vector78.Y, num625, num626, type, Main.zenithWorld ? 25 : npc.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, (Main.zenithWorld && phase3) ? Main.player[npc.target].position.Y : 0f);
                        Main.projectile[projectile].timeLeft = 600;
                        Main.projectile[projectile].extraUpdates = 1;
                    }
                }

                // Movement calculations
                float num620 = 0.075f;
                num620 += 0.2f * enrageScale;
                if (!Collision.CanHit(new Vector2(vector78.X, vector78.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    num620 = 0.1f;
                    if (enrageScale > 0f)
                        num620 = 0.5f;

                    vector79 = vector78;
                    num621 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector79.X;
                    num622 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector79.Y;

                    if (npc.velocity.X < num621)
                    {
                        npc.velocity.X += num620;
                        if (npc.velocity.X < 0f && num621 > 0f)
                            npc.velocity.X += num620;
                    }
                    else if (npc.velocity.X > num621)
                    {
                        npc.velocity.X -= num620;
                        if (npc.velocity.X > 0f && num621 < 0f)
                            npc.velocity.X -= num620;
                    }
                    if (npc.velocity.Y < num622)
                    {
                        npc.velocity.Y += num620;
                        if (npc.velocity.Y < 0f && num622 > 0f)
                            npc.velocity.Y += num620;
                    }
                    else if (npc.velocity.Y > num622)
                    {
                        npc.velocity.Y -= num620;
                        if (npc.velocity.Y > 0f && num622 < 0f)
                            npc.velocity.Y -= num620;
                    }
                }
                else if (num623 > 100f)
                {
                    float playerLocation = npc.Center.X - Main.player[npc.target].Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;
                    npc.spriteDirection = npc.direction;

                    if (npc.velocity.X < num621)
                    {
                        npc.velocity.X += num620;
                        if (npc.velocity.X < 0f && num621 > 0f)
                            npc.velocity.X += num620 * 2f;
                    }
                    else if (npc.velocity.X > num621)
                    {
                        npc.velocity.X -= num620;
                        if (npc.velocity.X > 0f && num621 < 0f)
                            npc.velocity.X -= num620 * 2f;
                    }
                    if (npc.velocity.Y < num622)
                    {
                        npc.velocity.Y += num620;
                        if (npc.velocity.Y < 0f && num622 > 0f)
                            npc.velocity.Y += num620 * 2f;
                    }
                    else if (npc.velocity.Y > num622)
                    {
                        npc.velocity.Y -= num620;
                        if (npc.velocity.Y > 0f && num622 < 0f)
                            npc.velocity.Y -= num620 * 2f;
                    }
                }

                // Go to a random phase
                if (npc.ai[1] > num650 * 15f)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 3f;
                    npc.netUpdate = true;
                }
            }

            else if (npc.ai[0] == 4f)
            {
                npc.localAI[0] = 1f;
                float num661 = 14f;
                float num662 = 14f;

                Vector2 value2 = Main.player[npc.target].Center - npc.Center;
                value2.Normalize();
                value2 *= num661;

                npc.velocity = (npc.velocity * num662 + value2) / (num662 + 1f);
                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;

                if (num616 < 2000f)
                {
                    npc.ai[0] = -1f;
                    npc.localAI[0] = 0f;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);

            npc.netSpam = 5;

            return false;
        }
    }
}
