using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod.NPCs.Calamitas;
using CalamityMod;

namespace CalamityMod.NPCs.Calamitas
{
	[AutoloadBossHead]
	public class CalamitasRun3 : ModNPC
	{
        private float bossLife;
        private bool halfLife = false;
        private bool secondStage = false;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calamitas");
			Main.npcFrameCount[npc.type] = 3;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 80;
			npc.npcSlots = 14f;
			npc.width = 120;
			npc.height = 120;
			npc.defense = 25;
			animationType = 125;
			npc.lifeMax = CalamityWorld.revenge ? 36750 : 27500;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 60250;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 15, 0, 0);
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
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Calamitas");
			bossBag = mod.ItemType("CalamitasBag");
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 160;
				npc.defense = 150;
				npc.lifeMax = 120000;
				npc.value = Item.buyPrice(1, 15, 0, 0);
			}
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 6500000 : 5900000;
            }
        }
		
		public override void AI()
		{
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool dayTime = Main.dayTime;
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			Player player = Main.player[npc.target];
			if (!halfLife && npc.life <= npc.lifeMax / 2)
			{
				if (!secondStage && Main.netMode != 1) 
				{
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
					for (int I = 0; I < 5; I++) 
					{
						int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), mod.NPCType("SoulSeeker"), npc.whoAmI, 0, 0, 0, -1);
						NPC Eye = Main.npc[FireEye];
						Eye.ai[0] = I * 72;
						Eye.ai[3] = I * 72;
					}
					secondStage = true;
				}
				string key = "Mods.CalamityMod.CalamitasBossText3";
				Color messageColor = Color.Orange;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				halfLife = true;
			}
			if (bossLife == 0f && npc.life > 0)
			{
				bossLife = (float)npc.lifeMax;
			}
	       	if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.45);
					if ((float)(npc.life + num660) < bossLife)
					{
						bossLife = (float)npc.life;
						if (!expertMode)
						{
							int num661 = Main.rand.Next(8, 13);
							for (int num662 = 0; num662 < num661; num662++)
							{
								int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
								int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
								int num663 = mod.NPCType("LifeSeeker");
								int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
								Main.npc[num664].SetDefaults(num663, -1f);
								Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
								Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
							}
						}
						else
						{
                            if (revenge)
                            {
                                int num661 = Main.rand.Next(5, 9);
                                for (int num662 = 0; num662 < num661; num662++)
                                {
                                    int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
                                    int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
                                    int num663 = mod.NPCType("LifeSeeker");
                                    int num664 = NPC.NewNPC(x, y, num663, 0, 0f, 0f, 0f, 0f, 255);
                                    Main.npc[num664].SetDefaults(num663, -1f);
                                    Main.npc[num664].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                                    Main.npc[num664].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                                }
                            }
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("RedSpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("GraySpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
                            if (CalamityWorld.death)
                            {
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("RedSpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, mod.ProjectileType("GraySpawn"), 0, 0f, Main.myPlayer, 0f, 0f);
                            }
							string key = "Mods.CalamityMod.CalamitasBossText4";
							Color messageColor = Color.Orange;
							if (Main.netMode == 0)
							{
								Main.NewText(Language.GetTextValue(key), messageColor);
							}
							else if (Main.netMode == 2)
							{
								NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
							}
						}
						return;
					}
				}
	       	}
			bool flag100 = false;
			int num568 = 0;
			if (expertMode)
			{
                if (NPC.AnyNPCs(mod.NPCType("CalamitasRun")) || NPC.AnyNPCs(mod.NPCType("CalamitasRun2")))
                {
                    flag100 = true;
                    num568 += 255;
                }
				npc.defense += num568 * 50;
                if (!flag100)
                {
                    npc.defense = 30;
                }
            }
            npc.chaseable = !flag100;
			if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest(true);
			}
			float num801 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
			float num802 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
			float num803 = (float)Math.Atan2((double)num802, (double)num801) + 1.57f;
			if (num803 < 0f)
			{
				num803 += 6.283f;
			}
			else if ((double)num803 > 6.283)
			{
				num803 -= 6.283f;
			}
			float num804 = 0.1f;
			if (npc.rotation < num803)
			{
				if ((double)(num803 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num804;
				}
				else
				{
					npc.rotation += num804;
				}
			}
			else if (npc.rotation > num803)
			{
				if ((double)(npc.rotation - num803) > 3.1415)
				{
					npc.rotation += num804;
				}
				else
				{
					npc.rotation -= num804;
				}
			}
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
			{
				npc.rotation = num803;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
			}
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
			{
				npc.rotation = num803;
			}
			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					npc.velocity = new Vector2(0f, -10f);
					if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else if (npc.timeLeft < 1800)
			{
				npc.timeLeft = 1800;
			}
			if (npc.ai[1] == 0f)
			{
				float num823 = expertMode ? 10.5f : 9f;
				float num824 = expertMode ? 0.195f : 0.175f;
				Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
				float num826 = player.position.Y + (float)(player.height / 2) - 360f - vector82.Y;
				float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
				num827 = num823 / num827;
				num825 *= num827;
				num826 *= num827;
				if (npc.velocity.X < num825)
				{
					npc.velocity.X = npc.velocity.X + num824;
					if (npc.velocity.X < 0f && num825 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num824;
					}
				}
				else if (npc.velocity.X > num825)
				{
					npc.velocity.X = npc.velocity.X - num824;
					if (npc.velocity.X > 0f && num825 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num824;
					}
				}
				if (npc.velocity.Y < num826)
				{
					npc.velocity.Y = npc.velocity.Y + num824;
					if (npc.velocity.Y < 0f && num826 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num824;
					}
				}
				else if (npc.velocity.Y > num826)
				{
					npc.velocity.Y = npc.velocity.Y - num824;
					if (npc.velocity.Y > 0f && num826 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num824;
					}
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 200f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}
				vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num825 = player.position.X + (float)(player.width / 2) - vector82.X;
				num826 = player.position.Y + (float)(player.height / 2) - vector82.Y;
				npc.rotation = (float)Math.Atan2((double)num826, (double)num825) - 1.57f;
				if (Main.netMode != 1)
				{
					npc.localAI[1] += 1f;
					if (revenge)
					{
						npc.localAI[1] += 0.5f;
					}
                    if (CalamityWorld.death || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[1] += 1f;
                    }
					if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						npc.localAI[1] = 0f;
						float num828 = expertMode ? 14f : 12.5f;
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            num828 += 5f;
                        }
						int num829 = expertMode ? 34 : 42;
						int num830 = mod.ProjectileType("BrimstoneHellfireball");
						num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
						num827 = num828 / num827;
						num825 *= num827;
						num826 *= num827;
						vector82.X += num825 * 15f;
						vector82.Y += num826 * 15f;
						Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						return;
					}
				}
			}
			else
			{
				int num831 = 1;
				if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
				{
					num831 = -1;
				}
				float num832 = expertMode ? 10.5f : 9f;
				float num833 = expertMode ? 0.3f : 0.25f;
				Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * 360) - vector83.X;
				float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
				float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
				num836 = num832 / num836;
				num834 *= num836;
				num835 *= num836;
				if (npc.velocity.X < num834)
				{
					npc.velocity.X = npc.velocity.X + num833;
					if (npc.velocity.X < 0f && num834 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num833;
					}
				}
				else if (npc.velocity.X > num834)
				{
					npc.velocity.X = npc.velocity.X - num833;
					if (npc.velocity.X > 0f && num834 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num833;
					}
				}
				if (npc.velocity.Y < num835)
				{
					npc.velocity.Y = npc.velocity.Y + num833;
					if (npc.velocity.Y < 0f && num835 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num833;
					}
				}
				else if (npc.velocity.Y > num835)
				{
					npc.velocity.Y = npc.velocity.Y - num833;
					if (npc.velocity.Y > 0f && num835 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num833;
					}
				}
				vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num834 = player.position.X + (float)(player.width / 2) - vector83.X;
				num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
				npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;
				if (Main.netMode != 1)
				{
					npc.localAI[1] += 1f;
					if (revenge)
					{
						npc.localAI[1] += 0.5f;
					}
                    if (CalamityWorld.death || CalamityWorld.bossRushActive)
                    {
                        npc.localAI[1] += 0.5f;
                    }
                    if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                    {
                        npc.localAI[1] += 1f;
                    }
					if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if (expertMode)
					{
						npc.localAI[1] += 1.5f;
					}
					if (npc.localAI[1] > 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						npc.localAI[1] = 0f;
						float num837 = 11f;
						int num838 = expertMode ? 24 : 30;
						int num839 = mod.ProjectileType("BrimstoneLaser");
						num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
						num836 = num837 / num836;
						num834 *= num836;
						num835 *= num836;
						vector83.X += num834 * 15f;
						vector83.Y += num835 * 15f;
						Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					}
				}
				npc.ai[2] += 1f;
				if (npc.ai[2] >= 120f)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
					return;
				}
			}
		}

        public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CalamitasTrophy"));
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
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"), Main.rand.Next(3, 6));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CalamityDust"), Main.rand.Next(9, 15));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BlightedLens"), Main.rand.Next(1, 3));
                if (CalamityWorld.downedProvidence)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Bloodstone"), Main.rand.Next(30, 41));
                }
                if (Main.rand.Next(10) == 0)
                {
                    npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("ChaosStone"), 1, true);
                }
                if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CalamitasInferno"));
				}
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CalamitasMask"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TheEyeofCalamitas"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BlightedEyeStaff"));
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "A Calamitas Doppelganger";
			potionType = ItemID.GreaterHealingPotion;
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
				npc.width = 100;
				npc.height = 100;
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
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.damage = (int)(npc.damage * 0.8f);
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 180, true);
			}
			player.AddBuff(mod.BuffType("BrimstoneFlames"), 300, true);
		}
	}
}