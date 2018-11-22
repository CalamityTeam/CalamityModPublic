using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.NPCs;

namespace CalamityMod.NPCs.CosmicWraith
{
	[AutoloadBossHead]
	public class CosmicWraith : ModNPC
	{
        private const int CosmicProjectiles = 3;
        private const float CosmicAngleSpread = 170;
        private int CosmicCountdown = 0;
        private float phaseSwitch = 0f;
        private float chargeSwitch = 0f;
        private int dustTimer = 3;
        private float spawnX = 750f;
        private float spawnY = 120f;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Signus, Envoy of the Devourer");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 32f;
			npc.damage = 150;
			npc.width = 130;
			npc.height = 130;
			npc.defense = 70;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            npc.lifeMax = CalamityWorld.revenge ? 229500 : 140000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 209250;
            }
            if (CalamityGlobalNPC.DoGSecondStageCountdown <= 0)
            {
                music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Signus");
                npc.lifeMax = CalamityWorld.revenge ? 445500 : 280000;
                if (CalamityWorld.death)
                {
                    npc.lifeMax = 722250;
                }
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 4400000 : 3900000;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.value = Item.buyPrice(0, 15, 0, 0);
			npc.boss = true;
            for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[mod.BuffType("ExoFreeze")] = false;
                npc.buffImmune[mod.BuffType("AbyssalFlames")] = false;
                npc.buffImmune[mod.BuffType("ArmorCrunch")] = false;
                npc.buffImmune[mod.BuffType("DemonFlames")] = false;
                npc.buffImmune[mod.BuffType("GodSlayerInferno")] = false;
                npc.buffImmune[mod.BuffType("Nightwither")] = false;
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.netAlways = true;
			npc.HitSound = SoundID.NPCHit49;
			npc.DeathSound = SoundID.NPCDeath51;
		}
		
