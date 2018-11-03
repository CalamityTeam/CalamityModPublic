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

namespace CalamityMod.NPCs.Providence
{
	[AutoloadBossHead]
	public class Providence : ModNPC
	{
        private bool text = false;
        private float bossLife;
        private int phaseChange = 0;
        private int immuneTimer = 300;
        private int frameUsed = 0;
        private int healTimer = 0;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Providence, the Profaned Goddess");
			Main.npcFrameCount[npc.type] = 3;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 36f;
			npc.damage = 100;
			npc.width = 600;
			npc.height = 450;
			npc.defense = 50;
			npc.lifeMax = CalamityWorld.revenge ? 299000 : 264500;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 402500;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 9900000 : 9000000;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.value = Item.buyPrice(3, 0, 0, 0);
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
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
			npc.chaseable = true;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/ProvidenceTheme");
			npc.HitSound = SoundID.NPCHit44;
			npc.DeathSound = SoundID.NPCDeath46;
			bossBag = mod.ItemType("ProvidenceBag");
		}
		
		public override void AI()
		{
            npc.damage = 0;
			CalamityGlobalNPC.holyBoss = npc.whoAmI;
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Player player = Main.player[npc.target];
			Vector2 vector = npc.Center;
			bool isHoly = player.ZoneHoly;
			bool isHell = player.ZoneUnderworldHeight;
            bool canAttack = true;
            bool attackMore = (double)npc.life <= (double)npc.lifeMax * 0.5;
            if (!isHoly && !isHell && !CalamityWorld.bossRushActive)
            {
                if (immuneTimer > 0)
                {
                    immuneTimer--;
                }
            }
            else
            {
                immuneTimer = 300;
            }
            npc.dontTakeDamage = (immuneTimer <= 0);
            npc.rotation = npc.velocity.X * 0.004f;
            if (Main.raining)
            {
                Main.raining = false;
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
			if (NPC.CountNPCS(mod.NPCType("ProvSpawnHealer")) > 0)
			{
				float heal = revenge ? 90f : 120f;
                if (CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    heal = 30f;
                }
                healTimer++;
				if (healTimer >= heal)
				{
					healTimer = 0;
					if (Main.netMode != 1)
					{
						int healAmt = npc.lifeMax / 200;
						if (healAmt > npc.lifeMax - npc.life)
						{
							healAmt = npc.lifeMax - npc.life;
						}
						if (healAmt > 0)
						{
							npc.life += healAmt;
							npc.HealEffect(healAmt, true);
							npc.netUpdate = true;
						}
					}
				}
			}
            bool tooFarAway = Vector2.Distance(Main.player[npc.target].Center, vector) > 2800f;
			if ((!Main.dayTime && npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f) || player.dead)
			{
				if (npc.timeLeft > 10)
				{
					npc.timeLeft = 10;
				}
				if (npc.velocity.X > 0f) 
				{
					npc.velocity.X = npc.velocity.X + 0.25f;
				} 
				else 
				{
					npc.velocity.X = npc.velocity.X - 0.25f;
				}
				npc.velocity.Y = npc.velocity.Y - 0.25f;
			}
            else if (npc.timeLeft < 3600)
            {
                npc.timeLeft = 3600;
            }
            if (tooFarAway)
            {
                if (Main.netMode != 2)
                {
                    if (!Main.player[npc.target].dead && Main.player[npc.target].active)
                    {
                        Main.player[npc.target].AddBuff(mod.BuffType("HolyInferno"), 2);
                    }
                }
            }
			if (bossLife == 0f && npc.life > 0)
			{
				bossLife = (float)npc.lifeMax;
			}
	       	if (npc.life > 0)
			{
				if (Main.netMode != 1)
				{
					int num660 = (int)((double)npc.lifeMax * 0.3);
                    if ((float)(npc.life + num660) < bossLife)
                    {
                        bossLife = (float)npc.life;
                        int x = (int)(npc.position.X + (float)Main.rand.Next(npc.width - 32));
                        int y = (int)(npc.position.Y + (float)Main.rand.Next(npc.height - 32));
                        int spawn1 = mod.NPCType("ProvSpawnDefense");
                        int spawn2 = mod.NPCType("ProvSpawnHealer");
                        int spawn3 = mod.NPCType("ProvSpawnOffense");
                        int spawnNPC1 = NPC.NewNPC(x - 100, y - 100, spawn1, 0, 0f, 0f, 0f, 0f, 255);
                        int spawnNPC2 = NPC.NewNPC(x + 100, y - 100, spawn2, 0, 0f, 0f, 0f, 0f, 255);
                        int spawnNPC3 = NPC.NewNPC(x, y + 100, spawn3, 0, 0f, 0f, 0f, 0f, 255);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(23, -1, -1, null, spawnNPC1, 0f, 0f, 0f, 0, 0, 0);
                            NetMessage.SendData(23, -1, -1, null, spawnNPC2, 0f, 0f, 0f, 0, 0, 0);
                            NetMessage.SendData(23, -1, -1, null, spawnNPC3, 0f, 0f, 0f, 0, 0, 0);
                        }
                        return;
                    }
				}
	       	}
            if (NPC.CountNPCS(mod.NPCType("ProvSpawnOffense")) > 0 ||
                NPC.CountNPCS(mod.NPCType("ProvSpawnDefense")) > 0 ||
                NPC.CountNPCS(mod.NPCType("ProvSpawnHealer")) > 0)
            {
                npc.dontTakeDamage = true;
                canAttack = false;
            }
            if (npc.ai[0] != 2f && npc.ai[0] != 5f && npc.ai[0] != 7f)
            {
                if (npc.ai[2] == 0f)
                {
                    npc.TargetClosest(true);
                    if (npc.Center.X < player.Center.X)
                    {
                        npc.ai[2] = 1f;
                    }
                    else
                    {
                        npc.ai[2] = -1f;
                    }
                }
                npc.TargetClosest(true);
                int num851 = 800;
                float num852 = Math.Abs(npc.Center.X - player.Center.X);
                if (npc.Center.X < player.Center.X && npc.ai[2] < 0f && num852 > (float)num851)
                {
                    npc.ai[2] = 0f;
                }
                if (npc.Center.X > player.Center.X && npc.ai[2] > 0f && num852 > (float)num851)
                {
                    npc.ai[2] = 0f;
                }
                float num853 = expertMode ? 1.1f : 1.05f;
                float num854 = expertMode ? 16f : 15f;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                {
                    num853 = expertMode ? 1.15f : 1.1f;
                    num854 = expertMode ? 17f : 16f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    num853 = expertMode ? 1.2f : 1.15f;
                    num854 = expertMode ? 18f : 17f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                {
                    num853 = expertMode ? 1.25f : 1.2f;
                    num854 = expertMode ? 19f : 18f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                {
                    num853 = expertMode ? 1.3f : 1.25f;
                    num854 = expertMode ? 20f : 19f;
                }
                npc.velocity.X = npc.velocity.X + npc.ai[2] * num853;
                if (npc.velocity.X > num854)
                {
                    npc.velocity.X = num854;
                }
                if (npc.velocity.X < -num854)
                {
                    npc.velocity.X = -num854;
                }
                float num855 = player.position.Y - (npc.position.Y + (float)npc.height);
                if (num855 < 200f) //150
                {
                    npc.velocity.Y = npc.velocity.Y - 0.2f;
                }
                if (num855 > 250f) //200
                {
                    npc.velocity.Y = npc.velocity.Y + 0.2f;
                }
                if (npc.velocity.Y > 6f) //8
                {
                    npc.velocity.Y = 6f;
                }
                if (npc.velocity.Y < -6f) //8
                {
                    npc.velocity.Y = -6f;
                }
            }
            if (npc.ai[0] == 0f) 
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = true;
				float num852 = Math.Abs(npc.Center.X - player.Center.X);
				if ((num852 < 500f || npc.ai[3] < 0f) && npc.position.Y < player.position.Y) 
				{
					npc.ai[3] += 1f;
					int num856 = expertMode ? 25 : 26;
					if ((double)npc.life < (double)npc.lifeMax * 0.5) 
					{
						num856 = expertMode ? 23 : 24;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) 
					{
						num856 = expertMode ? 21 : 22;
					}
                    if (!canAttack)
                    {
                        if (attackMore)
                        {
                            num856 = expertMode ? 35 : 40;
                        }
                        else
                        {
                            num856 = expertMode ? 40 : 46;
                        }
                    }
					num856++;
					if (npc.ai[3] > (float)num856) 
					{
						npc.ai[3] = (float)(-(float)num856);
					}
					if (npc.ai[3] == 0f && Main.netMode != 1) 
					{
						Vector2 vector112 = new Vector2(npc.Center.X, npc.Center.Y);
						vector112.X += npc.velocity.X * 7f;
						float num857 = player.position.X + (float)player.width * 0.5f - vector112.X;
						float num858 = player.Center.Y - vector112.Y;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));
						float num860 = expertMode ? 10.25f : 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5) 
						{
							num860 = expertMode ? 11.5f : 10f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) 
						{
							num860 = expertMode ? 12.75f : 11f;
						}
						if (revenge)
						{
							num860 *= 1.15f;
						}
						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;
						int holyDamage = expertMode ? 46 : 63;
                        Projectile.NewProjectile(vector112.X, vector112.Y, num857, num858, mod.ProjectileType("HolyBlast"), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				else if (npc.ai[3] < 0f) 
				{
					npc.ai[3] += 1f;
				}
				if (Main.netMode != 1) 
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f) 
					{
						npc.ai[0] = -1f;
					}
				}
			}
			else if (npc.ai[0] == 1f) 
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = true;
				if (Main.netMode != 1) 
				{
					npc.ai[3] += 1f;
					int num864 = expertMode ? 33 : 36;
					if ((double)npc.life < (double)npc.lifeMax * 0.5) 
					{
						num864 = expertMode ? 30 : 33;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive) 
					{
						num864 = expertMode ? 26 : 29;
					}
                    if (!canAttack)
                    {
                        if (attackMore)
                        {
                            num864 = expertMode ? 40 : 45;
                        }
                        else
                        {
                            num864 = expertMode ? 45 : 52;
                        }
                    }
                    num864 += 3;
                    if (npc.ai[3] >= (float)num864)
                    {
                        npc.ai[3] = 0f;
                        Vector2 vector113 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
                        float num865 = npc.velocity.Y;
                        if (num865 < 0f)
                        {
                            num865 = 0f;
                        }
                        num865 += expertMode ? 4f : 3f;
                        float speedX2 = npc.velocity.X * 0.25f;
                        int fireDamage = expertMode ? 40 : 59; //260 200
                        Projectile.NewProjectile(vector113.X, vector113.Y, speedX2, num865, mod.ProjectileType("HolyFire"), fireDamage, 0f, Main.myPlayer, 0f, 0f);
                    }
				}
				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
					{
						npc.ai[0] = -1f;
					}
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = false;
				npc.TargetClosest(true);
				Vector2 vector114 = new Vector2(npc.Center.X, npc.Center.Y - 20f);
				float num866 = (float)Main.rand.Next(-1000, 1001);
				float num867 = (float)Main.rand.Next(-1000, 1001);
				float num868 = (float)Math.Sqrt((double)(num866 * num866 + num867 * num867));
				float num869 = 3f;
				npc.velocity *= 0.95f;
				num868 = num869 / num868;
				num866 *= num868;
				num867 *= num868;
				vector114.X += num866 * 4f;
				vector114.Y += num867 * 4f;
				npc.ai[3] += 1f;
				int num870 = expertMode ? 3 : 4;
                if (!canAttack)
                {
                    if (attackMore)
                    {
                        num870 = expertMode ? 8 : 11;
                    }
                    else
                    {
                        num870 = expertMode ? 12 : 16;
                    }
                }
				if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
				{
					num870 -= 2;
				}
				if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive) 
				{
					num870 -= 2;
				}
				if (npc.ai[3] > (float)num870) 
				{
					npc.ai[3] = 0f;
                    if (Main.netMode != 1)
                    {
                        if (Main.rand.Next(4) == 0 && !CalamityWorld.death && !CalamityWorld.bossRushActive)
                        {
                            Projectile.NewProjectile(vector114.X, vector114.Y, num866, num867, mod.ProjectileType("HolyLight"), 0, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else
                        {
                            Projectile.NewProjectile(vector114.X, vector114.Y, num866, num867, mod.ProjectileType("HolyBurnOrb"), 0, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
				}
				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 450f && !text)
					{
						text = true;
						string key = "Mods.CalamityMod.ProfanedBossText";
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
					if (npc.ai[1] > 600f) 
					{
						Main.PlaySound(SoundID.Item20, player.position);
						player.AddBuff(mod.BuffType("ExtremeGravity"), 1500, true);
						for (int num621 = 0; num621 < 40; num621++)
						{
							int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num622].velocity *= 3f;
							if (Main.rand.Next(2) == 0)
							{
								Main.dust[num622].scale = 0.5f;
								Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
							}
						}
						for (int num623 = 0; num623 < 60; num623++)
						{
							int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default(Color), 3f);
							Main.dust[num624].noGravity = true;
							Main.dust[num624].velocity *= 5f;
							num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default(Color), 2f);
							Main.dust[num624].velocity *= 2f;
						}
						text = false;
						npc.ai[0] = -1f;
					}
				}
			}
			if (npc.ai[0] == 3f) 
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = true;
				float num852 = Math.Abs(npc.Center.X - player.Center.X);
				if ((num852 < 500f || npc.ai[3] < 0f) && npc.position.Y < player.position.Y)
				{
					npc.ai[3] += 1f;
					int num856 = expertMode ? 10 : 11;
					if ((double)npc.life < (double)npc.lifeMax * 0.5)
					{
						num856 = expertMode ? 9 : 10;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
					{
						num856 = expertMode ? 8 : 9;
					}
                    if (!canAttack)
                    {
                        if (attackMore)
                        {
                            num856 = expertMode ? 20 : 23;
                        }
                        else
                        {
                            num856 = expertMode ? 30 : 35;
                        }
                    }
                    num856++;
					if (npc.ai[3] > (float)num856)
					{
						npc.ai[3] = (float)(-(float)num856);
					}
					if (npc.ai[3] == 0f && Main.netMode != 1) 
					{
						Vector2 vector112 = new Vector2(npc.Center.X, npc.Center.Y);
						vector112.X += npc.velocity.X * 7f;
						float num857 = player.position.X + (float)player.width * 0.5f - vector112.X;
						float num858 = player.Center.Y - vector112.Y;
						float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));
						float num860 = expertMode ? 10.25f : 9f;
						if ((double)npc.life < (double)npc.lifeMax * 0.5) 
						{
							num860 = expertMode ? 11.5f : 10f;
						}
						if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive) 
						{
							num860 = expertMode ? 12.75f : 11f;
						}
						if (revenge)
						{
							num860 *= 1.15f;
						}
						num859 = num860 / num859;
						num857 *= num859;
						num858 *= num859;
						int holyDamage = expertMode ? 39 : 55; //280 210
                        Projectile.NewProjectile(vector112.X, vector112.Y, num857 * 0.1f, num858, mod.ProjectileType("MoltenBlast"), holyDamage, 0f, Main.myPlayer, 0f, 0f);
					}
				} 
				else if (npc.ai[3] < 0f)
				{
					npc.ai[3] += 1f;
				}
				if (Main.netMode != 1)
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f)
					{
						npc.ai[0] = -1f;
					}
				}
			}
			else if (npc.ai[0] == 4f) 
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = true;
				if (Main.netMode != 1) 
				{
					npc.ai[3] += 1f;
					int num864 = expertMode ? 70 : 74;
					if ((double)npc.life < (double)npc.lifeMax * 0.5) 
					{
						num864 = expertMode ? 64 : 70;
					}
					if ((double)npc.life < (double)npc.lifeMax * 0.1 || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) 
					{
						num864 = expertMode ? 56 : 64;
					}
                    if (!canAttack)
                    {
                        if (attackMore)
                        {
                            num864 = expertMode ? 115 : 135;
                        }
                        else
                        {
                            num864 = expertMode ? 148 : 156;
                        }
                    }
                    num864 += 3;
					if (npc.ai[3] >= (float)num864) 
					{
						npc.ai[3] = 0f;
						Vector2 vector113 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
						int i2 = (int)(vector113.X / 16f);
						int j2 = (int)(vector113.Y / 16f);
						if (!WorldGen.SolidTile(i2, j2)) 
						{
							float num865 = npc.velocity.Y;
							if (num865 < 0f) 
							{
								num865 = 0f;
							}
							num865 += expertMode ? 4f : 3f;
							float speedX2 = npc.velocity.X * 0.25f;
							int fireDamage = expertMode ? 44 : 60; //260 100
                            Projectile.NewProjectile(vector113.X, vector113.Y, speedX2, num865, mod.ProjectileType("HolyBomb"), fireDamage, 0f, Main.myPlayer, (float)Main.rand.Next(5), 0f);
						}
					}
				}
				if (Main.netMode != 1) 
				{
					npc.ai[1] += 1f;
					if (npc.ai[1] > 300f) 
					{
						npc.ai[0] = -1f;
					}
				}
			}
            else if (npc.ai[0] == 5f)
            {
                npc.noGravity = true;
                npc.noTileCollide = true;
                npc.chaseable = false;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                if (Main.netMode != 1)
                {
                    npc.ai[2] += (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 2f : 1f);
                    if ((double)npc.life < (double)npc.lifeMax * 0.5 || CalamityWorld.bossRushActive)
                    {
                        npc.ai[2] += 1f;
                    }
                    if ((double)npc.life < (double)npc.lifeMax * 0.1 || CalamityWorld.bossRushActive)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (!canAttack && attackMore)
                    {
                        npc.ai[2] += 1f; //4
                    }
                    if (npc.ai[2] > (canAttack ? 24f : 60f))
                    {
                        npc.ai[2] = 0f;
                        Vector2 vector93 = new Vector2(vector.X, vector.Y);
                        float num742 = 10f;
                        if (expertMode)
                        {
                            num742 = 12f;
                        }
                        float num743 = player.position.X + (float)player.width * 0.5f - vector93.X;
                        float num744 = player.position.Y + (float)player.height * 0.5f - vector93.Y;
                        float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));
                        num745 = num742 / num745;
                        num743 *= num745;
                        num744 *= num745;
                        int num746 = expertMode ? 48 : 65; //288 220
                        int num747 = mod.ProjectileType("HolyShot");
                        vector93.X += num743 * 3f;
                        vector93.Y += num744 * 3f;
                        Projectile.NewProjectile(vector93.X, vector93.Y, num743, num744, num747, num746, 0f, Main.myPlayer, 0f, 0f);
                        Projectile.NewProjectile(player.position.X + (float)Main.rand.Next(-1000, 1000), player.position.Y + (float)Main.rand.Next(-100, 100), 0f, 0f, mod.ProjectileType("HolySpear"), num746, 0f, Main.myPlayer, (float)Main.rand.Next(2), 0f);
                        return;
                    }
                }
                if (Main.netMode != 1)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 300f)
                    {
                        npc.ai[0] = -1f;
                    }
                }
            }
            else if (npc.ai[0] == 6f)
            {
                npc.noGravity = true;
                npc.noTileCollide = true;
                npc.chaseable = false;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                if (Main.netMode != 1)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 60f)
                    {
                        int damage = expertMode ? 52 : 70; //288 220
                        Projectile.NewProjectile(player.Center.X, player.Center.Y - 360f, 0f, 0f, mod.ProjectileType("ProvidenceCrystal"), damage, 0f, Main.myPlayer, 0f, 0f);
                        npc.ai[0] = -1f;
                    }
                }
            }
            else if (npc.ai[0] == 7f)
            {
                npc.noGravity = true;
                npc.noTileCollide = true;
                npc.chaseable = false;
                npc.TargetClosest(true);
                npc.velocity *= 0.95f;
                Vector2 value19 = new Vector2(27f, 59f);
                npc.ai[2] += 1f;
                if (npc.ai[2] < 180f)
                {
                    npc.localAI[1] -= 0.05f;
                    if (npc.localAI[1] < 0f)
                    {
                        npc.localAI[1] = 0f;
                    }
                    if (npc.ai[2] >= 60f)
                    {
                        Vector2 center20 = npc.Center;
                        int num1220 = 0;
                        if (npc.ai[2] >= 120f)
                        {
                            num1220 = 1;
                        }
                        int num;
                        for (int num1221 = 0; num1221 < 1 + num1220; num1221 = num + 1)
                        {
                            int num1222 = 244;
                            float num1223 = 0.8f;
                            if (num1221 % 2 == 1)
                            {
                                num1223 = 1.65f;
                            }
                            Vector2 vector199 = center20 + ((float)Main.rand.NextDouble() * 6.28318548f).ToRotationVector2() * value19 / 2f;
                            int num1224 = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, num1222, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default(Color), 1f);
                            Main.dust[num1224].velocity = Vector2.Normalize(center20 - vector199) * 3.5f * (10f - (float)num1220 * 2f) / 10f;
                            Main.dust[num1224].noGravity = true;
                            Main.dust[num1224].scale = num1223;
                            num = num1221;
                        }
                    }
                }
                else if (npc.ai[2] < 360f)
                {
                    if (npc.ai[2] == 180f && Main.netMode != 1)
                    {
                        npc.TargetClosest(false);
                        Vector2 vector200 = player.Center - npc.Center;
                        vector200.Normalize();
                        float num1225 = -1f;
                        if (vector200.X < 0f)
                        {
                            num1225 = 1f;
                        }
                        vector200 = vector200.RotatedBy((double)(-(double)num1225 * 6.28318548f / 6f), default(Vector2));
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 16f, vector200.X, vector200.Y, mod.ProjectileType("ProvidenceHolyRay"), 100, 0f, Main.myPlayer, num1225 * 6.28318548f / 450f, (float)npc.whoAmI);
                        npc.ai[3] = (vector200.ToRotation() + 9.424778f) * num1225; //3.14159265f
                        npc.netUpdate = true;
                    }
                    npc.localAI[1] += 0.05f;
                    if (npc.localAI[1] > 1f)
                    {
                        npc.localAI[1] = 1f;
                    }
                    float num1226 = (float)(npc.ai[3] >= 0f).ToDirectionInt();
                    float num1227 = npc.ai[3];
                    if (num1227 < 0f)
                    {
                        num1227 *= -1f;
                    }
                    num1227 += -9.424778f;
                    num1227 += num1226 * 6.28318548f / 540f;
                    npc.localAI[0] = num1227;
                }
                else
                {
                    npc.localAI[1] -= 0.07f; //15
                    if (npc.localAI[1] < 0f)
                    {
                        npc.localAI[1] = 0f;
                    }
                }
                if (Main.netMode != 1)
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 375f)
                    {
                        npc.ai[0] = -1f;
                    }
                }
            }
            if (npc.ai[0] == -1f) 
			{
				npc.noGravity = true;
				npc.noTileCollide = true;
				npc.chaseable = true;
                phaseChange++;
                if (phaseChange > 14)
                {
                    phaseChange = 0;
                }
                int phase = 0; //0 = blasts 1 = holy fire 2 = shell heal 3 = molten blobs 4 = holy bombs 5 = shell spears 6 = crystal 7 = laser
                if (CalamityWorld.death || CalamityWorld.bossRushActive)
                {
                    switch (phaseChange)
                    {
                        case 0: phase = 4; break;
                        case 1: phase = 5; break;
                        case 2: phase = 0; break;
                        case 3: phase = attackMore ? 6 : 1; break;
                        case 4: phase = 2; break;
                        case 5: phase = 4; break;
                        case 6: phase = 1; break;
                        case 7: phase = 5; break;
                        case 8:
                            phase = attackMore ? 7 : 3;
                            if (attackMore)
                            {
                                npc.TargetClosest(false);
                                Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
                                float num1219 = v3.Length() / 500f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                num1219 = 1f - num1219;
                                num1219 *= 2f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                npc.localAI[0] = v3.ToRotation();
                                npc.localAI[1] = num1219;
                            }
                            break;
                        case 9: phase = 3; break;
                        case 10: phase = 2; break;
                        case 11: phase = 4; break;
                        case 12:
                            phase = attackMore ? 7 : 4;
                            if (attackMore)
                            {
                                npc.TargetClosest(false);
                                Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
                                float num1219 = v3.Length() / 500f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                num1219 = 1f - num1219;
                                num1219 *= 2f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                npc.localAI[0] = v3.ToRotation();
                                npc.localAI[1] = num1219;
                            }
                            break;
                        case 13: phase = 5; break;
                        case 14: phase = 0; break;
                    }
                }
                else
                {
                    switch (phaseChange)
                    {
                        case 0: phase = 0; break;
                        case 1:
                            phase = attackMore ? 7 : 1;
                            if (attackMore)
                            {
                                npc.TargetClosest(false);
                                Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
                                float num1219 = v3.Length() / 500f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                num1219 = 1f - num1219;
                                num1219 *= 2f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                npc.localAI[0] = v3.ToRotation();
                                npc.localAI[1] = num1219;
                            }
                            break;
                        case 2: phase = 3; break;
                        case 3: phase = 4; break;
                        case 4: phase = 5; break;
                        case 5: phase = attackMore ? 6 : 4; break;
                        case 6: phase = 3; break;
                        case 7: phase = 1; break;
                        case 8: phase = 0; break;
                        case 9: phase = 2; break;
                        case 10: phase = 4; break;
                        case 11:
                            phase = attackMore ? 7 : 0;
                            if (attackMore)
                            {
                                npc.TargetClosest(false);
                                Vector2 v3 = player.Center - npc.Center - new Vector2(0f, -22f);
                                float num1219 = v3.Length() / 500f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                num1219 = 1f - num1219;
                                num1219 *= 2f;
                                if (num1219 > 1f)
                                {
                                    num1219 = 1f;
                                }
                                npc.localAI[0] = v3.ToRotation();
                                npc.localAI[1] = num1219;
                            }
                            break;
                        case 12: phase = 3; break;
                        case 13: phase = 1; break;
                        case 14: phase = 5; break;
                    }
                }
				npc.TargetClosest(true);
				if (Math.Abs(npc.Center.X - player.Center.X) > 3000f) 
				{
					phase = 0;
				}
				npc.ai[0] = (float)phase;
				npc.ai[1] = 0f;
				npc.ai[2] = 0f;
				npc.ai[3] = 0f;
				return;
			}
		}
		
		public override void NPCLoot()
		{
			bool isHoly = Main.player[npc.target].ZoneHoly;
			bool isHell = Main.player[npc.target].ZoneUnderworldHeight;
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ProvidenceTrophy"));
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
				if (isHoly)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("ElysianWings"), 1, true);
				}
				if (isHell)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("ElysianAegis"), 1, true);
				}
				npc.DropBossBags();
			}
			else
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("UnholyEssence"), Main.rand.Next(20, 30));
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DivineGeode"), Main.rand.Next(10, 16));
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RuneofCos"));
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ProvidenceMask"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BlissfulBombardier"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HolyCollider"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MoltenAmputator"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PurgeGuzzler"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SolarFlare"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TelluricGlare"));
				}
			}
			if (Main.netMode != 1)
			{
				int num52 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
				int num53 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
				int num54 = npc.width / 2 / 16 + 1;
				for (int num55 = num52 - num54; num55 <= num52 + num54; num55++)
				{
					for (int num56 = num53 - num54; num56 <= num53 + num54; num56++)
					{
						if ((num55 == num52 - num54 || num55 == num52 + num54 || num56 == num53 - num54 || num56 == num53 + num54) && !Main.tile[num55, num56].active())
						{
							Main.tile[num55, num56].type = (ushort)mod.TileType("ProfanedRock");
							Main.tile[num55, num56].active(true);
						}
						Main.tile[num55, num56].lava(false);
						Main.tile[num55, num56].liquid = 0;
						if (Main.netMode == 2)
						{
							NetMessage.SendTileSquare(-1, num55, num56, 1, TileChangeType.None);
						}
						else
						{
							WorldGen.SquareTileFrame(num55, num56, true);
						}
					}
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
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
		
		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
            double newDamage = (damage + (int)((double)defense * 0.25));
            float protection = (((npc.ichor || npc.onFire2) ? 0.2f : 0.25f) +
                    ((npc.ai[0] == 2f || npc.ai[0] == 5f || npc.ai[0] == 7f) ? 0.65f : 0f)); //0.85 or 0.9
            if (CalamityWorld.defiled)
            {
                protection += (1f - protection) * 0.5f;
            }
            if (newDamage < 1.0)
			{
				newDamage = 1.0;
			}
			if (newDamage >= 1.0)
			{
                newDamage = (double)((int)((double)(1f - protection) * newDamage));
				if (newDamage < 1.0)
				{
					newDamage = 1.0;
				}
			}
			damage = newDamage;
			return true;
		}
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = Main.npcTexture[npc.type];
            Texture2D texture2 = mod.GetTexture("NPCs/Providence/ProvidenceAlt");
            Texture2D texture3 = mod.GetTexture("NPCs/Providence/ProvidenceAttack");
            Texture2D texture4 = mod.GetTexture("NPCs/Providence/ProvidenceAttackAlt");
            Texture2D texture5 = mod.GetTexture("NPCs/Providence/ProvidenceDefense");
            Texture2D texture6 = mod.GetTexture("NPCs/Providence/ProvidenceDefenseAlt");
            if (npc.ai[0] == 2f || npc.ai[0] == 5f)
            {
                if (npc.localAI[2] == 0f)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture5, 0, npc, drawColor);
                }
                else
                {
                    CalamityMod.DrawTexture(spriteBatch, texture6, 0, npc, drawColor);
                }
            }
            else
            {
                if (frameUsed == 0)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture, 0, npc, drawColor);
                }
                else if (frameUsed == 1)
                {
                    CalamityMod.DrawTexture(spriteBatch, texture2, 0, npc, drawColor);
                }
                else if (frameUsed == 2)
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
		
		public override void FindFrame(int frameHeight) //9 total frames
		{
            if (npc.ai[0] == 2f || npc.ai[0] == 5f)
            {
                if (npc.localAI[2] == 0f)
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 5.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 3)
                    {
                        npc.frame.Y = 0;
                        npc.localAI[2] = 1f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 5.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 2)
                    {
                        npc.frame.Y = frameHeight * 2;
                    }
                }
            }
            else
            {
                if (npc.localAI[2] > 0f)
                {
                    npc.localAI[2] = 0f;
                    npc.netUpdate = true;
                }
                npc.frameCounter += 1.0;
                if (npc.frameCounter > 5.0)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 3) //6
                {
                    npc.frame.Y = 0;
                    frameUsed++;
                }
                if (frameUsed > 3)
                {
                    frameUsed = 0;
                    npc.netUpdate = true;
                }
            }
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 15; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				float randomSpread = (float)(Main.rand.Next(-50, 50) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence3"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence4"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 400;
				npc.height = 350;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 60; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 90; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}