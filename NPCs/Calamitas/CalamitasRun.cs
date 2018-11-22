using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Calamitas
{
	[AutoloadBossHead]
	public class CalamitasRun : ModNPC
	{
        public bool canDespawn = false;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cataclysm");
			Main.npcFrameCount[npc.type] = 3;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 70;
			npc.npcSlots = 5f;
			npc.width = 120; //324
			npc.height = 120; //216
			npc.defense = 10;
			animationType = 126;
			npc.alpha = 50;
			npc.lifeMax = CalamityWorld.revenge ? 4400 : 3000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 5000;
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
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Calamitas");
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 170;
				npc.defense = 80;
				npc.lifeMax = 35000;
			}
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1700000 : 1400000;
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
			float num840 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
			float num841 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
			float num842 = (float)Math.Atan2((double)num841, (double)num840) + 1.57f;
			if (num842 < 0f)
			{
				num842 += 6.283f;
			}
			else if ((double)num842 > 6.283)
			{
				num842 -= 6.283f;
			}
			float num843 = 0.15f;
			if (npc.rotation < num842)
			{
				if ((double)(num842 - npc.rotation) > 3.1415)
				{
					npc.rotation -= num843;
				}
				else
				{
					npc.rotation += num843;
				}
			}
			else if (npc.rotation > num842)
			{
				if ((double)(npc.rotation - num842) > 3.1415)
				{
					npc.rotation += num843;
				}
				else
				{
					npc.rotation -= num843;
				}
			}
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
			{
				npc.rotation = num842;
			}
			if (npc.rotation < 0f)
			{
				npc.rotation += 6.283f;
			}
			else if ((double)npc.rotation > 6.283)
			{
				npc.rotation -= 6.283f;
			}
			if (npc.rotation > num842 - num843 && npc.rotation < num842 + num843)
			{
				npc.rotation = num842;
			}
			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					npc.velocity = new Vector2(0f, -10f);
                    canDespawn = true;
                    if (npc.timeLeft > 150)
					{
						npc.timeLeft = 150;
					}
					return;
				}
			}
			else
			{
                canDespawn = false;
            }
			if (npc.ai[1] == 0f)
			{
				float num861 = 4f;
				float num862 = 0.1f;
				int num863 = 1;
				if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
				{
					num863 = -1;
				}
				Vector2 vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num864 = player.position.X + (float)(player.width / 2) + (float)(num863 * 180) - vector86.X;
				float num865 = player.position.Y + (float)(player.height / 2) - vector86.Y;
				float num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
				if (expertMode)
				{
					if (num866 > 300f)
					{
						num861 += 0.5f;
					}
					if (num866 > 400f)
					{
						num861 += 0.5f;
					}
					if (num866 > 500f)
					{
						num861 += 0.55f;
					}
					if (num866 > 600f)
					{
						num861 += 0.55f;
					}
					if (num866 > 700f)
					{
						num861 += 0.6f;
					}
					if (num866 > 800f)
					{
						num861 += 0.6f;
					}
				}
				num866 = num861 / num866;
				num864 *= num866;
				num865 *= num866;
				if (npc.velocity.X < num864)
				{
					npc.velocity.X = npc.velocity.X + num862;
					if (npc.velocity.X < 0f && num864 > 0f)
					{
						npc.velocity.X = npc.velocity.X + num862;
					}
				}
				else if (npc.velocity.X > num864)
				{
					npc.velocity.X = npc.velocity.X - num862;
					if (npc.velocity.X > 0f && num864 < 0f)
					{
						npc.velocity.X = npc.velocity.X - num862;
					}
				}
				if (npc.velocity.Y < num865)
				{
					npc.velocity.Y = npc.velocity.Y + num862;
					if (npc.velocity.Y < 0f && num865 > 0f)
					{
						npc.velocity.Y = npc.velocity.Y + num862;
					}
				}
				else if (npc.velocity.Y > num865)
				{
					npc.velocity.Y = npc.velocity.Y - num862;
					if (npc.velocity.Y > 0f && num865 < 0f)
					{
						npc.velocity.Y = npc.velocity.Y - num862;
					}
				}
				npc.ai[2] += (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 2f : 1f);
				if (npc.ai[2] >= 400f)
				{
					npc.ai[1] = 1f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.target = 255;
					npc.netUpdate = true;
				}
				if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
				{
					npc.localAI[2] += 1f;
					if (npc.localAI[2] > 22f)
					{
						npc.localAI[2] = 0f;
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 34);
					}
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
							npc.localAI[1] += 1f;
						}
						if (npc.localAI[1] > 8f)
						{
							npc.localAI[1] = 0f;
							float num867 = 6f;
							int num868 = expertMode ? 26 : 32;
							int num869 = mod.ProjectileType("BrimstoneFire");
							vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							num864 = player.position.X + (float)(player.width / 2) - vector86.X;
							num865 = player.position.Y + (float)(player.height / 2) - vector86.Y;
							num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
							num866 = num867 / num866;
							num864 *= num866;
							num865 *= num866;
							num865 += (float)Main.rand.Next(-40, 41) * 0.01f;
							num864 += (float)Main.rand.Next(-40, 41) * 0.01f;
							num865 += npc.velocity.Y * 0.5f;
							num864 += npc.velocity.X * 0.5f;
							vector86.X -= num864 * 1f;
							vector86.Y -= num865 * 1f;
							Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, num868 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
							return;
						}
					}
				}
			}
			else
			{
				if (npc.ai[1] == 1f)
				{
					Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 0);
					npc.rotation = num842;
					float num870 = 15f;
					if (expertMode)
					{
						num870 += 2.5f;
					}
					if (revenge)
					{
						num870 += 1f;
					}
                    if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                    {
                        num870 += 4f;
                    }
					Vector2 vector87 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					float num871 = player.position.X + (float)(player.width / 2) - vector87.X;
					float num872 = player.position.Y + (float)(player.height / 2) - vector87.Y;
					float num873 = (float)Math.Sqrt((double)(num871 * num871 + num872 * num872));
					num873 = num870 / num873;
					npc.velocity.X = num871 * num873;
					npc.velocity.Y = num872 * num873;
					npc.ai[1] = 2f;
					return;
				}
				if (npc.ai[1] == 2f)
				{
					npc.ai[2] += 1f;
					if (expertMode)
					{
						npc.ai[2] += 0.5f;
					}
					if (revenge)
					{
						npc.ai[2] += 0.5f;
					}
					if (npc.ai[2] >= 50f)
					{
						npc.velocity.X = npc.velocity.X * 0.93f;
						npc.velocity.Y = npc.velocity.Y * 0.93f;
						if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
						{
							npc.velocity.X = 0f;
						}
						if ((double)npc.velocity.Y > -0.1 && (double)npc.velocity.Y < 0.1)
						{
							npc.velocity.Y = 0f;
						}
					}
					else
					{
						npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
					}
					if (npc.ai[2] >= 80f)
					{
						npc.ai[3] += 1f;
						npc.ai[2] = 0f;
						npc.target = 255;
						npc.rotation = num842;
						if (npc.ai[3] >= 5f)
						{
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							return;
						}
						npc.ai[1] = 1f;
						return;
					}
				}
			}
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged))
            {
                damage /= 2;
            }
        }

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CataclysmTrophy"));
			}
			if (Main.expertMode && Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrimstoneFlamesprayer"));
			}
			else if (Main.rand.Next(4) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrimstoneFlamesprayer"));
			}
			if (Main.expertMode && Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrimstoneFlameblaster"));
			}
			else if (Main.rand.Next(4) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BrimstoneFlameblaster"));
			}
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
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("MarkedforDeath"), 300);
				player.AddBuff(mod.BuffType("Horror"), 300, true);
			}
			player.AddBuff(mod.BuffType("BrimstoneFlames"), 180, true);
		}
	}
}