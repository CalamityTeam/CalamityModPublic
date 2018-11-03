using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Scavenger
{
    [AutoloadBossHead]
    public class ScavengerBody : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ravager");
			Main.npcFrameCount[npc.type] = 7;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 20f;
			npc.aiStyle = -1;
			npc.damage = 100;
			npc.width = 332; //324
			npc.height = 214; //216
			npc.defense = 80;
			npc.lifeMax = CalamityWorld.revenge ? 51300 : 42700;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 85000;
            }
            npc.knockBackResist = 0f;
			aiType = -1;
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
                npc.buffImmune[mod.BuffType("Shred")] = false;
                npc.buffImmune[mod.BuffType("WhisperingDeath")] = false;
                npc.buffImmune[mod.BuffType("SilvaStun")] = false;
            }
			npc.boss = true;
			npc.alpha = 255;
			npc.value = Item.buyPrice(0, 30, 0, 0);
			npc.HitSound = SoundID.NPCHit41;
			npc.DeathSound = SoundID.NPCDeath14;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Ravager");
            bossBag = mod.ItemType("RavagerBag");
            if (CalamityWorld.downedProvidence)
			{
				npc.defense = 180;
				npc.lifeMax = 350000;
				npc.value = Item.buyPrice(5, 0, 0, 0);
			}
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 3800000 : 3300000;
            }
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
			bool provy = (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			Lighting.AddLight((int)(npc.position.X - 100f) / 16, (int)(npc.position.Y - 20f) / 16, 0f, 0.51f, 2f);
			Lighting.AddLight((int)(npc.position.X + 100f) / 16, (int)(npc.position.Y - 20f) / 16, 0f, 0.51f, 2f);
			CalamityGlobalNPC.scavenger = npc.whoAmI;
			if (npc.localAI[0] == 0f && Main.netMode != 1) 
			{
				npc.localAI[0] = 1f;
				NPC.NewNPC((int)npc.Center.X - 70, (int)npc.Center.Y + 88, mod.NPCType("ScavengerLegLeft"), 0, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X + 70, (int)npc.Center.Y + 88, mod.NPCType("ScavengerLegRight"), 0, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X - 120, (int)npc.Center.Y + 50, mod.NPCType("ScavengerClawLeft"), 0, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X + 120, (int)npc.Center.Y + 50, mod.NPCType("ScavengerClawRight"), 0, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)npc.Center.X + 21, (int)npc.Center.Y - 25, mod.NPCType("ScavengerHead"), 0, 0f, 0f, 0f, 0f, 255);
            }
			if (npc.target >= 0 && Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
				if (Main.player[npc.target].dead)
				{
					npc.noTileCollide = true;
				}
			}
			if (npc.alpha > 0) 
			{
				npc.alpha -= 10;
				if (npc.alpha < 0) 
				{
					npc.alpha = 0;
				}
				npc.ai[1] = 0f;
			}
			bool leftLegActive = false;
			bool rightLegActive = false;
			bool headActive = false;
			bool rightClawActive = false;
			bool leftClawActive = false;
			for (int num619 = 0; num619 < 200; num619++) 
			{
				if (Main.npc[num619].active && Main.npc[num619].type == mod.NPCType("ScavengerHead")) 
				{
					headActive = true;
				}
				if (Main.npc[num619].active && Main.npc[num619].type == mod.NPCType("ScavengerClawRight")) 
				{
					rightClawActive = true;
				}
				if (Main.npc[num619].active && Main.npc[num619].type == mod.NPCType("ScavengerClawLeft")) 
				{
					leftClawActive = true;
				}
				if (Main.npc[num619].active && Main.npc[num619].type == mod.NPCType("ScavengerLegRight")) 
				{
					rightLegActive = true;
				}
				if (Main.npc[num619].active && Main.npc[num619].type == mod.NPCType("ScavengerLegLeft")) 
				{
					leftLegActive = true;
				}
			}
            bool enrage = false;
            if (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) > npc.position.Y + (float)(npc.height / 2) + 10f)
            {
                enrage = true;
            }
            if (headActive || rightClawActive || leftClawActive || rightLegActive || leftLegActive)
			{
				npc.dontTakeDamage = true;
			}
			else
			{
				npc.dontTakeDamage = false;
                if (Main.netMode != 2)
                {
                    if (!Main.player[npc.target].dead && Main.player[npc.target].active)
                    {
                        Main.player[npc.target].AddBuff(mod.BuffType("WeakPetrification"), 2);
                    }
                }
            }
			if (!headActive) 
			{
				int rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 5, 0f, 0f, 100, default(Color), 2.5f);
				Main.dust[rightDust].alpha += Main.rand.Next(100);
				Main.dust[rightDust].velocity *= 0.2f;
				Dust rightDustExpr = Main.dust[rightDust];
				rightDustExpr.velocity.Y = rightDustExpr.velocity.Y - (3f + (float)Main.rand.Next(10) * 0.1f);
				Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;
				if (Main.rand.Next(10) == 0) 
				{
					rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, 6, 0f, 0f, 0, default(Color), 1.5f);
					if (Main.rand.Next(20) != 0) 
					{
						Main.dust[rightDust].noGravity = true;
						Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
						Dust rightDustExpr2 = Main.dust[rightDust];
						rightDustExpr2.velocity.Y = rightDustExpr2.velocity.Y - 4f;
					}
				}
				if (Main.netMode != 1)
				{
					npc.localAI[1] += (enrage ? 2f : 1f);
					if (npc.localAI[1] >= 600f)
					{
						npc.localAI[1] = 0f;
						npc.TargetClosest(true);
						if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
						{
							Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
							float spread = 45f * 0.0174f;
					    	double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
					    	double deltaAngle = spread / 8f;
					    	double offsetAngle;
					    	int i;
					    	int laserDamage = expertMode ? 34 : 48;
					    	for (i = 0; i < 4; i++ )
					    	{
					   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					        	Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)( Math.Sin(offsetAngle) * 7f ), (float)( Math.Cos(offsetAngle) * 7f ), 259, laserDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					        	Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)( -Math.Sin(offsetAngle) * 7f ), (float)( -Math.Cos(offsetAngle) * 7f ), 259, laserDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					    	}
						}
					}
		       	}
			}
			if (!rightClawActive) 
			{
				int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default(Color), 3f);
				Main.dust[rightDust].alpha += Main.rand.Next(100);
				Main.dust[rightDust].velocity *= 0.2f;
				Dust rightDustExpr = Main.dust[rightDust];
				rightDustExpr.velocity.X = rightDustExpr.velocity.X + (3f + (float)Main.rand.Next(10) * 0.1f);
				Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;
				if (Main.rand.Next(10) == 0) 
				{
					rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default(Color), 2f);
					if (Main.rand.Next(20) != 0) 
					{
						Main.dust[rightDust].noGravity = true;
						Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
						Dust rightDustExpr2 = Main.dust[rightDust];
						rightDustExpr2.velocity.X = rightDustExpr2.velocity.X + 4f;
					}
				}
				if (Main.netMode != 1)
				{
					npc.localAI[2] += (enrage ? 2f : 1f);
					if (npc.localAI[2] >= 480f)
					{
						Main.PlaySound(SoundID.Item20, npc.position);
						npc.localAI[2] = 0f;
						Vector2 shootFromVector = new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f);
                        int damage = expertMode ? 28 : 40;
                        int laser = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 12f, 0f, 258, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
			if (!leftClawActive) 
			{
				int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 5, 0f, 0f, 100, default(Color), 3f);
				Main.dust[leftDust].alpha += Main.rand.Next(100);
				Main.dust[leftDust].velocity *= 0.2f;
				Dust leftDustExpr = Main.dust[leftDust];
				leftDustExpr.velocity.X = leftDustExpr.velocity.X - (3f + (float)Main.rand.Next(10) * 0.1f);
				Main.dust[leftDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;
				if (Main.rand.Next(10) == 0)
				{
					leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, 6, 0f, 0f, 0, default(Color), 2f);
					if (Main.rand.Next(20) != 0)
					{
						Main.dust[leftDust].noGravity = true;
						Main.dust[leftDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
						Dust leftDustExpr2 = Main.dust[leftDust];
						leftDustExpr2.velocity.X = leftDustExpr2.velocity.X - 4f;
					}
				}
				if (Main.netMode != 1)
				{
					npc.localAI[3] += (enrage ? 2f : 1f);
					if (npc.localAI[3] >= 480f)
					{
						Main.PlaySound(SoundID.Item20, npc.position);
						npc.localAI[3] = 0f;
						Vector2 shootFromVector = new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f);
                        int damage = expertMode ? 28 : 40;
                        int laser = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, -12f, 0f, 258, damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
					}
				}
			}
			if (!rightLegActive) 
			{
				int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default(Color), 2f);
				Main.dust[rightDust].alpha += Main.rand.Next(100);
				Main.dust[rightDust].velocity *= 0.2f;
				Dust rightDustExpr = Main.dust[rightDust];
				rightDustExpr.velocity.Y = rightDustExpr.velocity.Y + (0.5f + (float)Main.rand.Next(10) * 0.1f);
				Main.dust[rightDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;
				if (Main.rand.Next(10) == 0) 
				{
					rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default(Color), 1.5f);
					if (Main.rand.Next(20) != 0) 
					{
						Main.dust[rightDust].noGravity = true;
						Main.dust[rightDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
						Dust rightDustExpr2 = Main.dust[rightDust];
						rightDustExpr2.velocity.Y = rightDustExpr2.velocity.Y + 1f;
					}
				}
				if (Main.netMode != 1)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= 300f)
					{
						npc.ai[2] = 0f;
						Vector2 shootFromVector = new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f);
                        int damage = 0;
                        int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, 326 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[fire].timeLeft = 210;
					}
				}
			}
			if (!leftLegActive) 
			{
				int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 5, 0f, 0f, 100, default(Color), 2f);
				Main.dust[leftDust].alpha += Main.rand.Next(100);
				Main.dust[leftDust].velocity *= 0.2f;
				Dust leftDustExpr = Main.dust[leftDust];
				leftDustExpr.velocity.Y = leftDustExpr.velocity.Y + (0.5f + (float)Main.rand.Next(10) * 0.1f);
				Main.dust[leftDust].fadeIn = 0.5f + (float)Main.rand.Next(10) * 0.1f;
				if (Main.rand.Next(10) == 0) 
				{
					leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, 6, 0f, 0f, 0, default(Color), 1.5f);
					if (Main.rand.Next(20) != 0) 
					{
						Main.dust[leftDust].noGravity = true;
						Main.dust[leftDust].scale *= 1f + (float)Main.rand.Next(10) * 0.1f;
						Dust leftDustExpr2 = Main.dust[leftDust];
						leftDustExpr2.velocity.Y = leftDustExpr2.velocity.Y + 1f;
					}
				}
				if (Main.netMode != 1)
				{
					npc.ai[3] += 1f;
					if (npc.ai[3] >= 300f)
					{
						npc.ai[3] = 0f;
						Vector2 shootFromVector = new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f);
                        int damage = 0;
                        int fire = Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, 0f, 2f, 326 + Main.rand.Next(3), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[fire].timeLeft = 210;
					}
				}
			}
			if (npc.ai[0] == 0f) 
			{
				npc.noTileCollide = false;
				if (npc.velocity.Y == 0f) 
				{
					npc.velocity.X = npc.velocity.X * 0.8f;
					npc.ai[1] += 1f;
					if (npc.ai[1] > 0f) 
					{
						if ((!rightClawActive && !leftClawActive) || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
						{
							npc.ai[1] += 1f;
						}
						if (!headActive || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
						{
							npc.ai[1] += 1f;
						}
						if ((!rightLegActive && !leftLegActive) || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
						{
							npc.ai[1] += 1f;
						}
					}
					if (npc.ai[1] >= 300f) 
					{
						npc.ai[1] = -20f;
					} 
					else if (npc.ai[1] == -1f)
					{
						npc.TargetClosest(true);
                        int speedXMult = ((enrage || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged) ? 8 : 4);
						npc.velocity.X = (float)(speedXMult * npc.direction);
						npc.velocity.Y = -15.2f;
						npc.ai[0] = 1f;
						npc.ai[1] = 0f;
					}
				}
			} 
			else if (npc.ai[0] == 1f) 
			{
				if (npc.velocity.Y == 0f) 
				{
					Main.PlaySound(SoundID.Item14, npc.position);
					npc.ai[0] = 0f;
                    if (Main.netMode != 1)
                    {
                        if (NPC.CountNPCS(mod.NPCType("RockPillar")) < 2)
                        {
                            NPC.NewNPC((int)npc.Center.X - 360, (int)npc.Center.Y - 10, mod.NPCType("RockPillar"), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)npc.Center.X + 360, (int)npc.Center.Y - 10, mod.NPCType("RockPillar"), 0, 0f, 0f, 0f, 0f, 255);
                        }
                        if (NPC.CountNPCS(mod.NPCType("FlamePillar")) < 2)
                        {
                            NPC.NewNPC((int)Main.player[npc.target].Center.X - 180, (int)Main.player[npc.target].Center.Y - 10, mod.NPCType("FlamePillar"), 0, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC((int)Main.player[npc.target].Center.X + 180, (int)Main.player[npc.target].Center.Y - 10, mod.NPCType("FlamePillar"), 0, 0f, 0f, 0f, 0f, 255);
                        }
                    }
                    for (int stompDustArea = (int)npc.position.X - 30; stompDustArea < (int)npc.position.X + npc.width + 60; stompDustArea += 30) 
					{
						for (int stompDustAmount = 0; stompDustAmount < 6; stompDustAmount++) 
						{
							int stompDust = Dust.NewDust(new Vector2(npc.position.X - 30f, npc.position.Y + (float)npc.height), npc.width + 30, 4, 31, 0f, 0f, 100, default(Color), 1.5f);
							Main.dust[stompDust].velocity *= 0.2f;
						}
						int stompGore = Gore.NewGore(new Vector2((float)(stompDustArea - 30), npc.position.Y + (float)npc.height - 12f), default(Vector2), Main.rand.Next(61, 64), 1f);
						Main.gore[stompGore].velocity *= 0.4f;
					}
				} 
				else 
				{
					npc.TargetClosest(true);
					if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width) 
					{
						npc.velocity.X = npc.velocity.X * 0.9f;
						npc.velocity.Y = npc.velocity.Y + 0.2f;
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
						float velocityX = 3f;
                        if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged)
                        {
                            velocityX += 3f;
                        }
						if (!rightClawActive) 
						{
							velocityX += 1f;
						}
						if (!leftClawActive) 
						{
							velocityX += 1f;
						}
						if (!headActive) 
						{
							velocityX += 1f;
						}
						if (!rightLegActive) 
						{
							velocityX += 1f;
						}
						if (!leftLegActive) 
						{
							velocityX += 1f;
						}
						if (npc.velocity.X < -velocityX) 
						{
							npc.velocity.X = -velocityX;
						}
						if (npc.velocity.X > velocityX) 
						{
							npc.velocity.X = velocityX;
						}
					}
				}
			}
			if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			int distanceFromTarget = 3000;
			if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget) 
			{
				npc.TargetClosest(true);
				if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget) 
				{
					npc.active = false;
                    npc.netUpdate = true;
					return;
				}
			}
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 4;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)mod.GetTexture("NPCs/Scavenger/ScavengerBodyGlow").Width, (float)(mod.GetTexture("NPCs/Scavenger/ScavengerBodyGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Blue);
            Main.spriteBatch.Draw(mod.GetTexture("NPCs/Scavenger/ScavengerBodyGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
			Microsoft.Xna.Framework.Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/Scavenger/ScavengerLegRight"), new Vector2(center.X - Main.screenPosition.X + 28f, center.Y - Main.screenPosition.Y + 20f), //72 
			    new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/Scavenger/ScavengerLegRight").Width, mod.GetTexture("NPCs/Scavenger/ScavengerLegRight").Height)), 
			    color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(mod.GetTexture("NPCs/Scavenger/ScavengerLegLeft"), new Vector2(center.X - Main.screenPosition.X - 112f, center.Y - Main.screenPosition.Y + 20f), //72
			    new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/Scavenger/ScavengerLegLeft").Width, mod.GetTexture("NPCs/Scavenger/ScavengerLegLeft").Height)), 
			    color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            if (NPC.CountNPCS(mod.NPCType("ScavengerHead")) > 0)
            {
                Main.spriteBatch.Draw(mod.GetTexture("NPCs/Scavenger/ScavengerHead"), new Vector2(center.X - Main.screenPosition.X - 70f, center.Y - Main.screenPosition.Y - 75f),
                    new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/Scavenger/ScavengerHead").Width, mod.GetTexture("NPCs/Scavenger/ScavengerHead").Height)),
                    color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
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
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 2f);
				Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 2f);
					Dust.NewDust(npc.position, npc.width, npc.height, 6, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			name = "Ravager";
			potionType = ItemID.GreaterHealingPotion;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 600, true);
			}
		}
		
		public override void NPCLoot()
		{
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
                if (CalamityWorld.downedProvidence)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Bloodstone"), Main.rand.Next(50, 61));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VerstaltiteBar"), Main.rand.Next(5, 11));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DraedonBar"), Main.rand.Next(5, 11));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CruptixBar"), Main.rand.Next(5, 11));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofCinder"), Main.rand.Next(1, 4));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofEleum"), Main.rand.Next(1, 4));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofChaos"), Main.rand.Next(1, 4));
                    if (Main.rand.Next(2) == 0)
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BarofLife"));
                    }
                    if (Main.rand.Next(3) == 0)
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofCalamity"));
                    }
                }
                else
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VerstaltiteBar"), Main.rand.Next(1, 4));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DraedonBar"), Main.rand.Next(1, 4));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CruptixBar"), Main.rand.Next(1, 4));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofCinder"), Main.rand.Next(1, 3));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofEleum"), Main.rand.Next(1, 3));
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CoreofChaos"), Main.rand.Next(1, 3));
                }
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodPact"));
                }
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FleshTotem"));
                }
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Hematemesis"));
                }
            }
        }
	}
}