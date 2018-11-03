using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod;

namespace CalamityMod.NPCs.AstrumDeus
{
	[AutoloadBossHead]
	public class AstrumDeusHead : ModNPC
	{
        private bool flies = false;
        private const int minLength = 4;
        private const int maxLength = 5;
        private int addOrbiter = 0;
        private int addOrbiter2 = 0;
        private float speed = 0.15f;
        private float turnSpeed = 0.1f;
        private bool TailSpawned = false;
        private bool secondStage = false;
        private bool thirdStage = false;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 85; //150
			npc.npcSlots = 5f;
			npc.width = 56; //324
			npc.height = 56; //216
			npc.defense = 30;
            npc.lifeMax = CalamityWorld.revenge ? 10800 : 8000; //250000
            if (CalamityWorld.death)
            {
                npc.lifeMax = 16400;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 700000 : 550000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			npc.value = Item.buyPrice(0, 0, 0, 0);
			npc.alpha = 255;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
        }
		
		public override void AI()
		{
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			int defenseDown = (int)(30f * (1f - (float)npc.life / (float)npc.lifeMax));
			npc.defense = npc.defDefense - defenseDown;
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                if ((npc.life <= npc.lifeMax * 0.9f))
                {
                    if (secondStage == false && Main.netMode != 1)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 3; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 75)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 75)), mod.NPCType("AstrumDeusProbe"), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                        secondStage = true;
                    }
                }
                if ((npc.life <= npc.lifeMax * 0.8f))
                {
                    if (thirdStage == false && Main.netMode != 1)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 5; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), mod.NPCType("AstrumDeusProbe2"), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 72;
                            Eye.ai[3] = I * 72;
                        }
                        thirdStage = true;
                    }
                }
            }
            else
            {
                if ((npc.life <= npc.lifeMax * 0.65f))
                {
                    if (secondStage == false && Main.netMode != 1)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 3; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 120) * 75)), (int)(npc.Center.Y + (Math.Cos(I * 120) * 75)), mod.NPCType("AstrumDeusProbe"), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 120;
                            Eye.ai[3] = I * 120;
                        }
                        secondStage = true;
                    }
                }
                if ((npc.life <= npc.lifeMax * 0.3f))
                {
                    if (thirdStage == false && Main.netMode != 1)
                    {
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
                        for (int I = 0; I < 5; I++)
                        {
                            int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), mod.NPCType("AstrumDeusProbe2"), npc.whoAmI, 0, 0, 0, -1);
                            NPC Eye = Main.npc[FireEye];
                            Eye.ai[0] = I * 72;
                            Eye.ai[3] = I * 72;
                        }
                        thirdStage = true;
                    }
                }
            }
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
			float speedMult = expertMode ? 2f : 1.8f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                speedMult = (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 2.8f : 2.5f);
            }
			float life = (float)npc.life;
			float totalLife = (float)npc.lifeMax;
			float newSpeed = speed * (speedMult - (life / totalLife));
			float newTurnSpeed = turnSpeed * (speedMult - (life / totalLife));
			if (npc.alpha != 0)
			{
				for (int num934 = 0; num934 < 2; num934++)
				{
					int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num935].noGravity = true;
					Main.dust[num935].noLight = true;
				}
			}
			npc.alpha -= 42;
			if (npc.alpha < 0)
			{
				npc.alpha = 0;
			}
			if (Main.netMode != 1)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
	            {
	                int Previous = npc.whoAmI;
	                for (int num36 = 0; num36 < maxLength; num36++)
	                {
	                    int lol = 0;
	                    if (num36 >= 0 && num36 < minLength)
	                    {
	                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusBody"), npc.whoAmI);
	                    }
	                    else
	                    {
	                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusTail"), npc.whoAmI);
	                    }
	                    if (num36 % 2 == 0)
	                    {
	                    	Main.npc[lol].localAI[3] = 1f;
	                    }
	                    Main.npc[lol].realLife = npc.whoAmI;
	                    Main.npc[lol].ai[2] = (float)npc.whoAmI;
	                    Main.npc[lol].ai[1] = (float)Previous;
	                    Main.npc[Previous].ai[0] = (float)lol;
	                    Previous = lol;
	                }
	                TailSpawned = true;
	            }
				if (!npc.active && Main.netMode == 2)
				{
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
			}
			int num180 = (int)(npc.position.X / 16f) - 1;
			int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num182 = (int)(npc.position.Y / 16f) - 1;
			int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num180 < 0)
			{
				num180 = 0;
			}
			if (num181 > Main.maxTilesX)
			{
				num181 = Main.maxTilesX;
			}
			if (num182 < 0)
			{
				num182 = 0;
			}
			if (num183 > Main.maxTilesY)
			{
				num183 = Main.maxTilesY;
			}
			bool flag94 = flies;
			if (!flag94)
			{
				for (int num952 = num180; num952 < num181; num952++)
				{
					for (int num953 = num182; num953 < num183; num953++)
					{
						if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
						{
							Vector2 vector105;
							vector105.X = (float)(num952 * 16);
							vector105.Y = (float)(num953 * 16);
							if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
							{
								flag94 = true;
								break;
							}
						}
					}
				}
			}
			if (!flag94)
			{
				Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
				npc.localAI[1] = 1f;
				Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num954 = (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 200 : 400);
				bool flag95 = true;
				if (npc.position.Y > Main.player[npc.target].position.Y)
				{
					for (int num955 = 0; num955 < 255; num955++)
					{
						if (Main.player[num955].active)
						{
							Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - num954, (int)Main.player[num955].position.Y - num954, num954 * 2, num954 * 2);
							if (rectangle12.Intersects(rectangle13))
							{
								flag95 = false;
								break;
							}
						}
					}
					if (flag95)
					{
						flag94 = true;
					}
				}
			}
			else
			{
				npc.localAI[1] = 0f;
			}
			float maxSpeed = 20f;
			if (Main.dayTime || Main.player[npc.target].dead)
			{
				flag94 = false;
				npc.velocity.Y = npc.velocity.Y + 1f;
				if ((double)npc.position.Y > Main.worldSurface * 16.0)
				{
					npc.velocity.Y = npc.velocity.Y + 1f;
					maxSpeed = 40f;
				}
				if ((double)npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int num957 = 0; num957 < 200; num957++)
					{
						if (Main.npc[num957].aiStyle == npc.aiStyle)
						{
							Main.npc[num957].active = false;
                        }
					}
				}
			}
			float num188 = newSpeed;
			float num189 = newTurnSpeed;
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			num191 = (float)((int)(num191 / 16f) * 16);
			num192 = (float)((int)(num192 / 16f) * 16);
			vector18.X = (float)((int)(vector18.X / 16f) * 16);
			vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
			num191 -= vector18.X;
			num192 -= vector18.Y;
			float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
			if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
			{
				try
				{
					vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
					num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
					num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
				}
				catch
				{
				}
				npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
				num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
				int num194 = npc.width;
				num193 = (num193 - (float)num194) / num193;
				num191 *= num193;
				num192 *= num193;
				npc.velocity = Vector2.Zero;
				npc.position.X = npc.position.X + num191;
				npc.position.Y = npc.position.Y + num192;
			}
			else
			{
				if (!flag94)
				{
					npc.TargetClosest(true);
					npc.velocity.Y = npc.velocity.Y + 0.15f;
					if (npc.velocity.Y > maxSpeed)
					{
						npc.velocity.Y = maxSpeed;
					}
					if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.4)
					{
						if (npc.velocity.X < 0f)
						{
							npc.velocity.X = npc.velocity.X - num188 * 1.1f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X + num188 * 1.1f;
						}
					}
					else if (npc.velocity.Y == maxSpeed)
					{
						if (npc.velocity.X < num191)
						{
							npc.velocity.X = npc.velocity.X + num188;
						}
						else if (npc.velocity.X > num191)
						{
							npc.velocity.X = npc.velocity.X - num188;
						}
					}
					else if (npc.velocity.Y > 4f)
					{
						if (npc.velocity.X < 0f)
						{
							npc.velocity.X = npc.velocity.X + num188 * 0.9f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num188 * 0.9f;
						}
					}
				}
				else
				{
					if (!flies && npc.behindTiles && npc.soundDelay == 0)
					{
						float num195 = num193 / 40f;
						if (num195 < 10f)
						{
							num195 = 10f;
						}
						if (num195 > 20f)
						{
							num195 = 20f;
						}
						npc.soundDelay = (int)num195;
						Main.PlaySound(15, (int)npc.position.X, (int)npc.position.Y, 1);
					}
					num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
					float num196 = System.Math.Abs(num191);
					float num197 = System.Math.Abs(num192);
					float num198 = maxSpeed / num193;
					num191 *= num198;
					num192 *= num198;
					if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
					{
						if (npc.velocity.X < num191)
						{
							npc.velocity.X = npc.velocity.X + num189;
						}
						else if (npc.velocity.X > num191)
						{
							npc.velocity.X = npc.velocity.X - num189;
						}
						if (npc.velocity.Y < num192)
						{
							npc.velocity.Y = npc.velocity.Y + num189;
						}
						else if (npc.velocity.Y > num192)
						{
							npc.velocity.Y = npc.velocity.Y - num189;
						}
					}
					if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
					{
						if (npc.velocity.X < num191)
						{
							npc.velocity.X = npc.velocity.X + num188;
						}
						else
						{
							if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num188;
							}
						}
						if (npc.velocity.Y < num192)
						{
							npc.velocity.Y = npc.velocity.Y + num188;
						}
						else
						{
							if (npc.velocity.Y > num192)
							{
								npc.velocity.Y = npc.velocity.Y - num188;
							}
						}
						if ((double)System.Math.Abs(num192) < (double)maxSpeed * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
						{
							if (npc.velocity.Y > 0f)
							{
								npc.velocity.Y = npc.velocity.Y + num188 * 2f;
							}
							else
							{
								npc.velocity.Y = npc.velocity.Y - num188 * 2f;
							}
						}
						if ((double)System.Math.Abs(num191) < (double)maxSpeed * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
						{
							if (npc.velocity.X > 0f)
							{
								npc.velocity.X = npc.velocity.X + num188 * 2f;
							}
							else
							{
								npc.velocity.X = npc.velocity.X - num188 * 2f;
							}
						}
					}
					else
					{
						if (num196 > num197)
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X = npc.velocity.X + num188 * 1.1f;
							}
							else if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num188 * 1.1f;
							}
							if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
							{
								if (npc.velocity.Y > 0f)
								{
									npc.velocity.Y = npc.velocity.Y + num188;
								}
								else
								{
									npc.velocity.Y = npc.velocity.Y - num188;
								}
							}
						}
						else
						{
							if (npc.velocity.Y < num192)
							{
								npc.velocity.Y = npc.velocity.Y + num188 * 1.1f;
							}
							else if (npc.velocity.Y > num192)
							{
								npc.velocity.Y = npc.velocity.Y - num188 * 1.1f;
							}
							if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)maxSpeed * 0.5)
							{
								if (npc.velocity.X > 0f)
								{
									npc.velocity.X = npc.velocity.X + num188;
								}
								else
								{
									npc.velocity.X = npc.velocity.X - num188;
								}
							}
						}
					}
				}
				npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
				if (flag94)
				{
					if (npc.localAI[0] != 1f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 0f;
				}
				if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				{
					npc.netUpdate = true;
					return;
				}
			}
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 2;
            }
            if (projectile.penetrate == -1 && !projectile.minion && !projectile.thrown)
			{
				damage /= 5;
			}
			else if (projectile.penetrate > 1)
			{
				damage /= projectile.penetrate;
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}

        public override bool PreNPCLoot()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.downedStarGod)
			{
				player.AddBuff(mod.BuffType("GodSlayerInferno"), 150, true);
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}