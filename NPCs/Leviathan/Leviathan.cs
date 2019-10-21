using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Projectiles.Boss;
namespace CalamityMod.NPCs.Leviathan
{
    [AutoloadBossHead]
    public class Leviathan : ModNPC
    {
        private bool altTextureSwap = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Leviathan");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 20f;
            npc.damage = 90;
            npc.width = 850;
            npc.height = 450;
            npc.defense = 40;
            npc.Calamity().RevPlusDR(0.35f);
			npc.LifeMaxNERD(69000, 90700, 189750, 7000000, 7500000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Plague>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.boss = true;
            npc.netAlways = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/LeviathanAndSiren");
            else
                music = MusicID.Boss3;
            bossBag = ModContent.ItemType<LeviathanBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(altTextureSwap);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            altTextureSwap = reader.ReadBoolean();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void AI()
        {
            CalamityGlobalNPC.leviathan = npc.whoAmI;

            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            Vector2 vector = npc.Center;

            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

            bool sirenAlive = false;
            if (CalamityGlobalNPC.siren != -1)
            {
                sirenAlive = Main.npc[CalamityGlobalNPC.siren].active;
            }

            int soundChoice = Main.rand.Next(3);
            int soundChoiceRage = 92;
            if (soundChoice == 0)
            {
                soundChoice = 38;
            }
            else if (soundChoice == 1)
            {
                soundChoice = 39;
            }
            else
            {
                soundChoice = 40;
            }
            if (Main.rand.NextBool(600))
            {
                Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y,
                    (sirenAlive && !CalamityWorld.death && !CalamityWorld.bossRushActive) ? soundChoice : soundChoiceRage);
            }

            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(true);
            }
            Player player = Main.player[npc.target];

