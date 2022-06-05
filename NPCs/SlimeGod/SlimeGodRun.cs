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
    public class SlimeGodRun : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Crimulan Slime God");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 1.1f;
            NPC.defense = 12;
            NPC.LifeMaxNERB(4750, 5700, 160000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.KingSlime;
            NPC.value = 0f;
            NPC.alpha = 55;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Music = CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod") ?? MusicID.Boss1;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC.slimeGodRed = NPC.whoAmI;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || NPC.localAI[1] == 1f || BossRushEvent.BossRushActive;
            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            NPC.defense = NPC.defDefense;
            NPC.damage = NPC.defDamage;
            if (NPC.localAI[1] == 1f)
            {
                NPC.defense = NPC.defDefense + 24;
                NPC.damage = NPC.defDamage + 25;
            }

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

            if (NPC.ai[0] != 3f && (player.dead || !player.active))
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (player.dead || !player.active)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (lifeRatio <= 0.5f && Main.netMode != NetmodeID.MultiplayerClient && expertMode)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X - 30, (int)spawnAt.Y, ModContent.NPCType<SlimeGodRunSplit>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnAt.X + 30, (int)spawnAt.Y, ModContent.NPCType<SlimeGodRunSplit>());
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool enraged = true;
            bool hyperMode = NPC.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    enraged = false;
                }
            }
            if (CalamityGlobalNPC.slimeGod < 0 || !Main.npc[CalamityGlobalNPC.slimeGod].active)
            {
                NPC.localAI[1] = 0f;
                hyperMode = true;
                enraged = true;
            }
            if (malice)
            {
                enraged = true;
                hyperMode = true;
            }

            if (NPC.localAI[1] != 1f)
            {
                if (enraged)
                    NPC.defense = NPC.defDefense * 2;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, NPC.Center) * ((malice || hyperMode) ? 0.008f : 0.005f);

            if (NPC.ai[0] == 0f)
            {
                bool phaseThroughTilesToReachTarget = Vector2.Distance(player.Center, NPC.Center) > 2400f || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (Main.netMode != NetmodeID.MultiplayerClient && phaseThroughTilesToReachTarget)
                {
                    NPC.ai[0] = 4f;
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

                    float jumpGateValue = 50f;
                    float velocityX = death ? 10f : revenge ? 9f : expertMode ? 8f : 6f;
                    if (revenge)
                    {
                        float moveBoost = death ? 45f * (1f - lifeRatio) : 30f * (1f - lifeRatio);
                        float speedBoost = death ? 4.5f * (1f - lifeRatio) : 3f * (1f - lifeRatio);
                        jumpGateValue -= moveBoost;
                        velocityX += speedBoost;
                    }

                    float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                    float speedMult = 1f;
                    if (distanceBelowTarget > 0f)
                        speedMult += distanceBelowTarget * 0.002f;

                    if (speedMult > 2f)
                        speedMult = 2f;

                    float velocityY = 4f;
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                        velocityY += 2f;

                    if (NPC.ai[1] > jumpGateValue)
                    {
                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] >= 2f)
                        {
                            NPC.ai[3] = 0f;
                            velocityY *= 0.75f;
                            velocityX *= 1.25f;
                        }

                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= velocityY * speedMult;
                        NPC.velocity.X = (velocityX + distanceSpeedBoost) * NPC.direction;
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
                }

                NPC.ai[2] += 1f;
                if (revenge)
                    NPC.ai[2] += death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);

                if (NPC.velocity.Y == 0f)
                {
                    float phaseSwitchGateValue = 360f;
                    if (NPC.ai[2] >= phaseSwitchGateValue)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            switch ((int)NPC.localAI[2])
                            {
                                default:
                                case 1:
                                    NPC.ai[0] = 2f;
                                    break;

                                case 2:
                                    NPC.ai[0] = 1f;
                                    break;

                            }

                            if (NPC.ai[0] == 1f)
                            {
                                NPC.noTileCollide = true;
                                NPC.velocity.Y = death ? -10f : revenge ? -9f : expertMode ? -8f : -7f;
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
                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 center40 = player.Center;
                center40.Y -= 350f;
                Vector2 vector272 = center40 - NPC.Center;

                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    vector272 = player.Center - NPC.Center;
                    vector272.Normalize();
                    vector272 *= death ? 10f : revenge ? 9f : expertMode ? 8f : 7f;
                    NPC.velocity = (NPC.velocity * 4f + vector272) / 5f;

                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 1.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = vector272;
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

                    vector272.Normalize();
                    vector272 *= (death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + vector272) / 6f;
                }
            }
            else if (NPC.ai[0] == 1.1f)
            {
                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && (atTargetPosition || NPC.localAI[1] == 0f) && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }

                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Eruption of slime balls that fall down
                            float projectileVelocity = 8f;
                            int type = ModContent.ProjectileType<AbyssBallVolley2>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = 4;
                            float rotation = MathHelper.ToRadians(45);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 30f * NPC.scale, perturbedSpeed * 1.5f, type, damage, 0f, Main.myPlayer, 1f, 0f);
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
                                    Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center - NPC.Center) * projectileVelocity;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(velocity2) * 30f * NPC.scale, velocity2, type, damage, 0f, Main.myPlayer);
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
                float velocityLimit = malice ? 20f : death ? 15f : revenge ? 14f : expertMode ? 13f : 12f;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
            }
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 15f)
                    {
                        NPC.ai[1] = 0f;

                        NPC.velocity.Y -= 3f;
                        if (player.position.Y + player.height < NPC.Center.Y)
                            NPC.velocity.Y -= 1.25f;
                        if (player.position.Y + player.height < NPC.Center.Y - 40f)
                            NPC.velocity.Y -= 1.5f;
                        if (player.position.Y + player.height < NPC.Center.Y - 80f)
                            NPC.velocity.Y -= 1.75f;
                        if (player.position.Y + player.height < NPC.Center.Y - 120f)
                            NPC.velocity.Y -= 2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 160f)
                            NPC.velocity.Y -= 2.25f;
                        if (player.position.Y + player.height < NPC.Center.Y - 200f)
                            NPC.velocity.Y -= 2.5f;
                        if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                            NPC.velocity.Y -= 2f;

                        NPC.velocity.X = ((death ? 14f : revenge ? 13f : expertMode ? 12f : 11f) + distanceSpeedBoost) * NPC.direction;
                        NPC.ai[2] += 1f;
                    }
                    else
                        NPC.aiAction = 1;
                }
                else
                {
                    NPC.velocity.X *= 0.98f;
                    float velocityLimit = (death ? 5.5f : revenge ? 5f : expertMode ? 4.5f : 4f);
                    if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        NPC.velocity.X = -velocityLimit;
                    if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        NPC.velocity.X = velocityLimit;
                }

                if (NPC.ai[2] >= 3f && NPC.velocity.Y == 0f)
                {
                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                NPC.alpha += 7;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.alpha > 255)
                    NPC.alpha = 255;

                NPC.velocity.X *= 0.98f;
            }
            else if (NPC.ai[0] == 4f)
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
                if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromTarget.Length() < 600f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }

                if (distanceFromTarget.Length() > 10f)
                {
                    distanceFromTarget.Normalize();
                    distanceFromTarget *= 10f;
                }

                NPC.velocity = (NPC.velocity * 4f + distanceFromTarget) / 5f;
            }

            if (bossLife == 0f && NPC.life > 0)
                bossLife = NPC.lifeMax;

            float num644 = 1f;
            if (NPC.life > 0)
            {
                float num659 = lifeRatio;
                num659 = num659 * 0.5f + 0.75f;
                num659 *= num644;

                if (num659 != NPC.scale)
                {
                    NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (float)NPC.height;
                    NPC.scale = num659;
                    NPC.width = (int)(150f * NPC.scale);
                    NPC.height = (int)(92f * NPC.scale);
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)NPC.height;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)((double)NPC.lifeMax * 0.15);
                    if ((float)(NPC.life + num660) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                        int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                        int num663 = ModContent.NPCType<SlimeSpawnCrimson>();
                        if (Main.rand.NextBool(3))
                        {
                            num663 = ModContent.NPCType<SlimeSpawnCrimson2>();
                        }
                        int num664 = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num664].SetDefaults(num663);
                        Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[num664].ai[0] = (float)(-1000 * Main.rand.Next(3));
                        Main.npc[num664].ai[1] = 0f;
                        if (Main.netMode == NetmodeID.Server && num664 < 200)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color lightColor = new Color(Main.DiscoR, 100, 150, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : drawColor;
            return newColor;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            if (SlimeGodCore.LastSlimeGodStanding())
                SlimeGodCore.RealOnKill(NPC);
        }

        // If the un-split Crimulan Slime God gets one-shotted last, it should drop the boss loot
        public override void ModifyNPCLoot(NPCLoot npcLoot) => SlimeGodCore.DefineSlimeGodLoot(npcLoot);

        public override bool CheckActive()
        {
            if (CalamityGlobalNPC.slimeGod != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGod].active)
                    return false;
            }
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, NPC.alpha, Color.Crimson, 1f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Darkness, 300, true);
        }
    }
}
