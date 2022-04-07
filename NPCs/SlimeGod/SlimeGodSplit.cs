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

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SlimeGodSplit : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ebonian Slime God");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.LifeMaxNERB(1500, 1800, 110000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.GetNPCDamage();
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 0.8f;
            NPC.defense = 8;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            animationType = NPCID.KingSlime;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.alpha = 55;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            music = CalamityMod.Instance.GetMusicFromMusicMod("SlimeGod") ?? MusicID.Boss1;
            bossBag = ModContent.ItemType<SlimeGodBag>();
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
            if (CalamityGlobalNPC.slimeGodPurple < 0 || !Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                CalamityGlobalNPC.slimeGodPurple = NPC.whoAmI;

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
                NPC.defense = NPC.defDefense + 16;
                NPC.damage = NPC.defDamage + 20;
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

            float distanceSpeedBoost = Vector2.Distance(player.Center, vector) * (malice ? 0.008f : 0.005f);

            bool flag100 = false;
            bool hyperMode = NPC.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
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

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += flag100 ? 0.5f : 1f;
                if (revenge)
                    NPC.localAI[0] += 0.5f;
                if (malice)
                    NPC.localAI[0] += 1f;

                if (NPC.localAI[0] >= 600f && Vector2.Distance(player.Center, NPC.Center) > 160f)
                {
                    NPC.localAI[0] = 0f;
                    float num179 = 6f;
                    Vector2 value9 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                    float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                    float num181 = Math.Abs(num180) * 0.1f;
                    float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                    float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                    num183 = num179 / num183;
                    num180 *= num183;
                    num182 *= num183;
                    int type = ModContent.ProjectileType<AbyssBallVolley>();
                    int damage = NPC.GetProjectileDamage(type);
                    value9.X += num180;
                    value9.Y += num182;
                    int totalProjectiles = expertMode ? 3 : 2;
                    int spread = expertMode ? 45 : 30;
                    for (int num186 = 0; num186 < totalProjectiles; num186++)
                    {
                        num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                        num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                        num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                        num183 = num179 / num183;
                        num180 += (float)Main.rand.Next(-spread, spread + 1);
                        num182 += (float)Main.rand.Next(-spread, spread + 1);
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
                NPC.netUpdate = true;
            }
            else if (NPC.ai[0] == 1f)
            {
                if ((player.Center - vector).Length() > (hyperMode ? 1200f : 3600f))
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
                    NPC.ai[1] += flag100 ? 1f : 2f;
                    float num1879 = 40f;
                    float num1880 = death ? 5.5f : revenge ? 5f : expertMode ? 4.5f : 4f;
                    if (revenge)
                    {
                        float moveBoost = death ? 20f * (1f - lifeRatio) : 10f * (1f - lifeRatio);
                        float speedBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                        num1879 -= moveBoost;
                        num1880 += speedBoost;
                    }
                    float num1881 = 5f;
                    if (!Collision.CanHit(vector, 1, 1, player.Center, 1, 1))
                    {
                        num1881 += 2f;
                    }
                    if (NPC.ai[1] > num1879)
                    {
                        num1881 *= 1.25f;
                        num1880 *= 0.75f;
                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= num1881;
                        NPC.velocity.X = (num1880 + distanceSpeedBoost) * NPC.direction;
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
                    NPC.ai[2] += death ? 1f - lifeRatio : 0.5f * (1f - lifeRatio);
                }
                if (NPC.ai[2] >= 210f && NPC.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int random = Main.rand.Next(2) + (lifeRatio < 0.5f ? 1 : 0);
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
                NPC.localAI[0] += 5f;
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
                    if (value74.Length() < 280f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                    if (value74.Length() > 50f)
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
                            NPC.velocity.Y -= 3f;
                            if (player.position.Y + player.height < vector.Y)
                            {
                                NPC.velocity.Y -= 1.2f;
                            }
                            if (player.position.Y + player.height < vector.Y - 40f)
                            {
                                NPC.velocity.Y -= 1.4f;
                            }
                            if (player.position.Y + player.height < vector.Y - 80f)
                            {
                                NPC.velocity.Y -= 1.7f;
                            }
                            if (player.position.Y + player.height < vector.Y - 120f)
                            {
                                NPC.velocity.Y -= 2f;
                            }
                            if (player.position.Y + player.height < vector.Y - 160f)
                            {
                                NPC.velocity.Y -= 2.2f;
                            }
                            if (player.position.Y + player.height < vector.Y - 200f)
                            {
                                NPC.velocity.Y -= 2.4f;
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
                    if (NPC.ai[2] >= 2f && NPC.velocity.Y == 0f)
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
            int num244 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 173, NPC.velocity.X, NPC.velocity.Y, 255, new Color(0, 80, 255, 80), NPC.scale * 1.2f);
            Main.dust[num244].noGravity = true;
            Main.dust[num244].velocity *= 0.5f;
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
                    NPC.scale = num659 * 0.75f;
                    NPC.width = (int)(150f * NPC.scale);
                    NPC.height = (int)(92f * NPC.scale);
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)NPC.height;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)((double)NPC.lifeMax * 0.2);
                    if ((float)(NPC.life + num660) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                        int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                        int num663 = ModContent.NPCType<SlimeSpawnCorrupt>();
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
            Color lightColor = new Color(200, 150, Main.DiscoB, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : drawColor;
            return newColor;
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int heartAmt = Main.rand.Next(3) + 3;
                for (int i = 0; i < heartAmt; i++)
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }

            bool otherSlimeGodsAlive =
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) ||
                NPC.CountNPCS(ModContent.NPCType<SlimeGodSplit>()) > 1 || // the other ebonian split is alive
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>());
            if (!otherSlimeGodsAlive)
                SlimeGodCore.DropSlimeGodLoot(NPC);
        }

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
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Weak, 120, true);
        }
    }
}
