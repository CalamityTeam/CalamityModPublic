using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class EbonianPaladin : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 1.1f;
            NPC.defense = 10;
            NPC.LifeMaxNERB(8000, 9600, 220000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.KingSlime;
            NPC.value = 0f;
            NPC.Opacity = 0.8f;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            CalamityGlobalNPC.slimeGodPurple = NPC.whoAmI;
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || NPC.localAI[1] == 1f || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            NPC.defense = NPC.defDefense;
            NPC.damage = NPC.defDamage;
            if (NPC.localAI[1] == 1f)
            {
                NPC.defense = NPC.defDefense + 20;
                NPC.damage = NPC.defDamage + 22;
            }

            float scale = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 0.6f : Main.getGoodWorld ? 0.8f : 1f;
            NPC.aiAction = 0;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (NPC.ai[0] != 4f && (player.dead || !player.active))
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (player.dead || !player.active)
                {
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (lifeRatio <= 0.5f && Main.netMode != NetmodeID.MultiplayerClient && expertMode)
            {
                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                {
                    int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                    for (int i = 0; i < 30; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1199, 1200) * 0.01f, (float)Main.rand.Next(-1199, 1200) * 0.01f, type, 35, 0f);
                }

                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
                Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X - 30, (int)spawnAt.Y, ModContent.NPCType<SplitEbonianPaladin>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X + 30, (int)spawnAt.Y, ModContent.NPCType<SplitEbonianPaladin>());
                if (Main.zenithWorld && NPC.CountNPCS(ModContent.NPCType<SplitCrimulanPaladin>()) < 3) // split into 3 slimes if the other large slime hasn't split yet
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X, (int)spawnAt.Y - 30, ModContent.NPCType<SplitEbonianPaladin>());
                }
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool enraged = true;
            bool hyperMode = NPC.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                {
                    enraged = false;
                }
            }
            if (bossRush)
            {
                enraged = true;
                hyperMode = true;
            }

            // Teleport
            float teleportGateValue = 720f;
            if (!player.dead && NPC.timeLeft > 10 && calamityGlobalNPC.newAI[0] >= teleportGateValue && NPC.ai[0] == 0f && NPC.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down dramatically
                NPC.velocity.X *= 0.5f;

                NPC.ai[0] = 6f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.netUpdate = true;
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                    Vector2 vectorAimedAheadOfTarget = player.Center + new Vector2((float)Math.Round(player.velocity.X), 0f).SafeNormalize(Vector2.Zero) * 960f;
                    Point teleportAheadPos = vectorAimedAheadOfTarget.ToTileCoordinates();
                    int maxTeleportTries = 0;
                    while (maxTeleportTries < 100)
                    {
                        maxTeleportTries++;
                        int teleportPosX = Main.rand.Next(teleportAheadPos.X - 5, teleportAheadPos.X + 6);
                        int teleportPosY = Main.rand.Next(teleportAheadPos.Y - 5, teleportAheadPos.Y);
                        if (Main.tile[teleportPosX, teleportPosY].HasUnactuatedTile)
                            continue;

                        bool canTeleport = true;
                        if (canTeleport && Main.tile[teleportPosX, teleportPosY].LiquidType == LiquidID.Lava)
                            canTeleport = false;
                        if (canTeleport && !Collision.CanHitLine(NPC.Center, 0, 0, vectorAimedAheadOfTarget, 0, 0))
                            canTeleport = false;

                        if (canTeleport)
                        {
                            NPC.localAI[0] = teleportPosX * 16 + 8;
                            NPC.localAI[3] = teleportPosY * 16 + 16;
                            calamityGlobalNPC.newAI[0] = 0f;
                            break;
                        }
                    }

                    // Default teleport if the above conditions aren't met in 100 iterations
                    if (maxTeleportTries >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[0] = bottom.X;
                        NPC.localAI[3] = bottom.Y;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }
            }

            // Get ready to teleport by increasing ai[3]
            if (calamityGlobalNPC.newAI[0] < teleportGateValue)
            {
                if (!Collision.CanHitLine(NPC.Center, 0, 0, player.Center, 0, 0) || Math.Abs(NPC.Top.Y - player.Bottom.Y) > 320f)
                    calamityGlobalNPC.newAI[0] += death ? 3f : 2f;
                else
                    calamityGlobalNPC.newAI[0] += 1f;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, NPC.Center) * (bossRush ? 0.008f : 0.005f);

            if (NPC.ai[0] == 0f)
            {
                bool phaseThroughTilesToReachTarget = Vector2.Distance(player.Center, NPC.Center) > 2400f || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (Main.netMode != NetmodeID.MultiplayerClient && phaseThroughTilesToReachTarget)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y == 0f)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += hyperMode ? 2f : 1f;

                    float jumpGateValue = 60f;
                    float velocityX = death ? 8f : revenge ? 7f : expertMode ? 6f : 4f;
                    if (revenge)
                    {
                        float moveBoost = death ? 60f * (1f - lifeRatio) : 40f * (1f - lifeRatio);
                        float speedBoost = death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                        jumpGateValue -= moveBoost;
                        velocityX += speedBoost;
                    }

                    float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                    float speedMult = 1f;
                    if (distanceBelowTarget > 0f)
                        speedMult += distanceBelowTarget * 0.002f;

                    if (speedMult > 2f)
                        speedMult = 2f;

                    float velocityY = 5f;
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                        velocityY += 2f;

                    if (NPC.ai[1] > jumpGateValue)
                    {
                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] >= 2f)
                        {
                            NPC.ai[3] = 0f;
                            velocityY *= 1.25f;
                        }

                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= velocityY * speedMult;
                        NPC.velocity.X = (velocityX + distanceSpeedBoost) * NPC.direction;
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[1] >= jumpGateValue - 30f)
                        NPC.aiAction = 1;
                }
                else
                {
                    NPC.velocity.X *= 0.99f;
                    if (NPC.direction < 0 && NPC.velocity.X > -1f)
                        NPC.velocity.X = -1f;
                    if (NPC.direction > 0 && NPC.velocity.X < 1f)
                        NPC.velocity.X = 1f;

                    if (!player.dead)
                    {
                        if (NPC.velocity.Y > 0f && NPC.Bottom.Y > player.Top.Y)
                            NPC.noTileCollide = false;
                        else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            NPC.noTileCollide = false;
                        else
                            NPC.noTileCollide = true;
                    }
                }

                NPC.ai[2] += 1f;
                if (revenge)
                    NPC.ai[2] += death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);

                if (NPC.velocity.Y == 0f)
                {
                    float phaseSwitchGateValue = 420f;
                    bool switchPhase = NPC.ai[2] >= phaseSwitchGateValue;
                    if (switchPhase)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            switch ((int)NPC.localAI[2])
                            {
                                default:
                                case 1:
                                    NPC.ai[0] = Main.rand.NextBool() ? 2f : 3f;
                                    break;

                                case 2:
                                    NPC.ai[0] = Main.rand.NextBool() ? 3f : (hyperMode ? 2f : 1f);
                                    break;

                                case 3:
                                    NPC.ai[0] = Main.rand.NextBool() ? (hyperMode ? 3f : 1f) : 2f;
                                    break;
                            }

                            if (NPC.ai[0] == 2f)
                            {
                                NPC.noTileCollide = true;
                                NPC.velocity.Y = death ? -9f : revenge ? -8f : expertMode ? -7f : -6f;
                            }

                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] >= phaseSwitchGateValue - 30f)
                        NPC.aiAction = 1;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.velocity.X *= 0.85f;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 30f)
                {
                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 targetCenter = player.Center;
                targetCenter.Y -= 350f;
                Vector2 targetDist = targetCenter - NPC.Center;

                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    targetDist = player.Center - NPC.Center;
                    targetDist.Normalize();
                    targetDist *= death ? 9f : revenge ? 8f : expertMode ? 7f : 6f;
                    NPC.velocity = (NPC.velocity * 4f + targetDist) / 5f;

                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 2.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = targetDist;
                    }
                }
                else
                {
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 40f && NPC.Center.Y < player.Center.Y - 300f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 1f;
                        return;
                    }

                    targetDist.Normalize();
                    targetDist *= (death ? 11f : revenge ? 10f : expertMode ? 9f : 8f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + targetDist) / 6f;
                }
            }
            else if (NPC.ai[0] == 2.1f)
            {
                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && atTargetPosition && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }

                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        SoundEngine.PlaySound(SlimeGodCore.BigShotSound, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Eruption of slime balls
                            float projectileVelocity = 4f;
                            int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = 11;
                            float rotation = MathHelper.ToRadians(90);
                            for (int i = 0; i < numProj; i++)
                            {
                                // Spawn projectiles 0, 1, 2, 3, 7, 8, 9 and 10
                                if (i < 4 || i > 6)
                                {
                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 30f * NPC.scale + Vector2.Normalize(perturbedSpeed) * 30f * NPC.scale, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                }
                            }

                            // Fire slime balls directly at players with a max of 2
                            if (enraged && expertMode)
                            {
                                List<int> targets = new List<int>();
                                for (int p = 0; p < Main.maxPlayers; p++)
                                {
                                    if (Main.player[p].active && !Main.player[p].dead)
                                        targets.Add(p);

                                    if (targets.Count > 1)
                                        break;
                                }
                                foreach (int t in targets)
                                {
                                    Vector2 projFireDirection = Vector2.Normalize(Main.player[t].Center - NPC.Center) * projectileVelocity;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projFireDirection) * 30f * NPC.scale, projFireDirection, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        NPC.localAI[2] = NPC.ai[0] - 0.1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 0f)
                    NPC.noTileCollide = true;

                NPC.noGravity = true;

                NPC.velocity.Y += 0.5f;
                float velocityLimit = bossRush ? 20f : death ? 15f : revenge ? 14f : expertMode ? 13f : 12f;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
            }
            else if (NPC.ai[0] == 3f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 15f)
                    {
                        NPC.ai[1] = 0f;

                        NPC.velocity.Y -= 6f;
                        if (player.position.Y + player.height < NPC.Center.Y)
                            NPC.velocity.Y -= 1.2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 40f)
                            NPC.velocity.Y -= 1.4f;
                        if (player.position.Y + player.height < NPC.Center.Y - 80f)
                            NPC.velocity.Y -= 1.7f;
                        if (player.position.Y + player.height < NPC.Center.Y - 120f)
                            NPC.velocity.Y -= 2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 160f)
                            NPC.velocity.Y -= 2.2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 200f)
                            NPC.velocity.Y -= 2.4f;
                        if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                            NPC.velocity.Y -= 2f;

                        NPC.velocity.X = ((death ? 11f : revenge ? 10f : expertMode ? 9f : 8f) + distanceSpeedBoost) * NPC.direction;
                        NPC.ai[2] += 1f;
                    }
                    else
                        NPC.aiAction = 1;
                }
                else
                {
                    NPC.velocity.X *= 0.98f;
                    float velocityLimit = (death ? 6.5f : revenge ? 6f : expertMode ? 5.5f : 5f);
                    if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        NPC.velocity.X = -velocityLimit;
                    if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        NPC.velocity.X = velocityLimit;
                }

                if (NPC.ai[2] >= 2f && NPC.velocity.Y == 0f)
                {
                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                NPC.noTileCollide = true;
                NPC.Opacity -= 0.03f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;

                NPC.velocity.X *= 0.98f;
            }
            else if (NPC.ai[0] == 5f)
            {
                if (NPC.velocity.X > 0f)
                    NPC.direction = 1;
                else
                    NPC.direction = -1;

                NPC.spriteDirection = NPC.direction;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.knockBackResist = 0f;
                Vector2 distanceFromTarget = player.Center - NPC.Center;
                distanceFromTarget.Y -= 16f;
                if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromTarget.Length() < 500f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }

                if (distanceFromTarget.Length() > 12f)
                {
                    distanceFromTarget.Normalize();
                    distanceFromTarget *= 12f;
                }

                NPC.velocity = (NPC.velocity * 4f + distanceFromTarget) / 5f;
            }
            else if (NPC.ai[0] == 6f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.aiAction = 1;
                NPC.ai[1] += 1f;
                float teleportTime = bossRush ? 20f : death ? 30f : 40f;
                scale = MathHelper.Clamp((teleportTime - NPC.ai[1]) / teleportTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Bottom = new Vector2(NPC.localAI[0], NPC.localAI[3]);
                    NPC.ai[0] = 7f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportTime * 2f)
                {
                    NPC.ai[0] = 7f;
                    NPC.ai[1] = 0f;
                }

                // Emit teleport dust
                Color dustColor = Color.Lavender;
                dustColor.A = 150;
                for (int i = 0; i < 10; i++)
                {
                    int corruptDust = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptDust].noGravity = true;
                    Main.dust[corruptDust].velocity *= 0.5f;
                }
            }
            else if (NPC.ai[0] == 7f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[1] += 1f;
                float teleportEndTime = bossRush ? 10f : death ? 15f : 20f;
                scale = MathHelper.Clamp(NPC.ai[1] / teleportEndTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    NPC.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportEndTime * 2f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.TargetClosest();
                }

                // Emit teleport dust
                Color dustColor = Color.Lavender;
                dustColor.A = 150;
                for (int i = 0; i < 10; i++)
                {
                    int corruptDust = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptDust].noGravity = true;
                    Main.dust[corruptDust].velocity *= 0.5f;
                }
            }

            if (bossLife == 0f && NPC.life > 0)
                bossLife = NPC.lifeMax;

            if (NPC.life > 0)
            {
                float scaleRatio = lifeRatio;
                scaleRatio = scaleRatio * 0.5f + 0.75f;
                scaleRatio *= scale;

                if (scaleRatio != NPC.scale)
                {
                    NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (float)NPC.height;
                    NPC.scale = scaleRatio;
                    NPC.width = (int)(150f * NPC.scale);
                    NPC.height = (int)(92f * NPC.scale);
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)NPC.height;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int slimeSpawnThreshold = (int)((double)NPC.lifeMax * 0.2);
                    if ((float)(NPC.life + slimeSpawnThreshold) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int randSlimeAmt = Main.rand.Next(1, 3);
                        for (int j = 0; j < randSlimeAmt; j++)
                        {
                            int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                            int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                            int slimeType = ModContent.NPCType<CorruptSlimeSpawn>();
                            int slimeSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, slimeType, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[slimeSpawn].SetDefaults(slimeType);
                            Main.npc[slimeSpawn].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                            Main.npc[slimeSpawn].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                            Main.npc[slimeSpawn].ai[0] = (float)(-1000 * Main.rand.Next(3));
                            Main.npc[slimeSpawn].ai[1] = 0f;
                            if (Main.netMode == NetmodeID.Server && slimeSpawn < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slimeSpawn, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color lightColor = new Color(200, 150, Main.DiscoB, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : drawColor;
            return newColor * NPC.Opacity;
        }

        public override bool CheckActive() => false;

        public override void HitEffect(NPC.HitInfo hit)
        {
            Color dustColor = Color.Lavender;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Weak, 300, true);
        }
    }
}
