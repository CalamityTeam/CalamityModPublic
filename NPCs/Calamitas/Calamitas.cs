using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Calamitas
{
	[AutoloadBossHead]
	public class Calamitas : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Calamitas");
			Main.npcFrameCount[npc.type] = 3;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 70;
			npc.npcSlots = 14f;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 15;
			animationType = 125;
			npc.value = 0f;
			npc.lifeMax = CalamityWorld.revenge ? 15000 : 10000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 22500;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
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
			music = MusicID.Boss2;
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 150;
				npc.defense = 130;
				npc.lifeMax = 80000;
			}
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 2600000 : 2200000;
            }
        }
		
		public override void AI()
		{
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool dayTime = Main.dayTime;
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			Player player = Main.player[npc.target];
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
				float num823 = expertMode ? 10f : 8.5f;
				float num824 = expertMode ? 0.185f : 0.165f;
				Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
				float num826 = player.position.Y + (float)(player.height / 2) - 300f - vector82.Y;
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
				if (npc.ai[2] >= 300f)
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
					if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 2f;
					}
					if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						npc.localAI[1] = 0f;
						float num828 = expertMode ? 13f : 10.5f;
						int num829 = expertMode ? 24 : 30;
						int num830 = mod.ProjectileType("BrimstoneLaser");
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
				float num832 = expertMode ? 10f : 8.5f;
				float num833 = expertMode ? 0.275f : 0.225f;
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
					if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1f;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1.5f;
					}
					if (Main.expertMode || CalamityWorld.bossRushActive)
					{
						npc.localAI[1] += 1.5f;
					}
					if (npc.localAI[1] > 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
					{
						npc.localAI[1] = 0f;
						float num837 = 10.5f;
						int num838 = expertMode ? 15 : 22;
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
				if (npc.ai[2] >= 180f)
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

        public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life > 0)
			{
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
			else
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
		
		public override bool CheckDead()
		{
			if (Main.netMode != 1)
			{
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, mod.NPCType("CalamitasRun3"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, mod.NPCType("CalamitasRun"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, mod.NPCType("CalamitasRun2"), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
			}
			string key = "Mods.CalamityMod.CalamitasBossText";
			string key2 = "Mods.CalamityMod.CalamitasBossText2";
			Color messageColor = Color.Orange;
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(key), messageColor);
				Main.NewText(Language.GetTextValue(key2), messageColor);
			}
			else if (Main.netMode == 2)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor);
			}
			return true;
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
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