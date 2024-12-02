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
    public static class OldDukeAI
    {
        public static void VanillaOldDukeAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();
            OldDuke.OldDuke modNPC = npc.ModNPC<OldDuke.OldDuke>();

            npc.Calamity().canBreakPlayerDefense = true;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Variables
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            float exhaustionGateValue = 300f;
            if (Main.getGoodWorld)
                exhaustionGateValue *= 0.5f;

            float exhaustionIncreasePerAttack = exhaustionGateValue * 0.1f;
            bool exhausted = calamityGlobalNPC.newAI[1] == 1f;
            bool phase2 = lifeRatio <= (death ? 0.8f : revenge ? 0.7f : 0.5f);
            bool phase3 = lifeRatio <= (death ? 0.5f : (revenge ? 0.35f : 0.2f)) && expertMode;
            bool phase2AI = npc.ai[0] > 4f;
            bool phase3AI = npc.ai[0] > 9f;
            bool charging = npc.ai[3] < 10f;
            float pie = (float)Math.PI;

            if (calamityGlobalNPC.newAI[0] >= exhaustionGateValue)
                calamityGlobalNPC.newAI[1] = 1f;

            float alphaScale = (255 - npc.alpha) / 255f;
            float redLight = (phase3AI ? 0.4f : phase2AI ? 0.64f : 0.88f) * alphaScale;
            float greenLight = (phase3AI ? 1.2f : phase2AI ? 0.8f : 0.4f) * alphaScale;
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), redLight, greenLight, 0f);

            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();
            else if (!Main.raining)
                CalamityUtils.StartRain();

            // Adjust stats
            int setDamage = npc.defDamage;
            calamityGlobalNPC.DR = exhausted ? 0f : 0.5f;
            npc.defense = exhausted ? 0 : npc.defDefense;
            if (phase3AI)
            {
                setDamage = (int)Math.Round(setDamage * 1.2);
                npc.defense = exhausted ? 0 : npc.defDefense - 40;
            }
            else if (phase2AI)
            {
                setDamage = (int)Math.Round(setDamage * 1.1);
                npc.defense = exhausted ? 0 : npc.defDefense - 20;
            }

            int idlePhaseTimer = expertMode ? 55 : 60;
            float idlePhaseAcceleration = expertMode ? 0.75f : 0.7f;
            float idlePhaseVelocity = expertMode ? 14f : 13f;
            if (phase3AI)
            {
                idlePhaseAcceleration = expertMode ? 0.6f : 0.55f;
                idlePhaseVelocity = expertMode ? 12f : 11f;
            }
            else if (phase2AI & charging)
            {
                idlePhaseAcceleration = expertMode ? 0.8f : 0.75f;
                idlePhaseVelocity = expertMode ? 15f : 14f;
            }

            int chargeTime = expertMode ? 34 : 36;
            float chargeVelocity = expertMode ? 20f : 19f;
            if (phase3AI)
            {
                chargeTime = expertMode ? 28 : 30;
                chargeVelocity = expertMode ? 26f : 25f;
            }
            else if (charging & phase2AI)
            {
                chargeTime = expertMode ? 31 : 33;
                chargeVelocity = expertMode ? 24f : 23f;
            }

            if (bossRush)
            {
                idlePhaseTimer = 35;
                idlePhaseAcceleration *= 1.25f;
                idlePhaseVelocity *= 1.2f;
                chargeTime -= 3;
                chargeVelocity *= 1.25f;
            }
            else if (death)
            {
                idlePhaseTimer = 51;
                idlePhaseAcceleration *= 1.05f;
                idlePhaseVelocity *= 1.05f;
                chargeTime -= 2;
                chargeVelocity *= 1.1f;
            }
            else if (revenge)
            {
                idlePhaseTimer = 53;
                idlePhaseAcceleration *= 1.025f;
                idlePhaseVelocity *= 1.025f;
                chargeTime -= 1;
                chargeVelocity *= 1.05f;
            }

            // The dumbest thing to ever exist
            if (CalamityWorld.LegendaryMode && revenge)
                chargeVelocity *= 1.25f;

            if (exhausted)
                idlePhaseVelocity *= 0.25f;

            // Variables
            int maxToothBallBelches = bossRush ? 5 : death ? 4 : 3;
            int toothBallBelchPhaseDivisor = bossRush ? 24 : death ? 30 : 40;
            int toothBallBelchPhaseTimer = toothBallBelchPhaseDivisor * maxToothBallBelches;
            float toothBallBelchPhaseAcceleration = bossRush ? 0.95f : death ? 0.6f : 0.55f;
            float toothBallBelchPhaseVelocity = bossRush ? 14f : death ? 10f : 9f;
            float toothBallFinalVelocity = death ? 14f : revenge ? 13f : 12f;
            float goreVelocityX = death ? 8f : revenge ? 7.5f : expertMode ? 7f : 6f;
            float goreVelocityY = death ? 10.5f : revenge ? 10f : expertMode ? 9.5f : 8f;
            float sharkronVelocity = bossRush ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
            int attackTimer = 120;
            int phaseTransitionTimer = 180;
            int teleportPauseTimer = 30;
            int toothBallSpinPhaseDivisor = bossRush ? 27 : death ? 32 : 45;
            int toothBallSpinTimer = maxToothBallBelches * toothBallSpinPhaseDivisor;
            float spinTime = toothBallSpinTimer / 2f;
            float toothBallSpinToothBallVelocity = bossRush ? 14f : death ? 9.5f : 9f;
            float spinAttackSpeed = Main.zenithWorld ? 44f : 22f;
            float spinSpeed = MathHelper.TwoPi / spinTime;

            Player player = Main.player[npc.target];

            // Get target
            if (npc.target < 0 || npc.target == Main.maxPlayers || player.dead || !player.active)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                npc.netUpdate = true;
            }

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(player.Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Despawn
            if (player.dead || Vector2.Distance(player.Center, npc.Center) > 8800f)
            {
                npc.TargetClosest();

                npc.velocity.Y -= 0.4f;

                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;

                if (npc.timeLeft == 1)
                {
                    AcidRainEvent.AccumulatedKillPoints = 0;
                    AcidRainEvent.HasTriedToSummonOldDuke = false;
                    AcidRainEvent.UpdateInvasion(false);
                    npc.timeLeft = 0;
                }

                if (npc.ai[0] > 4f)
                    npc.ai[0] = 5f;
                else
                    npc.ai[0] = 0f;

                npc.ai[2] = 0f;
            }

            if (exhausted)
            {
                npc.Calamity().canBreakPlayerDefense = false;

                // Play exhausted sound
                if (calamityGlobalNPC.newAI[0] % 60f == 0f && Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < 2800f)
                    SoundEngine.PlaySound(OldDuke.OldDuke.HuffSound with { Volume = OldDuke.OldDuke.HuffSound.Volume * 1.25f }, Main.LocalPlayer.Center);

                if (Main.zenithWorld)
                {
                    float screenShakePower = 10 * Utils.GetLerpValue(800f, 0f, npc.Distance(Main.LocalPlayer.Center), true);
                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < screenShakePower)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = screenShakePower;

                    if (calamityGlobalNPC.newAI[0] == exhaustionGateValue)
                        SoundEngine.PlaySound(SoundID.NPCDeath64 with { Pitch = SoundID.NPCDeath64.Pitch - 0.9f, Volume = SoundID.NPCDeath64.Volume + 0.4f }, player.Center); // fart

                    if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[0] % 5f == 0f)
                    {
                        Vector2 dist = player.Center - npc.Center;
                        dist.Normalize();
                        dist *= 3;
                        dist.X += Main.rand.NextFloat(-0.5f, 0.5f);
                        dist.Y += Main.rand.NextFloat(-0.5f, 0.5f);
                        int type = ModContent.ProjectileType<SandPoisonCloudOldDuke>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, -dist, type, damage, 0, Main.myPlayer);
                    }
                }

                calamityGlobalNPC.newAI[0] -= bossRush ? 1.5f : 1f;
                if (calamityGlobalNPC.newAI[0] <= 0f)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                }
            }

            // Enrage variable
            bool enrage = !bossRush &&
                (player.position.Y < 300f || player.position.Y > Main.worldSurface * 16 ||
                (player.position.X > 8000f && player.position.X < (Main.maxTilesX * 16 - 8000)));

            // Check for the flipped Abyss
            if (Main.remixWorld)
            {
                enrage = !bossRush &&
                    (player.position.Y < Main.UnderworldLayer * 16 * 0.8f || player.position.Y > Main.UnderworldLayer * 16 ||
                    (player.position.X > 8000f && player.position.X < (Main.maxTilesX * 16 - 8000)));
            }

            // Enrage
            if (enrage)
            {
                if (npc.localAI[1] > 0f)
                    npc.localAI[1] -= 1f;
            }
            else
                npc.localAI[1] = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = npc.localAI[1] <= 0f;

            npc.Calamity().CurrentlyEnraged = biomeEnraged || bossRush;

            // Increased DR while transitioning phases and not exhausted
            if (!exhausted)
                calamityGlobalNPC.DR = (npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f) ? (bossRush ? 0.99f : 0.75f) : 0.5f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = npc.ai[0] == -1f || npc.ai[0] == 4f || npc.ai[0] == 9f;

            // Enrage
            if (biomeEnraged)
            {
                toothBallBelchPhaseTimer = 60;
                toothBallBelchPhaseDivisor = 20;
                toothBallBelchPhaseAcceleration = 1f;
                toothBallBelchPhaseVelocity = 15f;
                goreVelocityX = 12f;
                goreVelocityY = 16f;
                sharkronVelocity = 20f;
                idlePhaseTimer = 20;
                idlePhaseAcceleration = 1.2f;
                idlePhaseVelocity = 20f;
                chargeTime = 25;
                chargeVelocity += 8f;
                toothBallSpinPhaseDivisor = 24;
                toothBallSpinToothBallVelocity = 15f;
                setDamage *= 2;
                npc.defense = npc.defDefense * 3;
            }

            // The dumbest thing to ever exist
            if (CalamityWorld.LegendaryMode && revenge)
                chargeTime *= 2;

            // Set variables for spawn effects
            if (npc.localAI[0] == 0f)
            {
                npc.localAI[0] = 1f;
                npc.alpha = 255;
                npc.rotation = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[0] = -1f;
                    npc.netUpdate = true;
                }
            }

            // Rotation
            float rateOfRotation = 0.04f;
            if (npc.ai[0] == 1f || npc.ai[0] == 6f || npc.ai[0] == 7f || npc.ai[0] == 14f)
                rateOfRotation = 0f;
            if (npc.ai[0] == 3f || npc.ai[0] == 4f)
                rateOfRotation = 0.01f;
            if (npc.ai[0] == 8f || npc.ai[0] == 13f)
                rateOfRotation = 0.05f;

            Vector2 rotationVector = player.Center - npc.Center;
            if (calamityGlobalNPC.newAI[1] != 1f && !player.dead)
            {
                // Rotate to show direction of predictive charge
                if (npc.ai[0] == 0f && !phase2)
                {
                    if (npc.ai[3] < 6f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - npc.Center) * chargeVelocity;
                    }
                }
                else if (npc.ai[0] == 5f && !phase3)
                {
                    if (npc.ai[3] < 4f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - npc.Center) * chargeVelocity;
                    }
                }
                else if (npc.ai[0] == 10f)
                {
                    if (npc.ai[3] < 8f && npc.ai[3] != 1f && npc.ai[3] != 4f)
                    {
                        rateOfRotation = 0.1f;
                        rotationVector = Vector2.Normalize(player.Center + player.velocity * 20f - npc.Center) * chargeVelocity;
                    }
                }
            }

            float dukeRotationSpeed = (float)Math.Atan2(rotationVector.Y, rotationVector.X);
            if (npc.spriteDirection == 1)
                dukeRotationSpeed += pie;
            if (dukeRotationSpeed < 0f)
                dukeRotationSpeed += MathHelper.TwoPi;
            if (dukeRotationSpeed > MathHelper.TwoPi)
                dukeRotationSpeed -= MathHelper.TwoPi;
            if (npc.ai[0] == -1f || npc.ai[0] == 3f || npc.ai[0] == 4f)
                dukeRotationSpeed = 0f;
            if (npc.ai[0] == 8f || npc.ai[0] == 13f)
                dukeRotationSpeed = pie * 0.1666666667f * npc.spriteDirection;

            if (npc.rotation < dukeRotationSpeed)
            {
                if (dukeRotationSpeed - npc.rotation > pie)
                    npc.rotation -= rateOfRotation;
                else
                    npc.rotation += rateOfRotation;
            }
            if (npc.rotation > dukeRotationSpeed)
            {
                if (npc.rotation - dukeRotationSpeed > pie)
                    npc.rotation += rateOfRotation;
                else
                    npc.rotation -= rateOfRotation;
            }

            if ((npc.ai[0] != 8f && npc.ai[0] != 13f) || npc.spriteDirection == 1)
            {
                if (npc.rotation > dukeRotationSpeed - rateOfRotation && npc.rotation < dukeRotationSpeed + rateOfRotation)
                    npc.rotation = dukeRotationSpeed;
                if (npc.rotation < 0f)
                    npc.rotation += MathHelper.TwoPi;
                if (npc.rotation > MathHelper.TwoPi)
                    npc.rotation -= MathHelper.TwoPi;
                if (npc.rotation > dukeRotationSpeed - rateOfRotation && npc.rotation < dukeRotationSpeed + rateOfRotation)
                    npc.rotation = dukeRotationSpeed;
            }

            // Alpha adjustments
            if (npc.ai[0] != -1f && (npc.ai[0] < 9f || npc.ai[0] > 12f))
            {
                if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.alpha += 15;
                else
                    npc.alpha -= 15;

                if (npc.alpha < 0)
                    npc.alpha = 0;
                if (npc.alpha > 150)
                    npc.alpha = 150;
            }

            // Spawn effects
            if (npc.ai[0] == -1f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                if (npc.Calamity().newAI[3] == 0f)
                    npc.velocity *= 0.98f;

                // Direction
                int dukeFaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (dukeFaceDirection != 0)
                {
                    npc.direction = dukeFaceDirection;
                    npc.spriteDirection = -npc.direction;
                }

                // Alpha
                if (npc.ai[2] > 20f)
                {
                    if (npc.Calamity().newAI[3] == 0f)
                        npc.velocity.Y = -2f;

                    npc.alpha -= 5;
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                        npc.alpha += 15;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                    if (npc.alpha > 150)
                        npc.alpha = 150;
                }

                // Spawn dust and play sound
                if (npc.ai[2] == attackTimer - 30)
                {
                    int dustAmt = 36;
                    for (int i = 0; i < dustAmt; i++)
                    {
                        Vector2 dust = (Vector2.Normalize(npc.velocity) * new Vector2(npc.width / 2f, npc.height) * 0.75f * 0.5f).RotatedBy((i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt) + npc.Center;
                        Vector2 vector2 = dust - npc.Center;
                        int toxicDust = Dust.NewDust(dust + vector2, 0, 0, (int)CalamityDusts.SulphurousSeaAcid, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
                        Main.dust[toxicDust].noGravity = true;
                        Main.dust[toxicDust].noLight = true;
                        Main.dust[toxicDust].velocity = Vector2.Normalize(vector2) * 3f;
                    }

                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= 75)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Phase 1
            else if (npc.ai[0] == 0f && !player.dead)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 500 * Math.Sign((npc.Center - player.Center).X);

                Vector2 phase1IdleSpeed = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(phase1IdleSpeed, idlePhaseAcceleration);

                // Rotation and direction
                int dukeLookAt = Math.Sign(player.Center.X - npc.Center.X);
                if (dukeLookAt != 0)
                {
                    if (npc.ai[2] == 0f && dukeLookAt != npc.direction)
                        npc.rotation += pie;

                    npc.direction = dukeLookAt;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f || (phase2 && !phase2AI))
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= idlePhaseTimer || (phase2 && !phase2AI))
                    {
                        int oldDukeAttackPicker = 0;
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                oldDukeAttackPicker = 1;
                                break;
                            case 6:
                                npc.ai[3] = 1f;
                                oldDukeAttackPicker = 2;
                                break;
                            case 7:
                                npc.ai[3] = 0f;
                                oldDukeAttackPicker = 3;
                                break;
                        }

                        if (phase2)
                            oldDukeAttackPicker = 4;

                        // Set velocity for charge
                        if (oldDukeAttackPicker == 1)
                        {
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;

                            // Velocity
                            Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                            npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            // Direction
                            if (dukeLookAt != 0)
                            {
                                npc.direction = dukeLookAt;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }

                        // Tooth Balls
                        else if (oldDukeAttackPicker == 2)
                        {
                            npc.ai[0] = 2f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        // Call sharks from the sides of the screen
                        else if (oldDukeAttackPicker == 3)
                        {
                            npc.ai[0] = 3f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        // Go to phase 2
                        else if (oldDukeAttackPicker == 4)
                        {
                            npc.ai[0] = 4f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        npc.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 1f)
            {
                // Set damage
                npc.damage = setDamage;

                // The dumbest thing to ever exist
                if (CalamityWorld.LegendaryMode && revenge && npc.ai[2] % 10f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - npc.Center.X);
                    if (dir != 0)
                    {
                        if (npc.ai[2] == 0f && dir != npc.direction)
                            npc.rotation += pie;

                        npc.direction = dir;

                        if (npc.spriteDirection != -npc.direction)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                    npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        npc.direction = dir;

                        if (npc.spriteDirection == 1)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }
                }
                else
                {
                    // Accelerate
                    npc.velocity *= 1.01f;
                }

                // Spawn dust
                int chargeDustAmt = 7;
                for (int j = 0; j < chargeDustAmt; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (chargeDustAmt / 2 - 1)) * pie / chargeDustAmt) + npc.Center;
                    Vector2 vector4 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int chargeDust = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, (int)CalamityDusts.SulphurousSeaAcid, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                    Main.dust[chargeDust].noGravity = true;
                    Main.dust[chargeDust].noLight = true;
                    Main.dust[chargeDust].velocity /= 4f;
                    Main.dust[chargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Tooth Ball belch
            else if (npc.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 500 * Math.Sign((npc.Center - player.Center).X);

                Vector2 toothBallBelchPhaseSpeed = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - npc.Center - npc.velocity) * toothBallBelchPhaseVelocity;
                npc.SimpleFlyMovement(toothBallBelchPhaseSpeed, toothBallBelchPhaseAcceleration);

                // Play sounds and spawn Tooth Balls
                if (npc.ai[2] == 0f)
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                if (npc.ai[2] % toothBallBelchPhaseDivisor == 0f)
                {
                    if (npc.ai[2] != 0f)
                        SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                    Vector2 toothBallDirection = Vector2.Normalize(player.Center - npc.Center) * (npc.width + 20) / 2f + npc.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * toothBallFinalVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(toothBallDirection.X, toothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(npc.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * npc.velocity.Length();
                        Main.npc[toothBall].netUpdate = true;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Direction
                int toothBallLookAt = Math.Sign(player.Center.X - npc.Center.X);
                if (toothBallLookAt != 0)
                {
                    npc.direction = toothBallLookAt;
                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;
                    npc.spriteDirection = -npc.direction;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= toothBallBelchPhaseTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Call sharks from the sides of the screen
            else if (npc.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound and spawn sharks
                if (npc.ai[2] == attackTimer - 30)
                    SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                if (npc.ai[2] >= attackTimer - 90)
                {
                    if (npc.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 100f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 900f), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 900f), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 1800f), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 1800f), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                            }
                        }
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Transition to phase 2 and call sharks from below
            else if (npc.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Sound
                if (npc.ai[2] == phaseTransitionTimer - 60)
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                if (npc.ai[2] >= phaseTransitionTimer - 60)
                {
                    if (npc.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 150f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 50f + calamityGlobalNPC.newAI[2]), (int)(npc.Center.Y + 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 50f - calamityGlobalNPC.newAI[2]), (int)(npc.Center.Y + 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 50f + calamityGlobalNPC.newAI[2] * 0.5f), (int)(npc.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 50f - calamityGlobalNPC.newAI[2] * 0.5f), (int)(npc.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);
                            }
                        }
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseTransitionTimer)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Phase 2
            else if (npc.ai[0] == 5f && !player.dead)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 500 * Math.Sign((npc.Center - player.Center).X);

                Vector2 dukePhase2IdleSpeed = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -300f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(dukePhase2IdleSpeed, idlePhaseAcceleration);

                // Direction and rotation
                int phase2FaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (phase2FaceDirection != 0)
                {
                    if (npc.ai[2] == 0f && phase2FaceDirection != npc.direction)
                        npc.rotation += pie;

                    npc.direction = phase2FaceDirection;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f || (phase3 && !phase3AI))
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= idlePhaseTimer || (phase3 && !phase3AI))
                    {
                        int phase2AttackPicker = 0;
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                phase2AttackPicker = 1;
                                break;
                            case 4:
                                npc.ai[3] = 1f;
                                phase2AttackPicker = 2;
                                break;
                            case 5:
                                npc.ai[3] = 0f;
                                phase2AttackPicker = 3;
                                break;
                        }

                        if (phase3)
                            phase2AttackPicker = 4;

                        // Set velocity for charge
                        if (phase2AttackPicker == 1)
                        {
                            npc.ai[0] = 6f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;

                            // Velocity and rotation
                            Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                            npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            // Direction
                            if (phase2FaceDirection != 0)
                            {
                                npc.direction = phase2FaceDirection;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }

                        // Set velocity for spin
                        else if (phase2AttackPicker == 2)
                        {
                            // Velocity and rotation
                            npc.velocity = Vector2.Normalize(player.Center - npc.Center) * spinAttackSpeed;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            // Direction
                            if (phase2FaceDirection != 0)
                            {
                                npc.direction = phase2FaceDirection;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }

                            npc.ai[0] = 7f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        else if (phase2AttackPicker == 3)
                        {
                            npc.ai[0] = 8f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        // Go to next phase
                        else if (phase2AttackPicker == 4)
                        {
                            npc.ai[0] = 9f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        npc.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 6f)
            {
                // Set damage
                npc.damage = setDamage;

                // The dumbest thing to ever exist
                if (CalamityWorld.LegendaryMode && revenge && npc.ai[2] % 8f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - npc.Center.X);
                    if (dir != 0)
                    {
                        if (npc.ai[2] == 0f && dir != npc.direction)
                            npc.rotation += pie;

                        npc.direction = dir;

                        if (npc.spriteDirection != -npc.direction)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                    npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        npc.direction = dir;

                        if (npc.spriteDirection == 1)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }
                }
                else
                {
                    // Accelerate
                    npc.velocity *= 1.01f;
                }

                // Spawn dust
                int phase2ChargeDustAmt = 7;
                for (int k = 0; k < phase2ChargeDustAmt; k++)
                {
                    Vector2 arg_1A97_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((k - (phase2ChargeDustAmt / 2 - 1)) * pie / phase2ChargeDustAmt) + npc.Center;
                    Vector2 vector9 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase2ChargeDust = Dust.NewDust(arg_1A97_0 + vector9, 0, 0, (int)CalamityDusts.SulphurousSeaAcid, vector9.X * 2f, vector9.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase2ChargeDust].noGravity = true;
                    Main.dust[phase2ChargeDust].noLight = true;
                    Main.dust[phase2ChargeDust].velocity /= 4f;
                    Main.dust[phase2ChargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Tooth Ball and Vortex spin
            else if (npc.ai[0] == 7f)
            {
                // Set damage
                npc.damage = 0;

                // Play sounds and spawn Tooth Balls and a Vortex
                if (npc.ai[2] == 0f)
                {
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                    int type = ModContent.ProjectileType<OldDukeVortex>();
                    int damage = npc.GetProjectileDamage(type);
                    Vector2 vortexSpawn = npc.Center + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vortexSpawn, Vector2.Zero, type, damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
                }

                if (npc.ai[2] % toothBallSpinPhaseDivisor == 0f)
                {
                    if (npc.ai[2] != 0f)
                        SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                    Vector2 phase2ToothBallDirection = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + npc.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * toothBallSpinToothBallVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(phase2ToothBallDirection.X, phase2ToothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(npc.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].target = npc.target;
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * toothBallSpinToothBallVelocity * 0.5f;
                        Main.npc[toothBall].netUpdate = true;
                        Main.npc[toothBall].ai[3] = 30f;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Velocity and rotation
                npc.velocity = npc.velocity.RotatedBy(-(double)spinSpeed * (float)npc.direction);
                npc.rotation -= spinSpeed * npc.direction;

                npc.ai[2] += 1f;
                if (npc.ai[2] >= toothBallSpinTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
            else if (npc.ai[0] == 8f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound
                if (npc.ai[2] == attackTimer - 30)
                {
                    SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase2GoreDirection = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + npc.Center;
                        int type = ModContent.ProjectileType<OldDukeGore>();
                        int damage = npc.GetProjectileDamage(type);
                        int totalGore = Main.getGoodWorld ? 40 : 20;
                        for (int i = 0; i < totalGore; i++)
                        {
                            float velocityX = npc.direction * goreVelocityX * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);
                            float velocityY = goreVelocityY * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);

                            if (Main.getGoodWorld)
                            {
                                velocityX *= Main.rand.NextFloat() + 0.5f;
                                velocityY *= Main.rand.NextFloat() + 0.5f;
                            }

                            Projectile.NewProjectile(npc.GetSource_FromAI(), phase2GoreDirection.X, phase2GoreDirection.Y, velocityX, -velocityY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }

                if (npc.ai[2] >= attackTimer - 90)
                {
                    if (npc.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 100f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float x = 900f - calamityGlobalNPC.newAI[2];
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                        }
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Transition to phase 3 and summon sharks from above
            else if (npc.ai[0] == 9f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Sound
                if (npc.ai[2] == phaseTransitionTimer - 60)
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                if (npc.ai[2] >= phaseTransitionTimer - 60)
                {
                    if (npc.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 200f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 50f + calamityGlobalNPC.newAI[2]), (int)(npc.Center.Y - 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, sharkronVelocity, 255);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 50f - calamityGlobalNPC.newAI[2]), (int)(npc.Center.Y - 540f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, sharkronVelocity, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + 50f + calamityGlobalNPC.newAI[2] * 0.5f), (int)(npc.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, 1f, -sharkronVelocity, 255);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - 50f - calamityGlobalNPC.newAI[2] * 0.5f), (int)(npc.Center.Y + 270f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, -1f, -sharkronVelocity, 255);
                            }
                        }
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= phaseTransitionTimer)
                {
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Phase 3
            else if (npc.ai[0] == 10f && !player.dead)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Alpha
                npc.alpha -= 25;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                // Movement location
                if (npc.ai[1] == 0f)
                    npc.ai[1] = 500 * Math.Sign((npc.Center - player.Center).X);

                Vector2 desiredVelocity = Vector2.Normalize(player.Center + new Vector2(-npc.ai[1], -300f) - npc.Center - npc.velocity) * idlePhaseVelocity;
                npc.SimpleFlyMovement(desiredVelocity, idlePhaseAcceleration);

                // Rotation and direction
                int phase3FaceDirection = Math.Sign(player.Center.X - npc.Center.X);
                if (phase3FaceDirection != 0)
                {
                    if (npc.ai[2] == 0f && phase3FaceDirection != npc.direction)
                    {
                        npc.rotation += pie;
                        for (int l = 0; l < npc.oldPos.Length; l++)
                            npc.oldPos[l] = Vector2.Zero;
                    }

                    npc.direction = phase3FaceDirection;

                    if (npc.spriteDirection != -npc.direction)
                        npc.rotation += pie;

                    npc.spriteDirection = -npc.direction;
                }

                // Phase switch
                if (calamityGlobalNPC.newAI[1] != 1f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= idlePhaseTimer)
                    {
                        int phase3AttackPicker = 0;
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                            case 2:
                            case 3:
                            case 5:
                            case 6:
                            case 7:
                                phase3AttackPicker = 1;
                                break;
                            case 1:
                            case 8:
                                phase3AttackPicker = 2;
                                break;
                            case 4:
                                npc.ai[3] = 1f;
                                phase3AttackPicker = 3;
                                break;
                            case 9:
                                npc.ai[3] = 6f;
                                phase3AttackPicker = 4;
                                break;
                        }

                        // Set velocity for charge
                        if (phase3AttackPicker == 1)
                        {
                            npc.ai[0] = 11f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;

                            // Velocity and rotation
                            Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                            npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            // Direction
                            if (phase3FaceDirection != 0)
                            {
                                npc.direction = phase3FaceDirection;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }
                        }

                        // Pause
                        else if (phase3AttackPicker == 2)
                        {
                            npc.ai[0] = 12f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        else if (phase3AttackPicker == 3)
                        {
                            npc.ai[0] = 13f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        // Set velocity for spin
                        else if (phase3AttackPicker == 4)
                        {
                            // Velocity and rotation
                            npc.velocity = Vector2.Normalize(player.Center - npc.Center) * spinAttackSpeed;
                            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                            // Direction
                            if (phase3FaceDirection != 0)
                            {
                                npc.direction = phase3FaceDirection;

                                if (npc.spriteDirection == 1)
                                    npc.rotation += pie;

                                npc.spriteDirection = -npc.direction;
                            }

                            npc.ai[0] = 14f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }

                        npc.netUpdate = true;
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 11f)
            {
                // Set damage
                npc.damage = setDamage;

                // The dumbest thing to ever exist
                if (CalamityWorld.LegendaryMode && revenge && npc.ai[2] % 6f == 0f)
                {
                    // Rotation and direction
                    int dir = Math.Sign(player.Center.X - npc.Center.X);
                    if (dir != 0)
                    {
                        if (npc.ai[2] == 0f && dir != npc.direction)
                            npc.rotation += pie;

                        npc.direction = dir;

                        if (npc.spriteDirection != -npc.direction)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }

                    Vector2 distanceVector = player.Center + player.velocity * 20f - npc.Center;
                    npc.velocity = Vector2.Normalize(distanceVector) * chargeVelocity;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                    // Direction
                    if (dir != 0)
                    {
                        npc.direction = dir;

                        if (npc.spriteDirection == 1)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }
                }
                else
                {
                    // Accelerate
                    npc.velocity *= 1.01f;
                }

                // Spawn dust
                int phase3ChargeDustAmt = 7;
                for (int m = 0; m < phase3ChargeDustAmt; m++)
                {
                    Vector2 arg_2444_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((m - (phase3ChargeDustAmt / 2 - 1)) * pie / phase3ChargeDustAmt) + npc.Center;
                    Vector2 vector11 = ((float)(Main.rand.NextDouble() * pie) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int phase3ChargeDust = Dust.NewDust(arg_2444_0 + vector11, 0, 0, (int)CalamityDusts.SulphurousSeaAcid, vector11.X * 2f, vector11.Y * 2f, 100, default, 1.4f);
                    Main.dust[phase3ChargeDust].noGravity = true;
                    Main.dust[phase3ChargeDust].noLight = true;
                    Main.dust[phase3ChargeDust].velocity /= 4f;
                    Main.dust[phase3ChargeDust].velocity -= npc.velocity;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= chargeTime)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Pause before teleport
            else if (npc.ai[0] == 12f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Alpha
                if (npc.alpha < 255 && npc.ai[2] >= teleportPauseTimer - 15f)
                {
                    npc.alpha += 17;
                    if (npc.alpha > 255)
                        npc.alpha = 255;
                }

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound
                if (npc.ai[2] == teleportPauseTimer / 2)
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == teleportPauseTimer / 2)
                {
                    // Teleport location
                    if (npc.ai[1] == 0f)
                        npc.ai[1] = 600 * Math.Sign((npc.Center - player.Center).X);

                    // Rotation and direction
                    Vector2 center = player.Center + new Vector2(npc.ai[1], -300f);
                    Vector2 npcCenter = npc.Center = center;
                    int phase3TeleportFaceDirection = Math.Sign(player.Center.X - npcCenter.X);
                    if (phase3TeleportFaceDirection != 0)
                    {
                        if (npc.ai[2] == 0f && phase3TeleportFaceDirection != npc.direction)
                        {
                            npc.rotation += pie;
                            for (int n = 0; n < npc.oldPos.Length; n++)
                                npc.oldPos[n] = Vector2.Zero;
                        }

                        npc.direction = phase3TeleportFaceDirection;

                        if (npc.spriteDirection != -npc.direction)
                            npc.rotation += pie;

                        npc.spriteDirection = -npc.direction;
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= teleportPauseTimer)
                {
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;

                    npc.ai[3] += 2f;
                    if (npc.ai[3] >= 9f)
                        npc.ai[3] = 0f;

                    npc.netUpdate = true;
                }
            }

            // Vomit a huge amount of gore into the sky and call sharks from the sides of the screen
            else if (npc.ai[0] == 13f)
            {
                // Avoid cheap bullshit
                npc.damage = 0;

                // Velocity
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);

                // Play sound
                if (npc.ai[2] == attackTimer - 30)
                {
                    SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 phase3GoreDirection = npc.rotation.ToRotationVector2() * (Vector2.UnitX * npc.direction) * (npc.width + 20) / 2f + npc.Center;
                        int type = ModContent.ProjectileType<OldDukeGore>();
                        int damage = npc.GetProjectileDamage(type);
                        int totalGore = Main.getGoodWorld ? 40 : 20;
                        for (int i = 0; i < totalGore; i++)
                        {
                            float velocityX = npc.direction * goreVelocityX * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);
                            float velocityY = goreVelocityY * (Main.rand.NextFloat(0.2f, 0.8f) + 0.5f);

                            if (Main.getGoodWorld)
                            {
                                velocityX *= Main.rand.NextFloat() + 0.5f;
                                velocityY *= Main.rand.NextFloat() + 0.5f;
                            }

                            Projectile.NewProjectile(npc.GetSource_FromAI(), phase3GoreDirection.X, phase3GoreDirection.Y, velocityX, -velocityY, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }

                if (npc.ai[2] >= attackTimer - 90)
                {
                    if (npc.ai[2] % 18f == 0f)
                    {
                        calamityGlobalNPC.newAI[2] += 150f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float x = 900f - calamityGlobalNPC.newAI[2];
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2]), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);

                            if (Main.getGoodWorld)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X + x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2] * 0.5f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.Center.X - x), (int)(npc.Center.Y - calamityGlobalNPC.newAI[2] * 0.5f), ModContent.NPCType<SulphurousSharkron>(), 0, 0f, 0f, npc.whoAmI, 0f, 255);
                            }
                        }
                    }
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= attackTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Tooth Ball and Vortex spin
            else if (npc.ai[0] == 14f)
            {
                // Set damage
                npc.damage = 0;

                // Play sounds and spawn Tooth Balls and a Vortex
                if (npc.ai[2] == 0f)
                {
                    modNPC.RoarSoundSlot = SoundEngine.PlaySound(OldDuke.OldDuke.RoarSound, npc.Center);

                    int type = ModContent.ProjectileType<OldDukeVortex>();
                    int damage = npc.GetProjectileDamage(type);
                    Vector2 vortexSpawn = npc.Center + npc.velocity.RotatedBy(MathHelper.PiOver2 * -npc.direction) * spinTime / MathHelper.TwoPi;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vortexSpawn, Vector2.Zero, type, damage, 0f, Main.myPlayer, vortexSpawn.X, vortexSpawn.Y);
                }

                if (npc.ai[2] % toothBallSpinPhaseDivisor == 0f)
                {
                    if (npc.ai[2] != 0f)
                        SoundEngine.PlaySound(OldDuke.OldDuke.VomitSound, npc.Center);

                    Vector2 phase3ToothBallDirection = Vector2.Normalize(npc.velocity) * (npc.width + 20) / 2f + npc.Center;
                    Vector2 toothBallVelocity = Vector2.Normalize(npc.velocity).RotatedBy(MathHelper.PiOver2 * npc.direction) * toothBallSpinToothBallVelocity;
                    Vector2 toothBallSpawnPos = new Vector2(phase3ToothBallDirection.X, phase3ToothBallDirection.Y + 45f);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int toothBall = NPC.NewNPC(npc.GetSource_FromAI(), (int)toothBallSpawnPos.X, (int)toothBallSpawnPos.Y, ModContent.NPCType<OldDukeToothBall>(), 0, toothBallVelocity.X, toothBallVelocity.Y);
                        Main.npc[toothBall].target = npc.target;
                        Main.npc[toothBall].velocity = Vector2.Normalize(toothBallVelocity) * toothBallSpinToothBallVelocity * 0.5f;
                        Main.npc[toothBall].netUpdate = true;
                        Main.npc[toothBall].ai[3] = 60f;
                    }

                    for (int i = 0; i < 50; i++)
                    {
                        int dustID;
                        switch (Main.rand.Next(6))
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                dustID = (int)CalamityDusts.SulphurousSeaAcid;
                                break;
                            default:
                                dustID = DustID.Blood;
                                break;
                        }

                        // Choose a random speed and angle to belch out the vomit
                        float dustSpeed = Main.rand.NextFloat(3.0f, 12.0f);
                        float angleRandom = 0.06f;
                        Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(toothBallVelocity.ToRotation());
                        dustVel = dustVel.RotatedBy(-angleRandom);
                        dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                        // Pick a size for the vomit particles
                        float scale = Main.rand.NextFloat(1f, 2f);

                        // Actually spawn the vomit
                        int idx = Dust.NewDust(toothBallSpawnPos, 40, 40, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                        Main.dust[idx].noGravity = true;
                    }
                }

                // Velocity and rotation
                npc.velocity = npc.velocity.RotatedBy(-(double)spinSpeed * npc.direction);
                npc.rotation -= spinSpeed * npc.direction;

                npc.ai[2] += 1f;
                if (npc.ai[2] >= toothBallSpinTimer)
                {
                    calamityGlobalNPC.newAI[0] += exhaustionIncreasePerAttack;
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            if (SoundEngine.TryGetActiveSound(modNPC.RoarSoundSlot, out var roarSound) && roarSound.IsPlaying)
            {
                roarSound.Position = npc.Center;
            }
        }
    }
}
