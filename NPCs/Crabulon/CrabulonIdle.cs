using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Crabulon
{
	[AutoloadBossHead]
	public class CrabulonIdle : ModNPC
	{
        private float shotSpacing = 1000f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crabulon");
			Main.npcFrameCount[npc.type] = 6;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 14f;
			npc.damage = 30;
			npc.width = 164; //324
			npc.height = 154; //216
			npc.defense = 8;
			npc.lifeMax = CalamityWorld.revenge ? 2640 : 1800;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 6930;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 3600000 : 3000000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.noGravity = false;
			npc.noTileCollide = false;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Crabulon");
            npc.boss = true;
            npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 4, 0, 0);
			npc.HitSound = SoundID.NPCHit45;
			npc.DeathSound = SoundID.NPCDeath1;
			bossBag = mod.ItemType("CrabulonBag");
		}
		
		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.5f, 1f);
			Player player = Main.player[npc.target];
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.noTileCollide = true;
					npc.velocity = new Vector2(0f, 10f);
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else
			{
				if (npc.timeLeft < 1800)
				{
					npc.timeLeft = 1800;
				}
			}
            if (npc.ai[0] != 0f && npc.ai[0] < 3f)
            {
                Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num349 = player.position.X + (float)(player.width / 2) - vector34.X;
                float num350 = player.position.Y + (float)(player.height / 2) - vector34.Y;
                float num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
                num349 *= num351;
                num350 *= num351;
                if (Main.netMode != 1)
                {
                    int num352 = 1;
                    npc.localAI[3] += 2f;
                    if (CalamityWorld.bossRushActive)
                    {
                        npc.localAI[3] += 2f;
                        num352 += 3;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[3] += 1f;
                        num352 += 2;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[3] += 2f;
                        num352 += 3;
                    }
                    if (npc.ai[3] == 0f)
                    {
                        if (npc.localAI[3] > 600f)
                        {
                            npc.ai[3] = 1f;
                            npc.localAI[3] = 0f;
                        }
                    }
                    else if (npc.localAI[3] > 45f)
                    {
                        npc.localAI[3] = 0f;
                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= (float)num352)
                        {
                            npc.ai[3] = 0f;
                        }
                        float num353 = 10f;
                        int num354 = expertMode ? 11 : 14;
                        int num355 = mod.ProjectileType("MushBomb");
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 42);
                        if (CalamityWorld.bossRushActive)
                        {
                            num354 += 3;
                            num353 += 3f;
                        }
                        if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
                        {
                            num354++;
                            num353 += 1f;
                        }
                        if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                        {
                            num354++;
                            num353 += 1f;
                        }
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num354 += 3;
                            num353 += 1f;
                        }
                        vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        num349 = player.position.X + (float)player.width * 0.5f - vector34.X;
                        num350 = player.position.Y + (float)player.height * 0.5f - vector34.Y;
                        num351 = (float)Math.Sqrt((double)(num349 * num349 + num350 * num350));
                        num351 = num353 / num351;
                        num349 *= num351;
                        num350 *= num351;
                        vector34.X += num349;
                        vector34.Y += num350;
                        Projectile.NewProjectile(vector34.X, vector34.Y, num349, num350 - 5f, num355, num354, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (npc.ai[0] == 0f)
			{
				if (Main.netMode != 2)
				{
					if (!player.dead && player.active && (player.Center - npc.Center).Length() < 800f)
					{
						player.AddBuff(mod.BuffType("Mushy"), 2);
					}
				}
				int sporeDust = Dust.NewDust(npc.position, npc.width, npc.height, 56, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
				Main.dust[sporeDust].noGravity = true;
				Main.dust[sporeDust].velocity *= 0.5f;
                npc.damage = 0;
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 420f)
				{
					npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
			}
			else if (npc.ai[0] == 1f)
			{
				npc.damage = 0;
				npc.velocity.X *= 0.98f;
				npc.velocity.Y *= 0.98f;
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 60f)
				{
					npc.noGravity = true;
					npc.noTileCollide = true;
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
			}
			else if (npc.ai[0] == 2f)
			{
                int damageBoost = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
                npc.damage = npc.defDamage + damageBoost;
                float num823 = 1.25f;
				bool flag51 = false;
				if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive) 
				{
					num823 = 1.5f;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive) 
				{
					num823 = 2f;
				}
                if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                {
                    num823 = 8f;
                }
				if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 50f) 
				{
					flag51 = true;
				}
				if (flag51) 
				{
					npc.velocity.X = npc.velocity.X * 0.9f;
					if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1) 
					{
						npc.velocity.X = 0f;
					}
				} 
				else 
				{
                    npc.direction = -Main.player[npc.target].direction;
                    if (npc.direction > 0) 
					{
						npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
					}
					if (npc.direction < 0) 
					{
						npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
					}
				}
				int num854 = 80;
				int num855 = 20;
				Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);
				bool flag52 = false;
				if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height - 16f) 
				{
					flag52 = true;
				}
				if (flag52) 
				{
					npc.velocity.Y = npc.velocity.Y + 0.5f;
				} 
				else if (Collision.SolidCollision(position2, num854, num855))
				{
					if (npc.velocity.Y > 0f) 
					{
						npc.velocity.Y = 0f;
					}
					if ((double)npc.velocity.Y > -0.2) 
					{
						npc.velocity.Y = npc.velocity.Y - 0.025f;
					} 
					else
					{
						npc.velocity.Y = npc.velocity.Y - 0.2f;
					}
					if (npc.velocity.Y < -4f) 
					{
						npc.velocity.Y = -4f;
					}
				} 
				else 
				{
					if (npc.velocity.Y < 0f) 
					{
						npc.velocity.Y = 0f;
					}
					if ((double)npc.velocity.Y < 0.1) 
					{
						npc.velocity.Y = npc.velocity.Y + 0.025f;
					} 
					else 
					{
						npc.velocity.Y = npc.velocity.Y + 0.5f;
					}
				}
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 360f)
				{
					npc.noGravity = false;
					npc.noTileCollide = false;
					npc.ai[0] = 3f;
					npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
				if (npc.velocity.Y > 10f) 
				{
					npc.velocity.Y = 10f;
					return;
				}
			}
			else if (npc.ai[0] == 3f)
			{
                int damageBoost = (int)(20f * (1f - (float)npc.life / (float)npc.lifeMax));
                npc.damage = npc.defDamage + damageBoost;
                npc.noTileCollide = false;
				if (npc.velocity.Y == 0f) 
				{
					npc.velocity.X = npc.velocity.X * 0.8f;
					npc.ai[1] += 1f;
					if (npc.ai[1] > 0f)
					{
						if (npc.life < npc.lifeMax / 2 || CalamityWorld.bossRushActive) 
						{
							npc.ai[1] += 4f;
						}
						if (npc.life < npc.lifeMax / 10 || CalamityWorld.bossRushActive) 
						{
							npc.ai[1] += 8f;
						}
					}
					if (npc.ai[1] >= 300f) 
					{
						npc.ai[1] = -20f;
						npc.frameCounter = 0.0;
					} 
					else if (npc.ai[1] == -1f)
					{
						npc.TargetClosest(true);
						npc.velocity.X = (float)(4 * npc.direction);
						npc.velocity.Y = -12.1f;
						npc.ai[0] = 4f;
						npc.ai[1] = 0f;
					}
				}
			}
			else
			{
				if (npc.velocity.Y == 0f) 
				{
					Main.PlaySound(SoundID.Item14, npc.position);
                    if (Main.netMode != 1)
                    {
                        Projectile.NewProjectile((int)npc.Center.X, (int)npc.Center.Y + 20, 0f, 0f, mod.ProjectileType("Mushmash"), 20, 0f, Main.myPlayer, 0f, 0f);
                    }
                    npc.ai[2] += 1f;
					if (npc.ai[2] >= 3f)
					{
                        if (Main.netMode != 1 && revenge)
                        {
                            for (int x = 0; x < 20; x++)
                            {
                                int num354 = expertMode ? 11 : 14;
                                Projectile.NewProjectile(npc.Center.X + shotSpacing, npc.Center.Y - 1000f, 0f, 0f, mod.ProjectileType("MushBombFall"), num354, 0f, Main.myPlayer, 0f, 0f);
                                shotSpacing -= 100f;
                            }
                            shotSpacing = 1000f;
                        }
                        npc.ai[0] = 1f;
						npc.ai[2] = 0f;
					}
					else
					{
						npc.ai[0] = 3f;
					}
					for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20) 
					{
						for (int num623 = 0; num623 < 4; num623++) 
						{
							int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + (float)npc.height), npc.width + 20, 4, 56, 0f, 0f, 100, default(Color), 1.5f);
							Main.dust[num624].velocity *= 0.2f;
						}
					}
				} 
				else 
				{
					npc.TargetClosest(true);
					if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width) 
					{
						npc.velocity.X = npc.velocity.X * 0.9f;
						npc.velocity.Y = npc.velocity.Y + 0.2f; //0.2
					} 
					else 
					{
						if (npc.direction < 0) 
						{
							npc.velocity.X = npc.velocity.X - 0.2f;
						}
						else if (npc.direction > 0) 
						{
							npc.velocity.X = npc.velocity.X + 0.2f;
						}
						float num626 = 2.5f; //4
                        if (revenge)
                        {
                            num626 += 1f;
                        }
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            num626 += 3f;
                        }
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num626 += 2f;
                        }
						if (npc.life < npc.lifeMax / 2 || CalamityWorld.bossRushActive) 
						{
							num626 += 1f;
						}
						if (npc.life < npc.lifeMax / 10 || CalamityWorld.bossRushActive) 
						{
							num626 += 1f;
						}
						if (npc.velocity.X < -num626) 
						{
							npc.velocity.X = -num626;
						}
						if (npc.velocity.X > num626) 
						{
							npc.velocity.X = num626;
						}
					}
				}
			}
			if (npc.localAI[0] == 0f && npc.life > 0)
			{
				npc.localAI[0] = (float)npc.lifeMax;
			}
	       	if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.05);
					if ((float)(npc.life + num660) < npc.localAI[0])
					{
						npc.localAI[0] = (float)npc.life;
						int num661 = (expertMode ? Main.rand.Next(2, 4) : Main.rand.Next(1, 3));
						for (int num662 = 0; num662 < num661; num662++)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
							int num663 = mod.NPCType("CrabShroom");
							int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num664].SetDefaults(num663, -1f);
							Main.npc[num664].velocity.X = (float)Main.rand.Next(-50, 51) * 0.1f;
							Main.npc[num664].velocity.Y = (float)Main.rand.Next(-50, -31) * 0.1f;
							if (Main.netMode == 2 && num664 < 200)
							{
								NetMessage.SendData(23, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
							}
						}
						return;
					}
				}
			}
        }
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			Texture2D texture = mod.GetTexture("NPCs/Crabulon/CrabulonIdleAlt");
			Texture2D textureAttack = mod.GetTexture("NPCs/Crabulon/CrabulonAttack");
			if (npc.ai[0] > 2f)
			{
				CalamityMod.DrawTexture(spriteBatch, textureAttack, 0, npc, drawColor);
			}
			else
			{
				CalamityMod.DrawTexture(spriteBatch, (npc.ai[0] == 2f ? texture : Main.npcTexture[npc.type]), 0, npc, drawColor);
			}
			return false;
		}
		
		public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrabulonTrophy"));
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
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GlowingMushroom, Main.rand.Next(20, 31));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MushroomGrassSeeds, Main.rand.Next(3, 7));
                if (Main.rand.Next(7) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrabulonMask"));
                }
                if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HyphaeRod"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MycelialClaws"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Mycoroot"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Fungicide"));
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
				Dust.NewDust(npc.position, npc.width, npc.height, 56, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 200;
				npc.height = 100;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 56, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon4"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon5"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon6"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/Crabulon7"), 1f);
            }
		}
	}
}