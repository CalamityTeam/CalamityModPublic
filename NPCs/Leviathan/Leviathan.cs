using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Leviathan
{
	[AutoloadBossHead]
	public class Leviathan : ModNPC
	{		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Leviathan");
			Main.npcFrameCount[npc.type] = 3;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 20f;
			npc.damage = 75;
			npc.width = 850;
			npc.height = 450;
			npc.defense = 40;
			npc.lifeMax = CalamityWorld.revenge ? 78900 : 60000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 165000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 7600000 : 6700000;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.value = Item.buyPrice(0, 30, 0, 0);
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
                npc.buffImmune[k] = true;
                npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[BuffID.Daybreak] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
                npc.buffImmune[mod.BuffType("HolyLight")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Plague")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.HitSound = SoundID.NPCHit56;
			npc.DeathSound = SoundID.NPCDeath60;
			npc.noTileCollide = true;
			npc.noGravity = true;
			npc.boss = true;
			npc.netAlways = true;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/LeviathanAndSiren");
			bossBag = mod.ItemType("LeviathanBag");
		}
		
		public override void AI()
		{
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Vector2 vector = npc.Center;
			Player player = Main.player[npc.target];
			bool playerWet = player.wet;
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			int npcType = mod.NPCType("Siren");
			bool sirenAlive = false;
			if (NPC.CountNPCS(npcType) > 0)
			{
				sirenAlive = true;
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
			if (Main.rand.Next(600) == 0)
			{
				Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, 
                    ((sirenAlive && !CalamityWorld.death && !CalamityWorld.bossRushActive) ? soundChoice : soundChoiceRage));
			}
			bool flag6 = player.position.Y < 800f || (double)player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (float)(Main.maxTilesX * 16 - 6400));
			if (flag6 || !sirenAlive || CalamityWorld.death)
			{
				npc.defense = npc.defDefense * 2;
			}
            else
            {
                npc.defense = 40;
            }
			npc.dontTakeDamage = flag6 && !CalamityWorld.bossRushActive;
			int num1038 = 0;
			for (int num1039 = 0; num1039 < 255; num1039++)
			{
				if (Main.player[num1039].active && !Main.player[num1039].dead && (npc.Center - Main.player[num1039].Center).Length() < 1000f)
				{
					num1038++;
				}
			}
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
			}
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
                            if (Main.npc[x].type == mod.NPCType("Siren"))
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
					float num412 = sirenAlive ? 4f : 9f;
					float num413 = sirenAlive ? 0.2f : 0.35f;
                    if (CalamityWorld.bossRushActive)
                    {
                        num412 = 12f;
                        num413 = 0.4f;
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
							{
								npc.ai[2] += 2f;
							}
							else
							{
								if (!playerWet)
								{
									npc.ai[2] += 0.5f;
								}
								if (Siren.phase2)
								{
									npc.ai[2] += 0.5f;
								}
								if (Siren.phase3)
								{
									npc.ai[2] += 0.5f;
								}
							}
						}
						if (npc.ai[2] >= 90f) 
						{
							npc.ai[2] = 0f;
							vector40 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num415 = player.position.X + (float)(player.width / 2) - vector40.X;
							num416 = player.position.Y + (float)(player.height / 2) - vector40.Y;
							if (Main.netMode != 1) 
							{
								float num418 = sirenAlive ? 13.5f : 16f;
								int num419 = playerWet ? 40 : 48;
								int num420 = mod.ProjectileType("LeviathanBomb");
								if (expertMode)
								{
									num418 = sirenAlive ? 14f : 17f;
									num419 = playerWet ? 29 : 33;
								}
                                if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                                {
                                    num418 = 22f;
                                }
								num417 = (float)Math.Sqrt((double)(num415 * num415 + num416 * num416));
								num417 = num418 / num417;
								num415 *= num417;
								num416 *= num417;
								num415 += (float)Main.rand.Next(-5, 6) * 0.05f;
								num416 += (float)Main.rand.Next(-5, 6) * 0.05f;
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
					npc.ai[1] += 1f;
					npc.ai[1] += (float)(num1038 / 2);
					if (revenge)
					{
						npc.ai[1] += 1f;
					}
					if (!sirenAlive || CalamityWorld.death || CalamityWorld.bossRushActive)
					{
						npc.ai[1] += 2f;
					}
					else
					{
						if (Siren.phase2 || CalamityWorld.bossRushActive)
						{
							npc.ai[1] += 0.5f;
						}
						if (Siren.phase3 || CalamityWorld.bossRushActive)
						{
							npc.ai[1] += 0.5f;
						}
					}
					bool flag103 = false;
					int spawnLimit = sirenAlive ? 2 : 4;
					int spawnLimit2 = sirenAlive ? 5 : 10;
					if (npc.ai[1] > 80f) //changed from 40 not a prob
					{
						npc.ai[1] = 0f;
						npc.ai[2] += 1f;
						flag103 = true;
					}
					if (Collision.CanHit(vector119, 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && flag103)
					{
						Main.PlaySound(29, (int)npc.position.X, (int)npc.position.Y, soundChoice);
						if (Main.netMode != 1 && NPC.CountNPCS(mod.NPCType("Parasea")) < spawnLimit2 && NPC.CountNPCS(mod.NPCType("AquaticAberration")) < spawnLimit)
						{
							int num1061;
                            int value = CalamityWorld.death ? 2 : 3;
                            if (CalamityWorld.bossRushActive)
                                value++;
							if (Main.rand.Next(value) == 0)
							{
								num1061 = mod.NPCType("AquaticAberration");
							}
							else
							{
								num1061 = mod.NPCType("Parasea");
							}
							int num1062 = NPC.NewNPC((int)vector119.X, (int)vector119.Y, num1061, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num1062].velocity.X = (float)Main.rand.Next(-200, 201) * 0.01f;
							Main.npc[num1062].velocity.Y = (float)Main.rand.Next(-200, 201) * 0.01f;
							Main.npc[num1062].netUpdate = true;
						}
					}
					if (num1060 > 400f || !Collision.CanHit(new Vector2(vector119.X, vector119.Y - 30f), 1, 1, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num1063 = sirenAlive ? 7f : 8f; //changed from 14 not a prob
						float num1064 = sirenAlive ? 0.05f : 0.065f; //changed from 0.1 not a prob
                        if (CalamityWorld.bossRushActive)
                        {
                            num1063 = 10f;
                            num1064 = 0.075f;
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
						npc.ai[0] = ((double)npc.life < (double)npc.lifeMax * 0.5 ? 2f : 0f);
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
						return;
					}
				}
                else if (npc.ai[0] == 2f)
                {
                    Vector2 distFromPlayer = Main.player[npc.target].Center - npc.Center;
                    int num1043 = 1; //2
                    if ((npc.ai[1] > (float)(2 * num1043) && npc.ai[1] % 2f == 0f) || distFromPlayer.Length() > 2400f)
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
                            Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)((float)num24), default(Vector2)) + vector;
                            Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                            int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default(Color), 1.4f);
                            Main.dust[num25].noGravity = true;
                            Main.dust[num25].noLight = true;
                            Main.dust[num25].velocity /= 4f;
                            Main.dust[num25].velocity -= npc.velocity;
                        }
                        npc.TargetClosest(true);
                        if (Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 20f)
                        {
                            npc.localAI[1] = 1f;
                            npc.ai[1] += 1f;
                            npc.ai[2] = 0f;
                            float num1044 = revenge ? 20f : 18f; //16
                            if ((double)npc.life < (double)npc.lifeMax * 0.25)
                            {
                                num1044 += 2f; //2 not a prob
                            }
                            if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                            {
                                num1044 += 2f; //2 not a prob
                            }
                            if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                            {
                                num1044 += 4f;
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
                        npc.localAI[1] = 0f;
                        float num1048 = revenge ? 7.5f : 6.5f; //12 not a prob
                        float num1049 = revenge ? 0.12f : 0.11f; //0.15 not a prob
                        if ((double)npc.life < (double)npc.lifeMax * 0.25)
                        {
                            num1048 += 2f; //2 not a prob
                            num1049 += 0.05f; //0.05 not a prob
                        }
                        if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                        {
                            num1048 += 2f; //2 not a prob
                            num1049 += 0.1f; //0.1 not a prob
                        }
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            num1048 += 3f;
                            num1049 += 0.2f;
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
                        return;
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
                        int num1050 = sirenAlive ? 750 : 600; //600 not a prob
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
                            npc.localAI[1] = 1f;
                            return;
                        }
                        npc.TargetClosest(true);
                        npc.spriteDirection = npc.direction;
                        npc.localAI[1] = 0f;
                        npc.velocity *= 0.9f;
                        float num1052 = revenge ? 0.11f : 0.1f; //0.1
                        if (npc.life < npc.lifeMax / 3 || CalamityWorld.bossRushActive)
                        {
                            npc.velocity *= 0.9f;
                            num1052 += 0.05f; //0.05
                        }
                        if (npc.life < npc.lifeMax / 5 || CalamityWorld.bossRushActive)
                        {
                            npc.velocity *= 0.9f;
                            num1052 += 0.05f; //0.05
                        }
                        if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] += 1f;
                            return;
                        }
                    }
                }
            }
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 2;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default(Color), 2f);
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
		
		public override void NPCLoot()
		{
			bool hardMode = Main.hardMode;
			int bossAlive = mod.NPCType("Siren");
			if (!NPC.AnyNPCs(bossAlive))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EnchantedPearl")); //done
				if (Main.rand.Next(10) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HotlineFishingHook);
				}
				if (Main.rand.Next(10) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BottomlessBucket);
				}
				if (Main.rand.Next(10) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SuperAbsorbantSponge);
				}
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CratePotion, Main.rand.Next(5, 9));
				}
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishingPotion, Main.rand.Next(5, 9));
				}
				if (Main.rand.Next(5) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SonarPotion, Main.rand.Next(5, 9));
				}
				if (!hardMode)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("IOU"), 1, true);
				}
				if (Main.rand.Next(10) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LeviathanTrophy")); //done
				}
                if (CalamityWorld.armageddon)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        npc.DropBossBags();
                    }
                }
                if (Main.expertMode && hardMode)
				{
					npc.DropBossBags();
				}
				else
				{
					if (Main.rand.Next(7) == 0)
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LeviathanMask")); //done
					}
					if (hardMode)
					{
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Atlantis")); //done
						}
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrackishFlask")); //done
						}
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Leviatitan")); //done
						}
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LureofEnthrallment")); //done
						}
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SirensSong")); //done
						}
						if (Main.rand.Next(4) == 0)
						{
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Greentide")); //done
						}
					}
				}
			}
			else
			{
				npc.value = 0f;
				npc.boss = false;
			}
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
            Texture2D texture2 = mod.GetTexture("NPCs/Leviathan/LeviathanTexTwo");
            Texture2D texture3 = mod.GetTexture("NPCs/Leviathan/LeviathanAltTexOne");
            Texture2D texture4 = mod.GetTexture("NPCs/Leviathan/LeviathanAltTexTwo");
            if (npc.ai[0] == 1f)
            {
                if (npc.localAI[0] == 0f)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
                }
                else
                {
                    CalamityMod.DrawTexture(spriteBatch, texture2, 0, npc, drawColor);
                }
            }
            else
            {
                if (npc.localAI[0] == 0f)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture3, 0, npc, drawColor);
                }
                else
                {
                    CalamityMod.DrawTexture(spriteBatch, texture4, 0, npc, drawColor);
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
                npc.localAI[0] += 1f;
			}
            if (npc.localAI[0] > 1f)
            {
                npc.localAI[0] = 0f;
                npc.netUpdate = true;
            }
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}