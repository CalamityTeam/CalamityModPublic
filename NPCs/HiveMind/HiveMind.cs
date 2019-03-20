using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.HiveMind
{
	[AutoloadBossHead]
	public class HiveMind : ModNPC
	{
		int burrowTimer = 720;
        int oldDamage = 10;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Hive Mind");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 5f;
			npc.damage = 10;
			npc.width = 150; //324
			npc.height = 120; //216
			npc.defense = 10;
			npc.lifeMax = CalamityWorld.revenge ? 1800 : 1200;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 3300;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 400000 : 350000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.buffImmune[mod.BuffType("GlacialState")] = true;
			npc.buffImmune[mod.BuffType("TemporalSadness")] = true;
			npc.knockBackResist = 0f;
			npc.boss = true;
            npc.value = 0f;
            npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/HiveMind");
            else
                music = MusicID.Boss2;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
			{
				if (npc.timeLeft > 60)
					npc.timeLeft = 60;
				if (npc.localAI[3] < 120f) 
				{
					float[] aiArray = npc.localAI;
					int number = 3;
					float num244 = aiArray[number];
					aiArray[number] = num244 + 1f;
				}
				if (npc.localAI[3] > 60f) 
				{
					npc.velocity.Y = npc.velocity.Y + (npc.localAI[3] - 60f) * 0.5f;
					npc.noGravity = true;
					npc.noTileCollide = true;
					if (burrowTimer > 30)
						burrowTimer = 30;
				}
				return;
			}
			if (npc.localAI[3] > 0f) 
			{
				float[] aiArray = npc.localAI;
				int number = 3;
				float num244 = aiArray[number];
				aiArray[number] = num244 - 1f;
				return;
			}
			npc.noGravity = false;
			npc.noTileCollide = false;
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			CalamityGlobalNPC.hiveMind = npc.whoAmI;
			if (Main.netMode != 1) 
			{
				if (revenge)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= 600f)
					{
						npc.localAI[1] = 0f;
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("HiveBlob"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}
				}
				if (npc.localAI[0] == 0f) 
				{
					npc.localAI[0] = 1f;
					for (int num723 = 0; num723 < 5; num723++) 
					{
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("HiveBlob"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					}
				}
			}
			bool flag100 = false;
			int num568 = 0;
			if (expertMode)
			{
				for (int num569 = 0; num569 < 200; num569++)
				{
					if (Main.npc[num569].active && Main.npc[num569].type == mod.NPCType("DankCreeper"))
					{
						flag100 = true;
						num568++;
					}
				}
				npc.defense += num568 * 25;
			}
			if (expertMode)
			{
				if (!flag100)
				{
					npc.defense = 10;
				}
			}
			if (npc.ai[3] == 0f && npc.life > 0)
			{
				npc.ai[3] = (float)npc.lifeMax;
			}
	       	if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.25);
					if ((float)(npc.life + num660) < npc.ai[3])
					{
						npc.ai[3] = (float)npc.life;
						int num661 = Main.rand.Next(3, 6);
						for (int num662 = 0; num662 < num661; num662++)
						{
							int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
							int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
							int num663 = mod.NPCType("HiveBlob");
							if (Main.rand.Next(3) == 0 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
							{
								num663 = mod.NPCType("DankCreeper");
							}
							int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
							Main.npc[num664].SetDefaults(num663, -1f);
							Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
							Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							if (Main.netMode == 2 && num664 < 200)
							{
								NetMessage.SendData(23, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
							}
						}
						return;
					}
				}
			}
			burrowTimer--;
			if (burrowTimer < -120)
			{
				burrowTimer = 600;
				npc.scale = 1f;
				npc.alpha = 0;
				npc.dontTakeDamage = false;
                npc.damage = oldDamage;
            }
			else if (burrowTimer < -60)
			{
				npc.scale += 0.0165f;
				npc.alpha -= 4;
				int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 2.5f * npc.scale);
				Main.dust[num622].velocity *= 2f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
				for (int i = 0; i < 2; i++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 3.5f * npc.scale);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 3.5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 2.5f * npc.scale);
					Main.dust[num624].velocity *= 1f;
				}
			}
			else if (burrowTimer == -60)
			{
				npc.scale = 0.01f;
				if (Main.netMode != 1)
				{
					npc.Center = player.Center;
					npc.position.Y = player.position.Y - npc.height;
					int tilePosX = (int)npc.Center.X / 16;
					int tilePosY = (int)(npc.position.Y + npc.height) / 16 + 1;
					if (Main.tile[tilePosX, tilePosY] == null)
						Main.tile[tilePosX, tilePosY] = new Tile();
					while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[(int)Main.tile[tilePosX, tilePosY].type]))
					{
						tilePosY++;
						npc.position.Y += 16;
						if (Main.tile[tilePosX, tilePosY] == null)
							Main.tile[tilePosX, tilePosY] = new Tile();
					}
				}
                npc.netUpdate = true;
            }
			else if (burrowTimer < 0)
			{
				npc.scale -= 0.0165f;
				npc.alpha += 4;
				int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 2.5f * npc.scale);
				Main.dust[num622].velocity *= 2f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
				for (int i = 0; i < 2; i++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 3.5f * npc.scale);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 3.5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default(Color), 2.5f * npc.scale);
					Main.dust[num624].velocity *= 1f;
				}
			}
			else if (burrowTimer == 0)
			{
				if (!player.active || player.dead)
				{
					burrowTimer = 30;
				}
				else
				{
					npc.dontTakeDamage = true;
                    oldDamage = npc.damage;
                    npc.damage = 0;
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
            if (npc.life > 0)
            {
                if (NPC.CountNPCS(NPCID.EaterofSouls) < 3 && NPC.CountNPCS(NPCID.DevourerHead) < 1)
                {
                    if (Main.rand.Next(60) == 0 && Main.netMode != 1)
                    {
                        Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                        NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.EaterofSouls);
                    }
                    if (Main.rand.Next(150) == 0 && Main.netMode != 1)
                    {
                        Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                        NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.DevourerHead);
                    }
                }
                int num285 = 0;
                while ((double)num285 < damage / (double)npc.lifeMax * 100.0)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 14, (float)hitDirection, -1f, 0, default(Color), 1f);
                    num285++;
                }
            }
            else
            {
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore3"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore4"), 1f);
				if (Main.netMode != 1)
				{
					if (NPC.CountNPCS(mod.NPCType("HiveMindP2")) < 1)
					{
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("HiveMindP2"), npc.whoAmI, 0f, 0f, 0f, 0f, npc.target);
						Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
					}
				}
            }
		}

		public override bool CanHitPlayer (Player target, ref int cooldownSlot)
		{
			return npc.scale == 1f; //no damage when shrunk
		}

		public override bool? DrawHealthBar (byte hbPosition, ref float scale, ref Vector2 position)
		{
			return npc.scale == 1f;
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 300, true);
			}
		}
	}
}