using CalamityMod.Events;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
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

            Vector2 vector = NPC.Center;

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
            if (Vector2.Distance(Main.player[NPC.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (NPC.ai[0] != 6f && (player.dead || !player.active))
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (player.dead || !player.active)
                {
                    NPC.ai[0] = 6f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (Vector2.Distance(player.Center, vector) > 5400f)
            {
                NPC.position.X = player.Center.X / 16 * 16f - (NPC.width / 2);
                NPC.position.Y = player.Center.Y / 16 * 16f - (NPC.height / 2) - 150f;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, vector) * (malice ? 0.008f : 0.005f);

            if (lifeRatio <= 0.5f && Main.netMode != NetmodeID.MultiplayerClient && expertMode)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.position);
                Vector2 spawnAt = vector + new Vector2(0f, NPC.height / 2f);
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)spawnAt.X - 30, (int)spawnAt.Y, ModContent.NPCType<SlimeGodRunSplit>());
                NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)spawnAt.X + 30, (int)spawnAt.Y, ModContent.NPCType<SlimeGodRunSplit>());
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            bool flag100 = false;
            bool hyperMode = NPC.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodPurple != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                {
                    flag100 = true;
                }
            }
            if (CalamityGlobalNPC.slimeGod < 0 || !Main.npc[CalamityGlobalNPC.slimeGod].active)
            {
                NPC.localAI[1] = 0f;
                hyperMode = true;
                flag100 = false;
            }
            if (malice)
            {
                flag100 = false;
                hyperMode = true;
            }

            if (NPC.localAI[1] != 1f)
            {
                if (!flag100)
                    NPC.defense = NPC.defDefense * 2;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += flag100 ? 1f : 2f;
                if (revenge)
                    NPC.localAI[0] += 0.5f;
                if (malice)
                    NPC.localAI[0] += 1f;

                if (NPC.localAI[0] >= 450f && Vector2.Distance(player.Center, NPC.Center) > 160f)
                {
                    NPC.localAI[0] = 0f;
                    float num179 = expertMode ? 12f : 10f;
                    Vector2 value9 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                    float num181 = Math.Abs(num180) * 0.1f;
                    float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                    float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                    num183 = num179 / num183;
                    num180 *= num183;
                    num182 *= num183;
                    int type = ModContent.ProjectileType<AbyssBallVolley2>();
                    int damage = NPC.GetProjectileDamage(type);
                    value9.X += num180;
                    value9.Y += num182;
                    for (int num186 = 0; num186 < 2; num186++)
                    {
                        num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                        num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                        num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                        num183 = num179 / num183;
                        num180 += (float)Main.rand.Next(-10, 11);
                        num182 += (float)Main.rand.Next(-10, 11);
                        num180 *= num183;
                        num182 *= num183;
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), value9.X, value9.Y, num180, num182, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                NPC.ai[0] = 1f;
                NPC.ai[1] = 0f;
            }
            else if (NPC.ai[0] == 1f)
            {
                if ((player.Center - vector).Length() > (hyperMode ? 1200f : 2400f))
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
                    NPC.ai[1] += 1f;
                    float num1879 = 50f;
                    float num1880 = death ? 4.5f : revenge ? 4f : expertMode ? 3.5f : 3f;
                    if (revenge)
                    {
                        float moveBoost = death ? 60f * (1f - lifeRatio) : 30f * (1f - lifeRatio);
                        float speedBoost = death ? 8f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                        num1879 -= moveBoost;
                        num1880 += speedBoost;
                    }
                    float num1881 = 4f;
                    if (!Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
                    {
                        num1881 += 2f;
                    }
                    if (NPC.ai[1] > num1879)
                    {
                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] >= 2f)
                        {
                            NPC.ai[3] = 0f;
                            num1881 *= 0.75f;
                            num1880 *= 1.25f;
                        }
                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= num1881;
                        NPC.velocity.X = (num1880 + distanceSpeedBoost) * NPC.direction;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    NPC.velocity.X *= 0.99f;
                    if (NPC.direction < 0 && NPC.velocity.X > -1f)
                    {
                        NPC.velocity.X = -1f;
                    }
                    if (NPC.direction > 0 && NPC.velocity.X < 1f)
                    {
                        NPC.velocity.X = 1f;
                    }
                }
                NPC.ai[2] += 1f;
                if (revenge)
                {
                    NPC.ai[2] += death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                }
                if (NPC.ai[2] >= 360f && NPC.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int random = Main.rand.Next(2) + (lifeRatio < 0.75f ? 1 : 0);
                    switch (random)
                    {
                        case 0:
                            NPC.ai[0] = 2f;
                            break;
                        case 1:
                            NPC.ai[0] = 3f;
                            NPC.noTileCollide = true;
                            NPC.velocity.Y = death ? -10f : revenge ? -9f : expertMode ? -8f : -7f;
                            break;
                        case 2:
                            NPC.ai[0] = 5f;
                            break;
                        default:
                            break;
                    }
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.velocity.X *= 0.85f;
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 40f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                if (NPC.velocity.X < 0f)
                {
                    NPC.direction = -1;
                }
                else
                {
                    NPC.direction = 1;
                }
                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 center40 = player.Center;
                center40.Y -= 350f;
                Vector2 vector272 = center40 - vector;
                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    vector272 = player.Center - vector;
                    vector272.Normalize();
                    vector272 *= death ? 10f : revenge ? 9f : expertMode ? 8f : 7f;
                    NPC.velocity = (NPC.velocity * 4f + vector272) / 5f;
                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 3.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = vector272;
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    if (Math.Abs(vector.X - player.Center.X) < 40f && vector.Y < player.Center.Y - 300f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 1f;
                        NPC.netUpdate = true;
                        return;
                    }
                    vector272.Normalize();
                    vector272 *= (death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + vector272) / 6f;
                }
            }
            else if (NPC.ai[0] == 3.1f)
            {
                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && (atTargetPosition || NPC.localAI[1] == 0f) && Collision.CanHit(vector, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }
                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            NPC.ai[0] = 4f;

                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 0f)
                {
                    NPC.noTileCollide = true;
                }
                NPC.noGravity = true;
                NPC.velocity.Y += 0.5f;
                float velocityLimit = malice ? 20f : death ? 15f : revenge ? 14f : expertMode ? 13f : 12f;
                if (NPC.velocity.Y > velocityLimit)
                {
                    NPC.velocity.Y = velocityLimit;
                }
            }
            else
            {
                if (NPC.ai[0] == 4f)
                {
                    if (NPC.velocity.X > 0f)
                    {
                        NPC.direction = 1;
                    }
                    else
                    {
                        NPC.direction = -1;
                    }
                    NPC.spriteDirection = NPC.direction;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    Vector2 value74 = player.Center - vector;
                    value74.Y -= 40f;
                    if (value74.Length() < 320f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                    if (value74.Length() > 100f)
                    {
                        value74.Normalize();
                        value74 *= (death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost;
                    }
                    NPC.velocity = (NPC.velocity * 4f + value74) / 5f;
                    return;
                }
                if (NPC.ai[0] == 5f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.TargetClosest();
                        NPC.velocity.X *= 0.8f;
                        NPC.ai[1] += 1f;
                        if (NPC.ai[1] > 5f)
                        {
                            NPC.ai[1] = 0f;
                            NPC.velocity.Y -= 4f;
                            if (player.position.Y + player.height < vector.Y)
                            {
                                NPC.velocity.Y -= 1.25f;
                            }
                            if (player.position.Y + player.height < vector.Y - 40f)
                            {
                                NPC.velocity.Y -= 1.5f;
                            }
                            if (player.position.Y + player.height < vector.Y - 80f)
                            {
                                NPC.velocity.Y -= 1.75f;
                            }
                            if (player.position.Y + player.height < vector.Y - 120f)
                            {
                                NPC.velocity.Y -= 2f;
                            }
                            if (player.position.Y + player.height < vector.Y - 160f)
                            {
                                NPC.velocity.Y -= 2.25f;
                            }
                            if (player.position.Y + player.height < vector.Y - 200f)
                            {
                                NPC.velocity.Y -= 2.5f;
                            }
                            if (!Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
                            {
                                NPC.velocity.Y -= 2f;
                            }
                            NPC.velocity.X = ((death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost) * NPC.direction;
                            NPC.ai[2] += 1f;
                        }
                    }
                    else
                    {
                        NPC.velocity.X *= 0.98f;
                        float velocityLimit = (death ? 6.5f : revenge ? 6f : expertMode ? 5.5f : 5f) + distanceSpeedBoost;
                        if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        {
                            NPC.velocity.X = -velocityLimit;
                        }
                        if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        {
                            NPC.velocity.X = velocityLimit;
                        }
                    }
                    if (NPC.ai[2] >= 3f && NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[0] == 6f)
                {
                    NPC.life = NPC.lifeMax;
                    NPC.defense = 9999;
                    NPC.noTileCollide = true;
                    NPC.alpha += 7;
                    if (NPC.timeLeft > 10)
                    {
                        NPC.timeLeft = 10;
                    }
                    if (NPC.alpha > 255)
                    {
                        NPC.alpha = 255;
                    }
                    NPC.velocity.X *= 0.98f;
                }
            }
            int num658 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 260, NPC.velocity.X, NPC.velocity.Y, 255, new Color(0, 80, 255, 80), NPC.scale * 1.5f);
            Main.dust[num658].noGravity = true;
            Main.dust[num658].velocity *= 0.5f;
            if (bossLife == 0f && NPC.life > 0)
            {
                bossLife = NPC.lifeMax;
            }
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
                        int num664 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num664].SetDefaults(num663);
                        Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[num664].ai[0] = (float)(-1000 * Main.rand.Next(3));
                        Main.npc[num664].ai[1] = 0f;
                        if (Main.netMode == NetmodeID.Server && num664 < 200)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
                        }
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, hitDirection, -1f, 0, default, 1f);
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