		public override void AI()
		{
			bool cosmicDust = (double)npc.life <= (double)npc.lifeMax * 0.85;
			bool speedBoost = (double)npc.life <= (double)npc.lifeMax * 0.75;
			bool cosmicRain = (double)npc.life <= (double)npc.lifeMax * 0.65;
			bool cosmicSpeed = (double)npc.life <= (double)npc.lifeMax * 0.5;
			bool cosmicTeleport = (double)npc.life <= (double)npc.lifeMax * 0.33;
            Player player = Main.player[npc.target];
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
            npc.TargetClosest(true);
			Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vectorCenter = npc.Center;
			float num1243 = player.Center.X - vector142.X;
			float num1244 = player.Center.Y - vector142.Y;
			float num1245 = (float)Math.Sqrt((double)(num1243 * num1243 + num1244 * num1244));
			float num998 = 8f;
			float scaleFactor3 = 300f;
			float num999 = 800f;
			float num1000 = cosmicSpeed ? 12f : 15f; //should be lower
			float num1001 = 5f;
			float scaleFactor4 = 0.75f; //should be 0.75
			int num1002 = 0; //should be 0
			float scaleFactor5 = 10f;
			float num1003 = 30f;
			float num1004 = 150f;
			float num1005 = cosmicSpeed ? 12f : 15f; //should be lower
			float num1006 = 0.333333343f;
			float num1007 = 10f; //yes
			num1006 *= num1005;
            for (int num1011 = 0; num1011 < 2; num1011++)
            {
                if (Main.rand.Next(3) < 1)
                {
                    int num1012 = Dust.NewDust(npc.Center - new Vector2(70f), 70 * 2, 70 * 2, 173, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
			if (Vector2.Distance(player.Center, vectorCenter) > 6400f)
			{
                CalamityGlobalNPC.DoGSecondStageCountdown = 0;
                if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}
			else if (npc.timeLeft < 1800)
			{
				npc.timeLeft = 1800;
			}
			if (cosmicRain && CosmicCountdown == 0)
			{
				CosmicCountdown = 300;
			}
			if (CosmicCountdown > 0)
			{
				CosmicCountdown--;
				if (CosmicCountdown == 0)
				{
                    if (Main.netMode != 1)
                    {
                        int speed2 = revenge ? 13 : 12;
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            speed2 += 3;
                        }
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity = baseVelocity * speed2;
                        int damage = expertMode ? 49 : 62; //360 300
                        for (int i = 0; i < CosmicProjectiles; i++)
                        {
                            Vector2 spawn2 = baseSpawn;
                            spawn2.X = spawn2.X + i * 30 - (CosmicProjectiles * 15);
                            Vector2 velocity = baseVelocity;
                            velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-CosmicAngleSpread / 2 + (CosmicAngleSpread * i / (float)CosmicProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int projectile = Projectile.NewProjectile(spawn2.X, spawn2.Y, velocity.X, velocity.Y, mod.ProjectileType("CosmicFlameBurst"), damage, 10f, Main.myPlayer, 0f, 0f);
                            Main.projectile[projectile].tileCollide = false;
                        }
                    }
				}
			}
			float speed = expertMode ? 6f : 5.5f;
			if (speedBoost)
			{
				speed = expertMode ? 7.5f : 7f;
			}
            if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
            {
                speed += 3f;
            }
			if (npc.ai[0] <= 2f)
			{
                npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				if (num1245 < speed)
				{
					npc.velocity.X = num1243;
					npc.velocity.Y = num1244;
				}
				else
				{
					num1245 = speed / num1245;
					npc.velocity.X = num1243 * num1245;
					npc.velocity.Y = num1244 * num1245;
				}
			}
			if (npc.ai[0] == 0f) 
			{
                npc.chaseable = true;
                if (Main.netMode != 1)
				{
					npc.localAI[1] += 1f;
                    if (npc.localAI[1] >= (float)(200 + Main.rand.Next(200)))
					{
						npc.localAI[1] = 0f;
						npc.TargetClosest(true);
						int num1249 = 0;
						int num1250;
						int num1251;
						while (true)
						{
							num1249++;
							num1250 = (int)player.Center.X / 16;
							num1251 = (int)player.Center.Y / 16;
							num1250 += Main.rand.Next(-40, 41);
							num1251 += Main.rand.Next(-40, 41);
							if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height))
							{
								break;
							}
							if (num1249 > 100)
							{
								return;
							}
						}
						npc.ai[0] = 1f;
						npc.ai[1] = (float)num1250;
						npc.ai[2] = (float)num1251;
						npc.netUpdate = true;
						return;
					}
				}
			}
			else if (npc.ai[0] == 1f) 
			{
                npc.damage = 0;
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                npc.alpha += (cosmicTeleport ? 5 : 4);
				if (npc.alpha >= 255)
				{
					npc.alpha = 255;
					npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
					npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
					npc.ai[0] = 2f;
                    npc.netUpdate = true;
                    return;
				}
			}
			else if (npc.ai[0] == 2f) 
			{
				npc.alpha -= (cosmicTeleport ? 5 : 4);
				if (npc.alpha <= 0)
				{
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
                    if (Main.netMode != 1 && revenge)
                    {
                        int num660 = NPC.NewNPC((int)(Main.player[npc.target].position.X + 750f), (int)(Main.player[npc.target].position.Y), mod.NPCType("SignusBomb"), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                        }
                        int num661 = NPC.NewNPC((int)(Main.player[npc.target].position.X - 750f), (int)(Main.player[npc.target].position.Y), mod.NPCType("SignusBomb"), 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(23, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                        }
                        for (int num621 = 0; num621 < 5; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num622].velocity *= 3f;
                            Main.dust[num622].noGravity = true;
                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[num622].scale = 0.5f;
                                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                            int num623 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num623].velocity *= 3f;
                            Main.dust[num623].noGravity = true;
                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[num623].scale = 0.5f;
                                Main.dust[num623].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int num623 = 0; num623 < 20; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 5f;
                            num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num624].velocity *= 2f;
                            int num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
                            Main.dust[num625].noGravity = true;
                            Main.dust[num625].velocity *= 5f;
                            num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
                            Main.dust[num625].velocity *= 2f;
                        }
                    }
                    npc.damage = expertMode ? 240 : 150;
                    npc.dontTakeDamage = false;
                    npc.chaseable = true;
                    npc.ai[3] += 1f;
					npc.alpha = 0;
					if (npc.ai[3] >= 2f) 
					{
						npc.ai[0] = 3f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
                    } 
					else
					{
						npc.ai[0] = 0f;
					}
                    npc.netUpdate = true;
                    return;
				}
			}
			else if (npc.ai[0] == 3f) 
			{
                npc.dontTakeDamage = false;
                npc.chaseable = true;
                npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				float num1065 = 6f; //changed from 6 to 7.5 modifies speed while firing projectiles
				float num1066 = 0.2f; //changed from 0.075 to 0.09375 modifies speed while firing projectiles
                float num10662 = 0.075f;
				Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				Vector2 vector122 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num1067 = player.position.X + (float)(player.width / 2) - vector122.X;
				float num1068 = player.position.Y + (float)(player.height / 2) - 300f - vector122.Y;
				float num1069 = (float)Math.Sqrt((double)(num1067 * num1067 + num1068 * num1068));
				npc.ai[1] += 1f;
				bool flag104 = false;
				if (npc.life < npc.lifeMax / 3 || CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					if (npc.ai[1] % 10f == 9f)
					{
						flag104 = true;
					}
				}
				else if (npc.life < npc.lifeMax / 2)
				{
					if (npc.ai[1] % 15f == 14f)
					{
						flag104 = true;
					}
				}
				else if (npc.ai[1] % 20f == 19f)
				{
					flag104 = true;
				}
				if (flag104 && npc.position.Y + (float)npc.height < player.position.Y && Collision.CanHit(vector121, 1, 1, player.position, player.width, player.height))
				{
					if (Main.netMode != 1)
					{
						float num1070 = 15f; //changed from 10
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            num1070 += 3f;
                        }
						if (cosmicRain)
						{
							num1070 += 2f; //changed from 3 not a prob
						}
						if (cosmicSpeed)
						{
							num1070 += 2f;
						}
						if (revenge)
						{
							num1070 += 1f;
						}
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num1070 += 1f;
                        }
                        float num1071 = player.position.X + (float)player.width * 0.5f - vector121.X + (float)Main.rand.Next(-80, 81);
						float num1072 = player.position.Y + (float)player.height * 0.5f - vector121.Y + (float)Main.rand.Next(-40, 41);
						float num1073 = (float)Math.Sqrt((double)(num1071 * num1071 + num1072 * num1072));
						num1073 = num1070 / num1073;
						num1071 *= num1073;
						num1072 *= num1073;
						int num1074 = expertMode ? 49 : 62; //projectile damage
						int num1075 = mod.ProjectileType("CosmicFlameBurst"); //projectile type
						int num1076 = Projectile.NewProjectile(vector121.X, vector121.Y, num1071, num1072, num1075, num1074, 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[num1076].timeLeft = 240;
					}
				}
				if (!Collision.CanHit(new Vector2(vector121.X, vector121.Y - 30f), 1, 1, player.position, player.width, player.height))
				{
					num1065 = 14f; //changed from 14 not a prob
					num1066 = 0.25f; //changed from 0.1 not a prob
                    num10662 = 0.1f;
					vector122 = vector121;
					num1067 = player.position.X + (float)(player.width / 2) - vector122.X;
					num1068 = player.position.Y + (float)(player.height / 2) - vector122.Y;
					num1069 = (float)Math.Sqrt((double)(num1067 * num1067 + num1068 * num1068));
					num1069 = num1065 / num1069;
					if (npc.velocity.X < num1067)
					{
						npc.velocity.X = npc.velocity.X + num1066;
						if (npc.velocity.X < 0f && num1067 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num1066;
						}
					}
					else if (npc.velocity.X > num1067)
					{
						npc.velocity.X = npc.velocity.X - num1066;
						if (npc.velocity.X > 0f && num1067 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num1066;
						}
					}
					if (npc.velocity.Y < num1068)
					{
						npc.velocity.Y = npc.velocity.Y + num10662;
						if (npc.velocity.Y < 0f && num1068 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num10662;
						}
					}
					else if (npc.velocity.Y > num1068)
					{
						npc.velocity.Y = npc.velocity.Y - num10662;
						if (npc.velocity.Y > 0f && num1068 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num10662;
						}
					}
				}
				else if (num1069 > 100f)
				{
					npc.TargetClosest(true);
					npc.spriteDirection = npc.direction;
					num1069 = num1065 / num1069;
					if (npc.velocity.X < num1067)
					{
						npc.velocity.X = npc.velocity.X + num1066;
						if (npc.velocity.X < 0f && num1067 > 0f)
						{
							npc.velocity.X = npc.velocity.X + num1066 * 2f;
						}
					}
					else if (npc.velocity.X > num1067)
					{
						npc.velocity.X = npc.velocity.X - num1066;
						if (npc.velocity.X > 0f && num1067 < 0f)
						{
							npc.velocity.X = npc.velocity.X - num1066 * 2f;
						}
					}
					if (npc.velocity.Y < num1068)
					{
						npc.velocity.Y = npc.velocity.Y + num10662;
						if (npc.velocity.Y < 0f && num1068 > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num10662 * 2f;
						}
					}
					else if (npc.velocity.Y > num1068)
					{
						npc.velocity.Y = npc.velocity.Y - num10662;
						if (npc.velocity.Y > 0f && num1068 < 0f)
						{
							npc.velocity.Y = npc.velocity.Y - num10662 * 2f;
						}
					}
				}
				if (npc.ai[1] > 300f)
				{
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[0] == 4f) 
			{
                npc.dontTakeDamage = false;
                npc.chaseable = true;
                if (Main.netMode != 1)
                {
                    if (NPC.CountNPCS(mod.NPCType("CosmicLantern")) < 5)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            int num660 = NPC.NewNPC((int)(Main.player[npc.target].position.X + spawnX), (int)(Main.player[npc.target].position.Y + spawnY), mod.NPCType("CosmicLantern"), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                            }
                            int num661 = NPC.NewNPC((int)(Main.player[npc.target].position.X - spawnX), (int)(Main.player[npc.target].position.Y + spawnY), mod.NPCType("CosmicLantern"), 0, 0f, 0f, 0f, 0f, 255);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(23, -1, -1, null, num661, 0f, 0f, 0f, 0, 0, 0);
                            }
                            spawnY -= 60f;
                        }
                        spawnY = 120f;
                        npc.netUpdate = true;
                    }
                }
                npc.TargetClosest(false);
				npc.rotation = npc.velocity.ToRotation();
				if (Math.Sign(npc.velocity.X) != 0)
				{
					npc.spriteDirection = -Math.Sign(npc.velocity.X);
				}
				if (npc.rotation < -1.57079637f)
				{
					npc.rotation += 3.14159274f;
				}
				if (npc.rotation > 1.57079637f)
				{
					npc.rotation -= 3.14159274f;
				}
				npc.spriteDirection = Math.Sign(npc.velocity.X);
				phaseSwitch += 1f;
                if (chargeSwitch == 0f)
				{
					float scaleFactor6 = num998;
					Vector2 center4 = npc.Center;
					Vector2 center5 = player.Center;
					Vector2 vector126 = center5 - center4;
					Vector2 vector127 = vector126 - Vector2.UnitY * scaleFactor3;
					float num1013 = vector126.Length();
					vector126 = Vector2.Normalize(vector126) * scaleFactor6;
					vector127 = Vector2.Normalize(vector127) * scaleFactor6;
					bool flag64 = Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1);
					if (npc.ai[3] >= 120f)
					{
						flag64 = true;
					}
					float num1014 = 8f;
					flag64 = (flag64 && vector126.ToRotation() > 3.14159274f / num1014 && vector126.ToRotation() < 3.14159274f - 3.14159274f / num1014);
					if (num1013 > num999 || !flag64)
					{
						npc.velocity.X = (npc.velocity.X * (num1000 - 1f) + vector127.X) / (cosmicSpeed ? 11.75f : 14.8f); //num1000 12f 15f
						npc.velocity.Y = (npc.velocity.Y * (num1000 - 1f) + vector127.Y) / (cosmicSpeed ? 11.75f : 14.8f); //num1000 12f 15f
						if (!flag64) 
						{
							npc.ai[3] += 1f;
							if (npc.ai[3] == 120f) 
							{
								npc.netUpdate = true;
							}
						} 
						else
						{
							npc.ai[3] = 0f;
						}
					} 
					else 
					{
						chargeSwitch = 1f;
						npc.ai[2] = vector126.X;
						npc.ai[3] = vector126.Y;
						npc.netUpdate = true;
					}
				} 
				else if (chargeSwitch == 1f) 
				{
					npc.velocity *= scaleFactor4;
					npc.ai[1] += 1f;
					if (npc.ai[1] >= num1001) 
					{
						chargeSwitch = 2f;
						npc.ai[1] = 0f;
						npc.netUpdate = true;
						Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) + new Vector2((float)Main.rand.Next(-num1002, num1002 + 1), (float)Main.rand.Next(-num1002, num1002 + 1)) * 0.04f;
						velocity.Normalize();
						velocity *= scaleFactor5;
						npc.velocity = velocity;
					}
				} 
				else if (chargeSwitch == 2f) 
				{
					if (Main.netMode != 1)
					{
						dustTimer--;
						if (cosmicDust && dustTimer <= 0)
						{
							Main.PlaySound(SoundID.Item73, npc.position);
							int damage = expertMode ? 49 : 62;
							Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
							int projectile = Projectile.NewProjectile((int)vector173.X, (int)vector173.Y, (float)(npc.direction * 2), 4f, mod.ProjectileType("EssenceDust"), damage, 0f, Main.myPlayer, 0f, 0f); //changed
							Main.projectile[projectile].timeLeft = 60;
							Main.projectile[projectile].velocity.X = 0f;
					        Main.projectile[projectile].velocity.Y = 0f;
				    	    dustTimer = 3;
						}
					}
					float num1016 = num1003;
					npc.ai[1] += 1f;
					bool flag65 = Vector2.Distance(npc.Center, player.Center) > num1004 && npc.Center.Y > player.Center.Y;
					if ((npc.ai[1] >= num1016 && flag65) || npc.velocity.Length() < num1007)
					{
						chargeSwitch = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.velocity /= 2f;
						npc.netUpdate = true;
						npc.ai[1] = 45f;
						chargeSwitch = 4f;
					} 
					else 
					{
						Vector2 center6 = npc.Center;
						Vector2 center7 = player.Center;
						Vector2 vec2 = center7 - center6;
						vec2.Normalize();
						if (vec2.HasNaNs()) 
						{
							vec2 = new Vector2((float)npc.direction, 0f);
						}
						npc.velocity = (npc.velocity * (num1005 - 1f) + vec2 * (npc.velocity.Length() + num1006)) / (cosmicSpeed ? 11.65f : 14.75f); //num1005 12f 15f
					}
				} 
				else if (chargeSwitch == 4f) 
				{
					npc.ai[1] -= 3f;
					if (npc.ai[1] <= 0f) 
					{
						chargeSwitch = 0f;
						npc.ai[1] = 0f;
						npc.netUpdate = true;
					}
					npc.velocity *= 0.95f;
				}
				if (phaseSwitch > 300f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					chargeSwitch = 0f;
					phaseSwitch = 0f;
					npc.netUpdate = true;
					return;
				}
			}
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[npc.target];
            if (player.vortexStealthActive && projectile.ranged)
            {
                damage /= 2;
                crit = false;
            }
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 8;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.ai[0] == 4f)
            {
                if (npc.frameCounter > 72.0) //12
                {
                    npc.frameCounter = 0.0;
                }
            }
            else
            {
                int frameY = 196;
                if (npc.frameCounter > 72.0)
                {
                    npc.frameCounter = 0.0;
                }
                npc.frame.Y = frameY * (int)(npc.frameCounter / 12.0); //1 to 6
                if (npc.frame.Y >= frameHeight * 6)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D NPCTexture = Main.npcTexture[npc.type];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color lightColor = (drawColor != null ? (Color)drawColor : CalamityMod.GetNPCColor(((NPC)npc), npc.Center, false));
            int frameCount = Main.npcFrameCount[npc.type];
            float scale = npc.scale;
            float rotation = npc.rotation;
            float offsetY = npc.gfxOffY;
            if (npc.ai[0] == 4f)
            {
                NPCTexture = mod.GetTexture("NPCs/CosmicWraith/CosmicWraithAlt2");
                int height = 564;
                int width = 176;
                Vector2 vector = new Vector2((float)(width / 2), (float)(height / frameCount / 2));
                Microsoft.Xna.Framework.Rectangle frame = new Rectangle(0, 0, width, height / frameCount);
                frame.Y = 94 * (int)(npc.frameCounter / 12.0); //1 to 6
                if (frame.Y >= 94 * 6)
                {
                    frame.Y = 0;
                }
                Main.spriteBatch.Draw(NPCTexture,
                    new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)width * scale / 2f + vector.X * scale,
                    npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)height * scale / (float)frameCount + 4f + vector.Y * scale + 0f + offsetY),
                    new Microsoft.Xna.Framework.Rectangle?(frame),
                    npc.GetAlpha(lightColor),
                    rotation,
                    vector,
                    scale,
                    spriteEffects,
                    0f);
                return false;
            }
            else if (npc.ai[0] == 3f)
            {
                NPCTexture = mod.GetTexture("NPCs/CosmicWraith/CosmicWraithAlt");
            }
            else
            {
                NPCTexture = Main.npcTexture[npc.type];
            }
            Microsoft.Xna.Framework.Rectangle frame2 = npc.frame;
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / frameCount / 2));
            Microsoft.Xna.Framework.Color color9 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Main.spriteBatch.Draw(NPCTexture,
                new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * scale / 2f + vector11.X * scale,
                npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * scale / (float)frameCount + 4f + vector11.Y * scale + 0f + offsetY),
                new Microsoft.Xna.Framework.Rectangle?(frame2),
                npc.GetAlpha(lightColor),
                rotation,
                vector11,
                scale,
                spriteEffects,
                0f);
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
		}
		
		public override void NPCLoot()
		{
            npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("TwistingNether"), Main.rand.Next(2, 4), true);
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SignusTrophy"));
			}
			if (Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CosmicKunai"));
			}
			if (Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Cosmilamp"));
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 200;
				npc.height = 150;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 60; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Signus5"), 1f);
            }
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 600, true);
			}
		}
	}
}