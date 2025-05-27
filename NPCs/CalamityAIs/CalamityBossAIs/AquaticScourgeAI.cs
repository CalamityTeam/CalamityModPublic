using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.CalamityAIs.CalamityBossAIs
{
    public static class AquaticScourgeAI
    {
        public static void VanillaAquaticScourgeAI(NPC npc, Mod mod, bool head)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            bool getFuckedAI = Main.zenithWorld;

            if (head)
                CalamityGlobalNPC.aquaticScourge = npc.whoAmI;

            // Adjust hostility and stats
            bool nonHostile = calamityGlobalNPC.newAI[0] == 0f;
            if (npc.justHit || npc.life <= npc.lifeMax * 0.999 || bossRush || Main.getGoodWorld)
            {
                if (nonHostile)
                {
                    // Kiss my motherfucking ass you piece of shit game
                    npc.timeLeft *= 20;
                    npc.npcSlots = 16f;
                    CalamityMod.bossKillTimes.TryGetValue(npc.type, out int revKillTime);
                    calamityGlobalNPC.KillTime = revKillTime;
                    calamityGlobalNPC.newAI[0] = 1f;
                    nonHostile = false;
                    npc.boss = head;
                    npc.chaseable = true;
                    npc.netUpdate = true;
                }
            }
            else
                npc.damage = 0;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Set worm variable
            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            if (!head)
            {
                if (npc.life > Main.npc[(int)npc.ai[1]].life)
                    npc.life = Main.npc[(int)npc.ai[1]].life;
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            Player player = Main.player[npc.target];

            bool notOcean = player.position.Y < 300f ||
                player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));

            // Check for the flipped Abyss
            if (Main.remixWorld)
            {
                notOcean = player.position.Y < Main.UnderworldLayer * 0.8f || player.position.Y > Main.UnderworldLayer ||
                    (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));
            }

            // Enrage
            if (head)
            {
                if (notOcean && !player.Calamity().ZoneSulphur && !bossRush)
                {
                    if (npc.localAI[2] > 0f)
                        npc.localAI[2] -= 1f;
                }
                else
                    npc.localAI[2] = CalamityGlobalNPC.biomeEnrageTimerMax;
            }

            bool biomeEnraged = npc.localAI[2] <= 0f || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Circular movement
            float colorFadeTimeAfterSpiral = 90f;
            float spiralGateValue = 480f;
            bool doSpiral = false;
            if (head && calamityGlobalNPC.newAI[0] == 1f && calamityGlobalNPC.newAI[2] == 1f && (revenge || getFuckedAI))
            {
                doSpiral = calamityGlobalNPC.newAI[1] == 0f && calamityGlobalNPC.newAI[3] >= spiralGateValue;
                if (Vector2.Distance(npc.Center, player.Center) < (getFuckedAI ? 1600f : 1000f) || doSpiral)
                    calamityGlobalNPC.newAI[3] += 1f;

                if (doSpiral)
                {
                    npc.localAI[3] = colorFadeTimeAfterSpiral;

                    // Vomit acid mist
                    float acidMistBarfDivisor = getFuckedAI ? 2f : ((float)Math.Floor(bossRush ? 4f : death ? 5f : 6f) * (phase3 ? 1.5f : 1f));
                    if (calamityGlobalNPC.newAI[3] % acidMistBarfDivisor == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float mistVelocity = death ? 10f : 8f;
                            Vector2 projectileVelocity = (npc.Center + npc.velocity * 10f - npc.Center).SafeNormalize(Vector2.UnitY);
                            int type = ModContent.ProjectileType<SulphuricAcidMist>();
                            int damage = npc.GetProjectileDamage(type);
                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 5f, projectileVelocity * mistVelocity, type, damage, 0f, Main.myPlayer);
                            Main.projectile[proj].tileCollide = false;
                            Main.projectile[proj].timeLeft = getFuckedAI ? 240 : 600;
                        }
                    }

                    // Vomit circular spreads of acid clouds while in phase 3
                    float toxicCloudBarfDivisor = bossRush ? 20f : death ? 30f : 40f;
                    if (calamityGlobalNPC.newAI[3] % toxicCloudBarfDivisor == 0f && phase3)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<ToxicCloud>();
                            int damage = npc.GetProjectileDamage(type);
                            int totalProjectiles = (phase4 ? 6 : 9) + (getFuckedAI ? Main.rand.Next(-2, 3) : (int)((calamityGlobalNPC.newAI[3] - spiralGateValue) / toxicCloudBarfDivisor) * (phase4 ? 2 : 3));
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            float cloudVelocity = 1f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -cloudVelocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + vector255.SafeNormalize(Vector2.UnitY) * 5f, vector255, type, damage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    // Velocity boost
                    if (calamityGlobalNPC.newAI[3] == spiralGateValue)
                    {
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY);
                        npc.velocity *= 24f;
                    }

                    // Spin velocity
                    float velocity = (float)(Math.PI * 2D) / 120f;
                    // In GFB, contracts the radius as the fight progresses
                    if (getFuckedAI)
                        velocity *= phase3 ? 1.5f : phase2 ? 1.25f : 1f;
                    npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.localAI[1]);
                    // Speed up even more in GFB for more radius
                    if (getFuckedAI && npc.velocity.Length() <= 32f)
                        npc.velocity *= 1.1f;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                    // Reset and charge at target
                    // Don't reset in GFB
                    if (!getFuckedAI && calamityGlobalNPC.newAI[3] >= spiralGateValue + 120f)
                    {
                        calamityGlobalNPC.newAI[3] = 0f;
                        npc.TargetClosest();
                    }
                }
                else
                {
                    if (!Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height) && calamityGlobalNPC.newAI[3] > 300f)
                        calamityGlobalNPC.newAI[3] -= 2f;

                    if (npc.localAI[3] > 0f)
                        npc.localAI[3] -= 1f;

                    npc.localAI[1] = npc.Center.X - player.Center.X < 0 ? 1f : -1f;
                }
            }

            // Adjust slowing debuff immunity
            bool immuneToSlowingDebuffs = doSpiral || getFuckedAI;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<TimeDistortion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<GalvanicCorrosion>()] = immuneToSlowingDebuffs;
            npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
            npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

            if (head)
            {
                // Spawn segments
                if (calamityGlobalNPC.newAI[2] == 0f && npc.ai[0] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int maxLength = getFuckedAI ? 24 : death ? 80 : revenge ? 40 : expertMode ? 35 : 30;
                        int Previous = npc.whoAmI;
                        for (int segments = 0; segments < maxLength; segments++)
                        {
                            int lol;
                            if (segments >= 0 && segments < maxLength - 1)
                            {
                                if (segments % 2 == 0)
                                    lol = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeBodyAlt>(), npc.whoAmI);
                                else
                                    lol = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeBody>(), npc.whoAmI);
                            }
                            else
                                lol = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeTail>(), npc.whoAmI);

                            Main.npc[lol].realLife = npc.whoAmI;
                            Main.npc[lol].ai[2] = npc.whoAmI;
                            Main.npc[lol].ai[1] = Previous;
                            Main.npc[Previous].ai[0] = lol;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol);
                            Previous = lol;
                        }
                    }

                    calamityGlobalNPC.newAI[2] = 1f;
                }

                // Big barf attack
                if (calamityGlobalNPC.newAI[0] == 1f && (!doSpiral && phase2) || (getFuckedAI && !phase3))
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= (revenge ? 360f : 420f))
                    {
                        if (Vector2.Distance(player.Center, npc.Center) > 320f)
                        {
                            npc.localAI[0] = 0f;
                            npc.netUpdate = true;
                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int totalProjectiles = bossRush ? 10 : expertMode ? 8 : 6;
                                if (phase3)
                                    totalProjectiles *= 2;

                                int type = ModContent.ProjectileType<SandPoisonCloud>();
                                int damage = npc.GetProjectileDamage(type);
                                for (int i = 0; i < totalProjectiles; i++)
                                {
                                    Vector2 velocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                    velocity = velocity.SafeNormalize(Vector2.UnitY);
                                    velocity *= Main.rand.Next(phase3 ? 300 : 100, 401) * 0.01f;

                                    float maximumVelocityMult = death ? 0.75f : 0.5f;
                                    if (expertMode)
                                        velocity *= 1f + (maximumVelocityMult * (0.5f - lifeRatio));

                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + velocity.SafeNormalize(Vector2.UnitY) * 5f, velocity, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }

            // Fire teeth depending on body type
            else
            {
                if (calamityGlobalNPC.newAI[0] == 1f && (!phase3 || phase4))
                {
                    npc.localAI[0] += 1f;
                    float shootProjectile = 300;
                    float timer = npc.ai[0] + 15f;
                    float divisor = timer + shootProjectile;

                    if (npc.type == ModContent.NPCType<AquaticScourgeBody>())
                    {
                        if (npc.localAI[0] % divisor == 0f && (npc.ai[0] % 3f == 0f || getFuckedAI || !death))
                        {
                            npc.TargetClosest();
                            if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                            {
                                SoundEngine.PlaySound(SoundID.Item17, npc.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    float toothVelocity = death ? 9f : 8f;
                                    Vector2 projectileVelocity = (player.Center - npc.Center).SafeNormalize(Vector2.UnitY);
                                    int type = ModContent.ProjectileType<SandTooth>();
                                    int damage = npc.GetProjectileDamage(type);
                                    float accelerate = phase4 ? 1f : 0f;
                                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + projectileVelocity * 5f, projectileVelocity * toothVelocity, type, damage, 0f, Main.myPlayer, accelerate, 0f);
                                }

                                npc.netUpdate = true;
                            }
                        }
                    }
                }
            }

            // Kill body and tail
            if (!head)
            {
                bool shouldDespawn = true;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<AquaticScourgeHead>())
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
            else
            {
                if (npc.life > Main.npc[(int)npc.ai[0]].life)
                    npc.life = Main.npc[(int)npc.ai[0]].life;
            }

            float maxDistance = calamityGlobalNPC.newAI[0] == 1f ? 12800f : 6400f;
            if (player.dead || Vector2.Distance(npc.Center, player.Center) > maxDistance || (nonHostile && biomeEnraged))
            {
                calamityGlobalNPC.newAI[1] = 1f;
                npc.TargetClosest(false);
                npc.velocity.Y += 2f;

                if (npc.position.Y > Main.worldSurface * 16D)
                    npc.velocity.Y += 2f;

                if (npc.position.Y > Main.worldSurface * 16D)
                {
                    for (int a = 0; a < Main.npc.Length; a++)
                    {
                        int type = Main.npc[a].type;
                        if (type == ModContent.NPCType<AquaticScourgeHead>() || type == ModContent.NPCType<AquaticScourgeBody>() || type == ModContent.NPCType<AquaticScourgeBodyAlt>() || type == ModContent.NPCType<AquaticScourgeTail>())
                            Main.npc[a].active = false;
                    }
                }
            }
            else
                calamityGlobalNPC.newAI[1] = 0f;

            // Change direction
            if (npc.velocity.X < 0f)
                npc.spriteDirection = -1;
            else if (npc.velocity.X > 0f)
                npc.spriteDirection = 1;

            // Alpha changes
            if (head || Main.npc[(int)npc.ai[1]].alpha < 128)
            {
                npc.alpha -= 42;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            Vector2 scourgePosition = npc.Center;
            Vector2 predictionVector = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? Main.player[npc.target].velocity * 20f : Vector2.Zero;
            float scourgeTargetX = player.Center.X + predictionVector.X;
            float scourgeTargetY = player.Center.Y + predictionVector.Y;

            // Velocity and movement
            float scourgeMaxSpeed = 5f;
            float scourgeAcceleration = 0.08f;
            if (calamityGlobalNPC.newAI[0] == 1f)
            {
                scourgeMaxSpeed = revenge ? 14.4f : 12f;
                scourgeAcceleration = revenge ? 0.18f : 0.15f;
                if (expertMode)
                {
                    scourgeMaxSpeed += 2.4f * (1f - lifeRatio);
                    scourgeAcceleration += 0.03f * (1f - lifeRatio);
                }
                scourgeMaxSpeed += 3f * enrageScale;
                scourgeAcceleration += 0.06f * enrageScale;
                if (death || getFuckedAI)
                {
                    scourgeMaxSpeed += 5f;
                    scourgeAcceleration -= getFuckedAI ? 0f : 0.03f;
                    scourgeMaxSpeed += Vector2.Distance(player.Center, npc.Center) * 0.001f;
                    scourgeAcceleration += Vector2.Distance(player.Center, npc.Center) * 0.000045f;
                }

                // Increase acceleration after spiral attack
                if (npc.localAI[3] > 0f)
                {
                    float accelerationMultiplier = MathHelper.Lerp(1f, 2f, npc.localAI[3] / colorFadeTimeAfterSpiral);
                    scourgeAcceleration *= accelerationMultiplier;
                }

                if (Main.getGoodWorld)
                {
                    scourgeMaxSpeed *= 1.15f;
                    scourgeAcceleration *= 1.15f;
                }
            }

            if (head && !doSpiral)
            {
                if (calamityGlobalNPC.newAI[0] != 1f)
                {
                    scourgeTargetY += 400;
                    if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
                    {
                        if (npc.velocity.X > 0f)
                            scourgeTargetX = player.Center.X + 600f;
                        else
                            scourgeTargetX = player.Center.X - 600f;
                    }
                }

                float scourgeHigherSpeed = scourgeMaxSpeed * 1.3f;
                float scourgeLowerSpeed = scourgeMaxSpeed * 0.7f;
                float scourgeSpeed = npc.velocity.Length();
                if (scourgeSpeed > 0f)
                {
                    if (scourgeSpeed > scourgeHigherSpeed)
                    {
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY);
                        npc.velocity *= scourgeHigherSpeed;
                    }
                    else if (scourgeSpeed < scourgeLowerSpeed)
                    {
                        npc.velocity = npc.velocity.SafeNormalize(Vector2.UnitY);
                        npc.velocity *= scourgeLowerSpeed;
                    }
                }
            }

            scourgeTargetX = (int)(scourgeTargetX / 16f) * 16;
            scourgeTargetY = (int)(scourgeTargetY / 16f) * 16;
            scourgePosition.X = (int)(scourgePosition.X / 16f) * 16;
            scourgePosition.Y = (int)(scourgePosition.Y / 16f) * 16;
            scourgeTargetX -= scourgePosition.X;
            scourgeTargetY -= scourgePosition.Y;
            float scourgeTargetDist = (float)Math.Sqrt(scourgeTargetX * scourgeTargetX + scourgeTargetY * scourgeTargetY);

            if (!head)
            {
                if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
                {
                    try
                    {
                        scourgePosition = npc.Center;
                        scourgeTargetX = Main.npc[(int)npc.ai[1]].Center.X - scourgePosition.X;
                        scourgeTargetY = Main.npc[(int)npc.ai[1]].Center.Y - scourgePosition.Y;
                    }
                    catch
                    {
                    }

                    npc.rotation = (float)Math.Atan2(scourgeTargetY, scourgeTargetX) + MathHelper.PiOver2;
                    scourgeTargetDist = (float)Math.Sqrt(scourgeTargetX * scourgeTargetX + scourgeTargetY * scourgeTargetY);
                    int scourgeWidth = npc.width;
                    scourgeTargetDist = (scourgeTargetDist - scourgeWidth) / scourgeTargetDist;
                    scourgeTargetX *= scourgeTargetDist;
                    scourgeTargetY *= scourgeTargetDist;
                    npc.velocity = Vector2.Zero;
                    npc.position.X = npc.position.X + scourgeTargetX;
                    npc.position.Y = npc.position.Y + scourgeTargetY;

                    if (scourgeTargetX < 0f)
                        npc.spriteDirection = -1;
                    else if (scourgeTargetX > 0f)
                        npc.spriteDirection = 1;
                }
            }
            else if (!doSpiral)
            {
                float scourgeAbsoluteTargetX = Math.Abs(scourgeTargetX);
                float scourgeAbsoluteTargetY = Math.Abs(scourgeTargetY);
                float scourgeTimeToReachTarget = scourgeMaxSpeed / scourgeTargetDist;
                scourgeTargetX *= scourgeTimeToReachTarget;
                scourgeTargetY *= scourgeTimeToReachTarget;

                if ((npc.velocity.X > 0f && scourgeTargetX > 0f) || (npc.velocity.X < 0f && scourgeTargetX < 0f) || (npc.velocity.Y > 0f && scourgeTargetY > 0f) || (npc.velocity.Y < 0f && scourgeTargetY < 0f))
                {
                    if (npc.velocity.X < scourgeTargetX)
                    {
                        npc.velocity.X += scourgeAcceleration;
                    }
                    else
                    {
                        if (npc.velocity.X > scourgeTargetX)
                            npc.velocity.X -= scourgeAcceleration;
                    }

                    if (npc.velocity.Y < scourgeTargetY)
                    {
                        npc.velocity.Y += scourgeAcceleration;
                    }
                    else
                    {
                        if (npc.velocity.Y > scourgeTargetY)
                            npc.velocity.Y -= scourgeAcceleration;
                    }

                    if (Math.Abs(scourgeTargetY) < scourgeMaxSpeed * 0.2 && ((npc.velocity.X > 0f && scourgeTargetX < 0f) || (npc.velocity.X < 0f && scourgeTargetX > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y += scourgeAcceleration * 2f;
                        else
                            npc.velocity.Y -= scourgeAcceleration * 2f;
                    }

                    if (Math.Abs(scourgeTargetX) < scourgeMaxSpeed * 0.2 && ((npc.velocity.Y > 0f && scourgeTargetY < 0f) || (npc.velocity.Y < 0f && scourgeTargetY > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X += scourgeAcceleration * 2f;
                        else
                            npc.velocity.X -= scourgeAcceleration * 2f;
                    }
                }
                else
                {
                    if (scourgeAbsoluteTargetX > scourgeAbsoluteTargetY)
                    {
                        if (npc.velocity.X < scourgeTargetX)
                            npc.velocity.X += scourgeAcceleration * 1.1f;
                        else if (npc.velocity.X > scourgeTargetX)
                            npc.velocity.X -= scourgeAcceleration * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < scourgeMaxSpeed * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += scourgeAcceleration;
                            else
                                npc.velocity.Y -= scourgeAcceleration;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < scourgeTargetY)
                            npc.velocity.Y += scourgeAcceleration * 1.1f;
                        else if (npc.velocity.Y > scourgeTargetY)
                            npc.velocity.Y -= scourgeAcceleration * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < scourgeMaxSpeed * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += scourgeAcceleration;
                            else
                                npc.velocity.X -= scourgeAcceleration;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
            }

            // Calculate contact damage based on velocity
            if (!nonHostile)
            {
                float minimalContactDamageVelocity = scourgeMaxSpeed * 0.25f;
                float minimalDamageVelocity = scourgeMaxSpeed * 0.5f;
                if (head)
                {
                    if (npc.velocity.Length() <= minimalContactDamageVelocity)
                    {
                        npc.damage = (int)Math.Round(npc.defDamage * 0.5);
                    }
                    else
                    {
                        float velocityDamageScalar = MathHelper.Clamp((npc.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                        npc.damage = (int)MathHelper.Lerp((float)Math.Round(npc.defDamage * 0.5), npc.defDamage, velocityDamageScalar);
                    }
                }
                else
                {
                    float bodyAndTailVelocity = (npc.position - npc.oldPosition).Length();
                    if (bodyAndTailVelocity <= minimalContactDamageVelocity)
                    {
                        npc.damage = 0;
                    }
                    else
                    {
                        float velocityDamageScalar = MathHelper.Clamp((bodyAndTailVelocity - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                        npc.damage = (int)MathHelper.Lerp(0f, npc.defDamage, velocityDamageScalar);
                    }
                }
            }
        }
    }
}
