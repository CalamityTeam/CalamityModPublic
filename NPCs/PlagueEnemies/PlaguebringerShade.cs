using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlaguebringerShade : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plaguebringer");
            Main.npcFrameCount[NPC.type] = 12;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 70;
            NPC.npcSlots = 8f;
            NPC.width = 66;
            NPC.height = 66;
            NPC.defense = 24;
            NPC.DR_NERD(0.2f);
            NPC.lifeMax = 3000;
            NPC.value = Item.buyPrice(0, 1, 50, 0);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            aiType = -1;
            animationType = NPCID.QueenBee;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            banner = NPC.type;
            bannerItem = ModContent.ItemType<PlaguebringerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.1f, 0.3f, 0f);
            bool flag113 = false;
            if (!Main.player[NPC.target].ZoneJungle)
            {
                flag113 = true;
                if (NPC.timeLeft > 150)
                {
                    NPC.timeLeft = 150;
                }
            }
            else
            {
                if (NPC.timeLeft < 750)
                {
                    NPC.timeLeft = 750;
                }
            }
            int num1038 = 0;
            for (int num1039 = 0; num1039 < 255; num1039++)
            {
                if (Main.player[num1039].active && !Main.player[num1039].dead && (NPC.Center - Main.player[num1039].Center).Length() < 1000f)
                {
                    num1038++;
                }
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            bool dead4 = Main.player[NPC.target].dead;
            if (dead4 && Main.expertMode)
            {
                if ((double)NPC.position.Y < Main.worldSurface * 16.0 + 2000.0)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                }
                if (NPC.position.X < (float)(Main.maxTilesX * 8))
                {
                    NPC.velocity.X = NPC.velocity.X - 0.04f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X + 0.04f;
                }
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                    return;
                }
            }
            else if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float num1041 = NPC.ai[1];
                    int num1042;
                    do
                    {
                        num1042 = Main.rand.Next(3);
                        if (num1042 == 1)
                        {
                            num1042 = 2;
                        }
                        else if (num1042 == 2)
                        {
                            num1042 = 3;
                        }
                    }
                    while ((float)num1042 == num1041);
                    NPC.ai[0] = (float)num1042;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    return;
                }
            }
            else if (NPC.ai[0] == 0f)
            {
                int num1043 = 2; //2
                if (flag113)
                {
                    num1043 += 1;
                }
                if (NPC.ai[1] > (float)(2 * num1043) && NPC.ai[1] % 2f == 0f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                if (NPC.ai[1] % 2f == 0f)
                {
                    NPC.TargetClosest(true);
                    if (Math.Abs(NPC.position.Y + (float)(NPC.height / 2) - (Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))) < 20f)
                    {
                        NPC.localAI[0] = 1f;
                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;
                        float num1044 = 15f;
                        if (flag113)
                        {
                            num1044 += 2f;
                        }
                        Vector2 vector117 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                        float num1045 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector117.X;
                        float num1046 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector117.Y;
                        float num1047 = (float)Math.Sqrt((double)(num1045 * num1045 + num1046 * num1046));
                        num1047 = num1044 / num1047;
                        NPC.velocity.X = num1045 * num1047;
                        NPC.velocity.Y = num1046 * num1047;
                        NPC.spriteDirection = NPC.direction;
                        SoundEngine.PlaySound(SoundID.Roar, NPC.position);
                        return;
                    }
                    NPC.localAI[0] = 0f;
                    float num1048 = 12.25f;
                    float num1049 = 0.155f;
                    if (flag113)
                    {
                        num1048 += 1f;
                        num1049 += 0.075f;
                    }
                    if (NPC.position.Y + (float)(NPC.height / 2) < Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2))
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num1049;
                    }
                    else
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num1049;
                    }
                    if (NPC.velocity.Y < -12f)
                    {
                        NPC.velocity.Y = -num1048;
                    }
                    if (NPC.velocity.Y > 12f)
                    {
                        NPC.velocity.Y = num1048;
                    }
                    if (Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) > 600f)
                    {
                        NPC.velocity.X = NPC.velocity.X + 0.15f * (float)NPC.direction;
                    }
                    else if (Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) < 300f)
                    {
                        NPC.velocity.X = NPC.velocity.X - 0.15f * (float)NPC.direction;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X * 0.8f;
                    }
                    if (NPC.velocity.X < -16f)
                    {
                        NPC.velocity.X = -16f;
                    }
                    if (NPC.velocity.X > 16f)
                    {
                        NPC.velocity.X = 16f;
                    }
                    NPC.spriteDirection = NPC.direction;
                }
                else
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.direction = -1;
                    }
                    else
                    {
                        NPC.direction = 1;
                    }
                    NPC.spriteDirection = NPC.direction;
                    int num1050 = 500;
                    int num1051 = 1;
                    if (NPC.position.X + (float)(NPC.width / 2) < Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))
                    {
                        num1051 = -1;
                    }
                    if (NPC.direction == num1051 && Math.Abs(NPC.position.X + (float)(NPC.width / 2) - (Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2))) > (float)num1050)
                    {
                        NPC.ai[2] = 1f;
                    }
                    if (NPC.ai[2] != 1f)
                    {
                        NPC.localAI[0] = 1f;
                        return;
                    }
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    NPC.localAI[0] = 0f;
                    NPC.velocity *= 0.9f;
                    float num1052 = 0.105f;
                    if (flag113)
                    {
                        NPC.velocity *= 0.9f;
                        num1052 += 0.075f;
                    }
                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < num1052)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                    }
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                NPC.TargetClosest(true);
                NPC.spriteDirection = NPC.direction;
                float num1053 = 12f;
                float num1054 = 0.1f;
                if (flag113)
                {
                    num1054 = 0.12f;
                }
                Vector2 vector118 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num1055 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector118.X;
                float num1056 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - 200f - vector118.Y;
                float num1057 = (float)Math.Sqrt((double)(num1055 * num1055 + num1056 * num1056));
                if (num1057 < 400f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    return;
                }
                num1057 = num1053 / num1057;
                if (NPC.velocity.X < num1055)
                {
                    NPC.velocity.X = NPC.velocity.X + num1054;
                    if (NPC.velocity.X < 0f && num1055 > 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + num1054;
                    }
                }
                else if (NPC.velocity.X > num1055)
                {
                    NPC.velocity.X = NPC.velocity.X - num1054;
                    if (NPC.velocity.X > 0f && num1055 < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - num1054;
                    }
                }
                if (NPC.velocity.Y < num1056)
                {
                    NPC.velocity.Y = NPC.velocity.Y + num1054;
                    if (NPC.velocity.Y < 0f && num1056 > 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num1054;
                    }
                }
                else if (NPC.velocity.Y > num1056)
                {
                    NPC.velocity.Y = NPC.velocity.Y - num1054;
                    if (NPC.velocity.Y > 0f && num1056 < 0f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num1054;
                    }
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                NPC.localAI[0] = 0f;
                NPC.TargetClosest(true);
                Vector2 vector119 = new Vector2(NPC.position.X + (float)(NPC.width / 2) + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
                Vector2 vector120 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num1058 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector120.X;
                float num1059 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector120.Y;
                float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
                NPC.ai[1] += 1f;
                NPC.ai[1] += (float)(num1038 / 2);
                bool flag103 = false;
                if (NPC.ai[1] > 10f)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    flag103 = true;
                }
                if (Collision.CanHit(vector119, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && flag103)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit8, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num1061;
                        if (Main.rand.NextBool(4))
                        {
                            num1061 = ModContent.NPCType<PlagueBeeLarge>();
                        }
                        else
                        {
                            num1061 = ModContent.NPCType<PlagueBee>();
                        }
                        if (NPC.CountNPCS(ModContent.NPCType<PlagueBee>()) < 3)
                        {
                            int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, num1061, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * 0.005f;
                            Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.005f;
                            Main.npc[num1062].localAI[0] = 60f;
                            Main.npc[num1062].netUpdate = true;
                        }
                    }
                }
                if (num1060 > 400f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    float num1063 = 14.5f;
                    float num1064 = 0.105f;
                    vector120 = vector119;
                    num1058 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector120.X;
                    num1059 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector120.Y;
                    num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
                    num1060 = num1063 / num1060;
                    if (NPC.velocity.X < num1058)
                    {
                        NPC.velocity.X = NPC.velocity.X + num1064;
                        if (NPC.velocity.X < 0f && num1058 > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num1064;
                        }
                    }
                    else if (NPC.velocity.X > num1058)
                    {
                        NPC.velocity.X = NPC.velocity.X - num1064;
                        if (NPC.velocity.X > 0f && num1058 < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - num1064;
                        }
                    }
                    if (NPC.velocity.Y < num1059)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num1064;
                        if (NPC.velocity.Y < 0f && num1059 > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num1064;
                        }
                    }
                    else if (NPC.velocity.Y > num1059)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num1064;
                        if (NPC.velocity.Y > 0f && num1059 < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num1064;
                        }
                    }
                }
                else
                {
                    NPC.velocity *= 0.9f; //changed from 0.9
                }
                NPC.spriteDirection = NPC.direction;
                if (NPC.ai[2] > 2f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 1f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                float num1065 = 7f;
                float num1066 = 0.075f;
                if (flag113)
                {
                    num1066 = 0.09f;
                    num1065 = 8f;
                }
                Vector2 vector121 = new Vector2(NPC.position.X + (float)(NPC.width / 2) + (float)(40 * NPC.direction), NPC.position.Y + (float)NPC.height * 0.8f);
                Vector2 vector122 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num1067 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector122.X;
                float num1068 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - 300f - vector122.Y;
                float num1069 = (float)Math.Sqrt((double)(num1067 * num1067 + num1068 * num1068));
                NPC.ai[1] += 1f;
                bool flag104 = false;
                if (NPC.ai[1] % 35f == 34f)
                {
                    flag104 = true;
                }
                if (flag104 && NPC.position.Y + (float)NPC.height < Main.player[NPC.target].position.Y && Collision.CanHit(vector121, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    SoundEngine.PlaySound(SoundID.Item42, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num1070 = 6f;
                        if (flag113)
                        {
                            num1070 += 2f;
                        }
                        float num1071 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector121.X;
                        float num1072 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector121.Y;
                        float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
                        num1073 = num1070 / num1073;
                        num1071 *= num1073;
                        num1072 *= num1073;
                        bool fireRocket = Main.rand.NextBool(15);
                        int type = fireRocket ? ModContent.ProjectileType<HiveBombGoliath>() : ModContent.ProjectileType<PlagueStingerGoliathV2>();
                        int damage = fireRocket ? 72 : 52;
                        if (Main.expertMode)
                            damage = fireRocket ? 50 : 35;

                        Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                if (!Collision.CanHit(new Vector2(vector121.X, vector121.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    num1066 = 0.105f;
                    vector122 = vector121;
                    num1067 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2) - vector122.X;
                    num1068 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2) - vector122.Y;
                    if (NPC.velocity.X < num1067)
                    {
                        NPC.velocity.X = NPC.velocity.X + num1066;
                        if (NPC.velocity.X < 0f && num1067 > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num1066;
                        }
                    }
                    else if (NPC.velocity.X > num1067)
                    {
                        NPC.velocity.X = NPC.velocity.X - num1066;
                        if (NPC.velocity.X > 0f && num1067 < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - num1066;
                        }
                    }
                    if (NPC.velocity.Y < num1068)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num1066;
                        if (NPC.velocity.Y < 0f && num1068 > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num1066;
                        }
                    }
                    else if (NPC.velocity.Y > num1068)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num1066;
                        if (NPC.velocity.Y > 0f && num1068 < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num1066;
                        }
                    }
                }
                else if (num1069 > 100f)
                {
                    NPC.TargetClosest(true);
                    NPC.spriteDirection = NPC.direction;
                    num1069 = num1065 / num1069;
                    if (NPC.velocity.X < num1067)
                    {
                        NPC.velocity.X = NPC.velocity.X + num1066;
                        if (NPC.velocity.X < 0f && num1067 > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num1066 * 2f;
                        }
                    }
                    else if (NPC.velocity.X > num1067)
                    {
                        NPC.velocity.X = NPC.velocity.X - num1066;
                        if (NPC.velocity.X > 0f && num1067 < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - num1066 * 2f;
                        }
                    }
                    if (NPC.velocity.Y < num1068)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num1066;
                        if (NPC.velocity.Y < 0f && num1068 > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num1066 * 2f;
                        }
                    }
                    else if (NPC.velocity.Y > num1068)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num1066;
                        if (NPC.velocity.Y > 0f && num1068 < 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num1066 * 2f;
                        }
                    }
                }
                if (NPC.ai[1] > 600f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.netUpdate = true;
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[NPC.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[NPC.type].Width / 2), (float)(Main.npcTexture[NPC.type].Height / Main.npcFrameCount[NPC.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 7;
            if (NPC.ai[0] != 0f)
                num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || NPC.AnyNPCs(ModContent.NPCType<PlaguebringerShade>()))
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.02f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Pbg"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Pbg2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Pbg3"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Pbg4"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/Pbg5"), 1f);
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.Plague, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void NPCLoot()
        {
            if (!CalamityWorld.revenge)
            {
                int heartAmt = Main.rand.Next(3) + 3;
                for (int i = 0; i < heartAmt; i++)
                    Item.NewItem((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
            }

            DropHelper.DropItemChance(NPC, ItemID.Stinger, Main.expertMode ? 0.5f : 0.25f, 2, 3);
            DropHelper.DropItem(NPC, ModContent.ItemType<PlagueCellCluster>(), 8, 12);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<PlaguedFuelPack>(), 10);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<PlagueCaller>(), 50);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 240, true);
        }
    }
}