            bool flag6 = player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400));
            npc.dontTakeDamage = flag6 && !CalamityWorld.bossRushActive;

            if (npc.timeLeft < 3000)
            {
                npc.timeLeft = 3000;
            }

            if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
                    npc.velocity = new Vector2(0f, 10f);
                    if ((double)npc.position.Y > Main.rockLayer * 16.0)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<Siren>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    return;
                }
            }
            else
            {
                if (npc.ai[0] == 0f)
                {
                    npc.TargetClosest(true);
                    float num412 = sirenAlive ? 3.5f : 7f;
                    float num413 = sirenAlive ? 0.1f : 0.2f;
                    if (CalamityWorld.bossRushActive)
                    {
                        num412 *= 1.5f;
                        num413 *= 1.5f;
                    }

                    int num414 = 1;
                    if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
                    {
                        num414 = -1;
                    }

                    Vector2 vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num415 = player.position.X + (float)(player.width / 2) + (float)(num414 * 800) - vector40.X;
                    float num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
                    float num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                    num417 = num412 / num417;
                    num415 *= num417;
                    num416 *= num417;

                    if (npc.velocity.X < num415)
                    {
                        npc.velocity.X = npc.velocity.X + num413;
                        if (npc.velocity.X < 0f && num415 > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num413;
                        }
                    }
                    else if (npc.velocity.X > num415)
                    {
                        npc.velocity.X = npc.velocity.X - num413;
                        if (npc.velocity.X > 0f && num415 < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num413;
                        }
                    }
                    if (npc.velocity.Y < num416)
                    {
                        npc.velocity.Y = npc.velocity.Y + num413;
                        if (npc.velocity.Y < 0f && num416 > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num413;
                        }
                    }
                    else if (npc.velocity.Y > num416)
                    {
                        npc.velocity.Y = npc.velocity.Y - num413;
                        if (npc.velocity.Y > 0f && num416 < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num413;
                        }
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 120f : 240f))
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.target = 255;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!player.dead)
                        {
                            npc.ai[2] += 1f;
                            if (!sirenAlive)
                                npc.ai[2] += 2f;
                        }

                        if (npc.ai[2] >= 75f)
                        {
                            npc.ai[2] = 0f;
                            vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            num415 = player.position.X + (float)(player.width / 2) - vector40.X;
                            num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float num418 = sirenAlive ? 13.5f : 16f;
                                int num419 = 40;
                                int num420 = ModContent.ProjectileType<LeviathanBomb>();
                                if (expertMode)
                                {
                                    num418 = sirenAlive ? 14f : 17f;
                                    num419 = 33;
                                }
                                if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                                {
                                    num418 = 22f;
                                }
                                if (CalamityWorld.bossRushActive)
                                {
                                    num418 *= 1.5f;
                                }

                                num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
                                num417 = num418 / num417;
                                num415 *= num417;
                                num416 *= num417;
                                vector40.X += num415 * 4f;
                                vector40.Y += num416 * 4f;
                                Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, num420, num419, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
                else if (npc.ai[0] == 1f)
                {
                    npc.TargetClosest(true);

                    Vector2 vector119 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
                    Vector2 vector120 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    float num1058 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector120.X;
                    float num1059 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector120.Y;
                    float num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));

                    npc.ai[1] += revenge ? 2f : 1f;
                    if (!sirenAlive || CalamityWorld.death || CalamityWorld.bossRushActive)
                    {
                        npc.ai[1] += 2.5f;
                    }

                    bool flag103 = false;
                    int spawnLimit = sirenAlive ? 2 : 4;
                    int spawnLimit2 = sirenAlive ? 5 : 10;

                    if (npc.ai[1] > 80f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] += 1f;
                        flag103 = true;
                    }
                    if (flag103)
                    {
                        Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, soundChoice);
                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<Parasea>()) < spawnLimit2 && NPC.CountNPCS(ModContent.NPCType<AquaticAberration>()) < spawnLimit)
                        {
                            int num1061;
                            int value = CalamityWorld.death ? 2 : 3;
                            if (CalamityWorld.bossRushActive)
                                value++;
                            if (Main.rand.Next(value) == 0)
                            {
                                num1061 = ModContent.NPCType<AquaticAberration>();
                            }
                            else
                            {
                                num1061 = ModContent.NPCType<Parasea>();
                            }
                            int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, num1061, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * 0.01f;
                            Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.01f;
                            Main.npc[num1062].netUpdate = true;
                        }
                    }

                    if (num1060 > 600f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        float num1063 = sirenAlive ? 7f : 8f;
                        float num1064 = sirenAlive ? 0.05f : 0.065f;
                        if (CalamityWorld.bossRushActive)
                        {
                            num1063 *= 1.5f;
                            num1064 *= 1.5f;
                        }

                        vector120 = vector119;
                        num1058 = player.position.X + (float)(player.width / 2) - vector120.X;
                        num1059 = player.position.Y + (float)(player.height / 2) - vector120.Y;
                        num1060 = (float)Math.Sqrt((double)(num1058 * num1058 + num1059 * num1059));
                        num1060 = num1063 / num1060;

                        if (npc.velocity.X < num1058)
                        {
                            npc.velocity.X = npc.velocity.X + num1064;
                            if (npc.velocity.X < 0f && num1058 > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num1064;
                            }
                        }
                        else if (npc.velocity.X > num1058)
                        {
                            npc.velocity.X = npc.velocity.X - num1064;
                            if (npc.velocity.X > 0f && num1058 < 0f)
                            {
                                npc.velocity.X = npc.velocity.X - num1064;
                            }
                        }
                        if (npc.velocity.Y < num1059)
                        {
                            npc.velocity.Y = npc.velocity.Y + num1064;
                            if (npc.velocity.Y < 0f && num1059 > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num1064;
                            }
                        }
                        else if (npc.velocity.Y > num1059)
                        {
                            npc.velocity.Y = npc.velocity.Y - num1064;
                            if (npc.velocity.Y > 0f && num1059 < 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y - num1064;
                            }
                        }
                    }
                    else
                    {
                        npc.velocity *= 0.9f;
                    }

                    npc.spriteDirection = npc.direction;

                    if (npc.ai[2] > 3f)
                    {
                        npc.ai[0] = (double)npc.life < (double)npc.lifeMax * 0.5 ? 2f : 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[0] == 2f)
                {
                    Vector2 distFromPlayer = Main.player[npc.target].Center - npc.Center;
                    if (npc.ai[1] > 1f || distFromPlayer.Length() > 2400f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                    if (npc.ai[1] % 2f == 0f)
                    {
                        int num24 = 7;
                        for (int j = 0; j < num24; j++)
                        {
                            Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)(float)num24, default) + vector;
                            Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].noLight = true;
                            Main.dust[num25].velocity /= 4f;
                            Main.dust[num25].velocity -= npc.velocity;
                        }

                        npc.TargetClosest(true);

                        if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
                        {
                            npc.ai[1] += 1f;
                            npc.ai[2] = 0f;
                            float num1044 = revenge ? 20f : 18f;
                            if ((double)npc.life < (double)npc.lifeMax * 0.25 || CalamityWorld.bossRushActive)
                            {
                                num1044 += 2f;
                            }
                            if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                            {
                                num1044 += 2f;
                            }
                            if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                            {
                                num1044 += 4f;
                            }
                            if (CalamityWorld.bossRushActive)
                            {
                                num1044 *= 1.25f;
                            }

                            Vector2 vector117 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num1045 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector117.X;
                            float num1046 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector117.Y;
                            float num1047 = (float)Math.Sqrt((double)(num1045 * num1045 + num1046 * num1046));
                            num1047 = num1044 / num1047;
                            npc.velocity.X = num1045 * num1047;
                            npc.velocity.Y = num1046 * num1047;
                            npc.spriteDirection = npc.direction;
                            Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, soundChoiceRage);
                            return;
                        }

                        float num1048 = revenge ? 7.5f : 6.5f;
                        float num1049 = revenge ? 0.12f : 0.11f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.25 || CalamityWorld.bossRushActive)
                        {
                            num1048 += 2f;
                            num1049 += 0.05f;
                        }
                        if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                        {
                            num1048 += 2f;
                            num1049 += 0.1f;
                        }
                        if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            num1048 += 3f;
                            num1049 += 0.2f;
                        }
                        if (CalamityWorld.bossRushActive)
                        {
                            num1048 *= 1.25f;
                            num1049 *= 1.25f;
                        }

                        if (npc.position.Y + (float)(npc.height / 2) < (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2)))
                        {
                            npc.velocity.Y = npc.velocity.Y + num1049;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num1049;
                        }
                        if (npc.velocity.Y < -12f)
                        {
                            npc.velocity.Y = -num1048;
                        }
                        if (npc.velocity.Y > 12f)
                        {
                            npc.velocity.Y = num1048;
                        }
                        if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > 600f)
                        {
                            npc.velocity.X = npc.velocity.X + 0.15f * (float)npc.direction;
                        }
                        else if (Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 300f)
                        {
                            npc.velocity.X = npc.velocity.X - 0.15f * (float)npc.direction;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X * 0.8f;
                        }
                        if (npc.velocity.X < -16f)
                        {
                            npc.velocity.X = -16f;
                        }
                        if (npc.velocity.X > 16f)
                        {
                            npc.velocity.X = 16f;
                        }
                        npc.spriteDirection = npc.direction;
                    }
                    else
                    {
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }

                        npc.spriteDirection = npc.direction;

                        int num1050 = sirenAlive ? 1100 : 900;
                        int num1051 = 1;
                        if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))
                        {
                            num1051 = -1;
                        }
                        if (npc.direction == num1051 && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) > (float)num1050)
                        {
                            npc.ai[2] = 1f;
                        }
                        if (npc.ai[2] != 1f)
                        {
                            return;
                        }

                        npc.TargetClosest(true);

                        npc.spriteDirection = npc.direction;
                        npc.velocity *= 0.9f;
                        float num1052 = revenge ? 0.11f : 0.1f;
                        if (npc.life < npc.lifeMax / 4 || CalamityWorld.bossRushActive)
                        {
                            npc.velocity *= 0.9f;
                            num1052 += 0.05f;
                        }
                        if (npc.life < npc.lifeMax / 10 || CalamityWorld.bossRushActive)
                        {
                            npc.velocity *= 0.9f;
                            num1052 += 0.05f;
                        }
                        if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] += 1f;
                        }
                    }
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Leviathangib1"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Leviathangib2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Leviathangib3"), 1f);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        // The Leviathan runs the same loot code as Anahita, but only if she dies last.
        public override void NPCLoot()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<Siren>()))
                DropSirenLeviLoot(npc);
        }

        // This loot code is shared with Anahita.
        public static void DropSirenLeviLoot(NPC npc)
        {
            CalamityMod mod = ModContent.GetInstance<CalamityMod>();
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<LeviathanTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeOcean>(), true, !CalamityWorld.downedLeviathan);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeLeviathanandSiren>(), true, !CalamityWorld.downedLeviathan);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedLeviathan, 4, 2, 1);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Weapons
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Greentide>(), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Leviatitan>(), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<SirensSong>(), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Atlantis>(), Main.hardMode, 4, 1, 1);
                DropHelper.DropItemCondition(npc, ModContent.ItemType<BrackishFlask>(), Main.hardMode, 4, 1, 1);

                // Equipment
                DropHelper.DropItemCondition(npc, ModContent.ItemType<LureofEnthrallment>(), Main.hardMode, 4, 1, 1);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<LeviathanMask>(), 7);

                // Fishing
                DropHelper.DropItem(npc, ModContent.ItemType<EnchantedPearl>());
                DropHelper.DropItemChance(npc, ItemID.HotlineFishingHook, 10);
                DropHelper.DropItemChance(npc, ItemID.BottomlessBucket, 10);
                DropHelper.DropItemChance(npc, ItemID.SuperAbsorbantSponge, 10);
                DropHelper.DropItemChance(npc, ItemID.FishingPotion, 5, 5, 8);
                DropHelper.DropItemChance(npc, ItemID.SonarPotion, 5, 5, 8);
                DropHelper.DropItemChance(npc, ItemID.CratePotion, 5, 5, 8);

                // Other
                DropHelper.DropItemCondition(npc, ModContent.ItemType<IOU>(), !Main.hardMode);
            }

            // Mark Siren & Levi as dead
            CalamityWorld.downedLeviathan = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Wet, 240, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = Main.npcTexture[npc.type];
            Texture2D texture2 = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/LeviathanTexTwo");
            Texture2D texture3 = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/LeviathanAltTexOne");
            Texture2D texture4 = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/LeviathanAltTexTwo");
            if (npc.ai[0] == 1f)
            {
                if (!altTextureSwap)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor, true);
                }
                else
                {
                    CalamityMod.DrawTexture(spriteBatch, texture2, 0, npc, drawColor, true);
                }
            }
            else
            {
                if (!altTextureSwap)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture3, 0, npc, drawColor, true);
                }
                else
                {
                    CalamityMod.DrawTexture(spriteBatch, texture4, 0, npc, drawColor, true);
                }
            }
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 0.0;
            }
            if (npc.frame.Y >= frameHeight * 3)
            {
                npc.frame.Y = 0;
                altTextureSwap = !altTextureSwap;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
    }
}
