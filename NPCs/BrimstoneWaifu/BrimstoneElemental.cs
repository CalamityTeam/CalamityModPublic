using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.BrimstoneWaifu
{
	[AutoloadBossHead]
	public class BrimstoneElemental : ModNPC
	{
        private int dustTimer = 90;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Elemental");
			Main.npcFrameCount[npc.type] = 12;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 64f;
			npc.damage = 60;
			npc.width = 100;
			npc.height = 150;
			npc.defense = 20;
			npc.lifeMax = CalamityWorld.revenge ? 35708 : 26000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 54050;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.value = Item.buyPrice(0, 10, 0, 0);
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
                npc.buffImmune[k] = true;
                npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[mod.BuffType("MarkedforDeath")] = false;
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
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.netAlways = true;
			npc.HitSound = SoundID.NPCHit23;
			npc.DeathSound = SoundID.NPCDeath39;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/LeftAlone");
            bossBag = mod.ItemType("BrimstoneWaifuBag");
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 210;
				npc.defense = 120;
				npc.lifeMax = 300000;
				npc.value = Item.buyPrice(1, 0, 0, 0);
			}
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 4500000 : 4000000;
            }
        }
		
		public override void AI()
		{
            CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;
            Player player = Main.player[npc.target];
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			bool brimTeleport = (double)npc.life <= (double)npc.lifeMax * 0.2;
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool calamity = modPlayer.ZoneCalamity;
			npc.TargetClosest(true);
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			Vector2 vectorCenter = npc.Center;
			float xDistance = player.Center.X - center.X;
			float yDistance = player.Center.Y - center.Y;
			float totalDistance = (float)Math.Sqrt((double)(xDistance * xDistance + yDistance * yDistance));
			int dustAmt = (npc.ai[0] == 2f) ? 2 : 1;
			int size = (npc.ai[0] == 2f) ? 50 : 35;
			float speed = expertMode ? 5f : 4.5f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                speed = 5.5f;
            }
            for (int num1011 = 0; num1011 < 2; num1011++) 
			{
				if (Main.rand.Next(3) < dustAmt) 
				{
					int dust = Dust.NewDust(npc.Center - new Vector2((float)size), size * 2, size * 2, 235, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 0.2f;
					Main.dust[dust].fadeIn = 1f;
				}
			}
			if (Vector2.Distance(player.Center, vectorCenter) > 5600f)
			{
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
			}
			else if (npc.timeLeft > 1800)
			{
				npc.timeLeft = 1800;
			}
            if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
            {
                speed = 11f;
            }
			else if (!calamity)
			{
				speed = 7f;
			}
			else if ((double)npc.life <= (double)npc.lifeMax * 0.65)
			{
				speed = expertMode ? 6f : 5f;
			}
			if (npc.ai[0] <= 2f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				if (totalDistance < speed)
				{
					npc.velocity.X = xDistance;
					npc.velocity.Y = yDistance;
				}
				else
				{
					totalDistance = speed / totalDistance;
					npc.velocity.X = xDistance * totalDistance;
					npc.velocity.Y = yDistance * totalDistance;
				}
			}
            if (Main.netMode != 1)
            {
                dustTimer--;
                if (dustTimer <= 0 && npc.alpha <= 0)
                {
                    int damage = expertMode ? 20 : 29;
                    Vector2 position = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                    int projectile = Projectile.NewProjectile((int)position.X, (int)position.Y, 0f, 0f, mod.ProjectileType("BrimDust"), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f); //changed
                    Main.projectile[projectile].timeLeft = 90;
                    dustTimer = 90;
                }
            }
            if (npc.ai[0] == 0f) 
			{
				npc.defense = 20;
				npc.chaseable = true;
				if (Main.netMode != 1)
				{
                    npc.localAI[1] += 1f;
                    if (npc.justHit)
					{
						npc.localAI[1] += 1f;
					}
					if (brimTeleport)
					{
						npc.localAI[1] += 1f;
					}
					if (!calamity)
					{
						npc.localAI[1] += 2f;
					}
					if (npc.localAI[1] >= (float)(200 + Main.rand.Next(100)))
					{
						npc.localAI[1] = 0f;
						npc.TargetClosest(true);
						int timer = 0;
						int playerPosX;
						int playerPosY;
						while (true)
						{
							timer++;
							playerPosX = (int)player.Center.X / 16;
							playerPosY = (int)player.Center.Y / 16;
							playerPosX += Main.rand.Next(-50, 51);
							playerPosY += Main.rand.Next(-50, 51);
							if (!WorldGen.SolidTile(playerPosX, playerPosY) && Collision.CanHit(new Vector2((float)(playerPosX * 16), (float)(playerPosY * 16)), 1, 1, player.position, player.width, player.height))
							{
								break;
							}
							if (timer > 100)
							{
								return;
							}
						}
						npc.ai[0] = 1f;
						npc.ai[1] = (float)playerPosX;
						npc.ai[2] = (float)playerPosY;
						npc.netUpdate = true;
						return;
					}
				}
			}
			else if (npc.ai[0] == 1f) 
			{
                npc.damage = 0;
                npc.dontTakeDamage = true;
                npc.defense = 20;
                npc.chaseable = false;
				npc.alpha += (brimTeleport ? 5 : 4);
				if (npc.alpha >= 255)
				{
                    if (Main.netMode != 1 && NPC.CountNPCS(mod.NPCType("Brimling")) < 2 && revenge)
                    {
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("Brimling"), 0, 0f, 0f, 0f, 0f, 255);
                    }
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
				npc.alpha -= (brimTeleport ? 5 : 4);
				if (npc.alpha <= 0)
				{
                    npc.damage = expertMode ? 96 : 60;
                    npc.dontTakeDamage = false;
                    npc.defense = 20;
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
                npc.defense = 20;
                npc.dontTakeDamage = false;
                npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				Vector2 shootFromVectorX = new Vector2(npc.position.X + (float)(npc.width / 2) + (float)(Main.rand.Next(20) * npc.direction), npc.position.Y + (float)npc.height * 0.8f);
				npc.ai[1] += 1f;
				bool shootProjectile = false;
                if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    if (npc.ai[1] % 10f == 9f)
                    {
                        shootProjectile = true;
                    }
                }
				else if (CalamityWorld.bossRushActive)
				{
					if (npc.ai[1] % 15f == 14f)
					{
						shootProjectile = true;
					}
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.1)
				{
					if (npc.ai[1] % 20f == 19f)
					{
						shootProjectile = true;
					}
				}
				else if ((double)npc.life < (double)npc.lifeMax * 0.5)
				{
					if (npc.ai[1] % 25f == 24f)
					{
						shootProjectile = true;
					}
				}
				else if (npc.ai[1] % 30f == 29f)
				{
					shootProjectile = true;
				}
				if (shootProjectile && npc.position.Y + (float)npc.height < player.position.Y && Collision.CanHit(shootFromVectorX, 1, 1, player.position, player.width, player.height))
				{
					if (Main.netMode != 1)
					{
						float projectileSpeed = 7f; //changed from 10
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            projectileSpeed += 4f;
                        }
						if (revenge)
						{
							projectileSpeed += 1f;
						}
						if ((double)npc.life <= (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
						{
							projectileSpeed += 1f; //changed from 3 not a prob
						}
						if ((double)npc.life <= (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
						{
							projectileSpeed += 1f;
						}
						if (!calamity)
						{
							projectileSpeed += 2f;
						}
						float relativeSpeedX = player.position.X + (float)player.width * 0.5f - shootFromVectorX.X + (float)Main.rand.Next(-80, 81);
						float relativeSpeedY = player.position.Y + (float)player.height * 0.5f - shootFromVectorX.Y + (float)Main.rand.Next(-40, 41);
						float totalRelativeSpeed = (float)Math.Sqrt((double)(relativeSpeedX * relativeSpeedX + relativeSpeedY * relativeSpeedY));
						totalRelativeSpeed = projectileSpeed / totalRelativeSpeed;
						relativeSpeedX *= totalRelativeSpeed;
						relativeSpeedY *= totalRelativeSpeed;
						int projectileDamage = expertMode ? 24 : 32; //projectile damage
						int projectileType = mod.ProjectileType("BrimstoneHellfireball"); //projectile type
						int projectileShot = Projectile.NewProjectile(shootFromVectorX.X, shootFromVectorX.Y, relativeSpeedX, relativeSpeedY, projectileType, projectileDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[projectileShot].timeLeft = 240;
					}
				}
                if (npc.position.Y > player.position.Y - 150f) //200
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    }
                    npc.velocity.Y = npc.velocity.Y - 0.1f;
                    if (npc.velocity.Y > 2f)
                    {
                        npc.velocity.Y = 2f;
                    }
                }
                else if (npc.position.Y < player.position.Y - 400f) //500
                {
                    if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y * 0.98f;
                    }
                    npc.velocity.Y = npc.velocity.Y + 0.1f;
                    if (npc.velocity.Y < -2f)
                    {
                        npc.velocity.Y = -2f;
                    }
                }
                if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 100f)
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    }
                    npc.velocity.X = npc.velocity.X - 0.1f;
                    if (npc.velocity.X > 8f)
                    {
                        npc.velocity.X = 8f;
                    }
                }
                if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 100f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X * 0.98f;
                    }
                    npc.velocity.X = npc.velocity.X + 0.1f;
                    if (npc.velocity.X < -8f)
                    {
                        npc.velocity.X = -8f;
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
				npc.defense = 99999;
                npc.dontTakeDamage = false;
                npc.chaseable = false;
				if (Main.netMode != 1)
				{
					npc.localAI[0] += (float)Main.rand.Next(4);
                    if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                    {
                        npc.localAI[0] += 3f;
                    }
                    if (CalamityWorld.death || !calamity)
                    {
                        npc.localAI[0] += 2f;
                    }
					if (npc.localAI[0] >= 140f)
					{
						npc.localAI[0] = 0f;
						npc.TargetClosest(true);
						float projectileSpeed = revenge ? 8f : 6f;
						Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
						float num180 = player.position.X + (float)player.width * 0.5f - shootFromVector.X;
						float num181 = Math.Abs(num180) * 0.1f;
						float num182 = player.position.Y + (float)player.height * 0.5f - shootFromVector.Y - num181;
						float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
						npc.netUpdate = true;
						num183 = projectileSpeed / num183;
						num180 *= num183;
						num182 *= num183;
						int num184 = expertMode ? 22 : 30;
						int num185 = mod.ProjectileType("BrimstoneHellblast");
						shootFromVector.X += num180;
						shootFromVector.Y += num182;
						for (int num186 = 0; num186 < 6; num186++)
						{
							num180 = player.position.X + (float)player.width * 0.5f - shootFromVector.X;
							num182 = player.position.Y + (float)player.height * 0.5f - shootFromVector.Y;
							num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = projectileSpeed / num183;
							num180 += (float)Main.rand.Next(-80, 81);
							num182 += (float)Main.rand.Next(-80, 81);
							num180 *= num183;
							num182 *= num183;
							int projectile = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, num180, num182, num185, num184 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[projectile].timeLeft = 300;
							Main.projectile[projectile].tileCollide = false;
						}
						float spread = 45f * 0.0174f;
					   	double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y)- spread / 2;
					   	double deltaAngle = spread / 8f;
					   	double offsetAngle;
					   	int damage = expertMode ? 22 : 30;
					   	int i;
					   	for (i = 0; i < 6; i++ )
					   	{
					   		offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					       	int projectile = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)( Math.Sin(offsetAngle) * 6f ), (float)( Math.Cos(offsetAngle) * 6f ), mod.ProjectileType("BrimstoneBarrage"), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					       	int projectile2 = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)( -Math.Sin(offsetAngle) * 6f ), (float)( -Math.Cos(offsetAngle) * 6f ), mod.ProjectileType("BrimstoneBarrage"), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					   	}
					}
		       	}
				npc.TargetClosest(true);
				npc.ai[1] += 1f;
				npc.velocity *= 0.95f;
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				if (npc.ai[1] > 300f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					return;
				}
			}
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 300, true);
			}
		}
		
		public override void FindFrame(int frameHeight) //9 total frames
		{
			npc.frameCounter += 1.0;
			if (npc.ai[0] <= 2f)
			{
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y >= frameHeight * 4)
				{
					npc.frame.Y = 0;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y < frameHeight * 4)
				{
					npc.frame.Y = frameHeight * 4;
				}
				if (npc.frame.Y >= frameHeight * 8)
				{
					npc.frame.Y = frameHeight * 4;
				}
			}
			else
			{
				if (npc.frameCounter > 12.0)
				{
					npc.frame.Y = npc.frame.Y + frameHeight;
					npc.frameCounter = 0.0;
				}
				if (npc.frame.Y < frameHeight * 8)
				{
					npc.frame.Y = frameHeight * 8;
				}
				if (npc.frame.Y >= frameHeight * 12)
				{
					npc.frame.Y = frameHeight * 8;
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}
		
		public override void NPCLoot()
		{
			if (CalamityWorld.downedProvidence)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Bloodstone"), Main.rand.Next(20, 31));
			}
			if (Main.rand.Next(10) == 0)
			{
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("RoseStone"), 1, true);
			}
            if (Main.rand.Next(10) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrimstoneElementalTrophy"));
            }
            if (CalamityWorld.armageddon)
            {
                for (int i = 0; i < 10; i++)
                {
                    npc.DropBossBags();
                }
            }
            if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofFright, Main.rand.Next(20, 41));
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"), Main.rand.Next(2, 4));
				int choice = Main.rand.Next(3);
				if (choice == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Abaddon"));
				}
				else if (choice == 1)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Brimlance"));
				}
				else
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SeethingDischarge"));
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default(Color), 1f);
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
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 60; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
				float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore1"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore3"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/BrimstoneGore4"), 1f);
			}
		}
	}
}