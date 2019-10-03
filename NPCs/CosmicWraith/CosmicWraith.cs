using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.NPCs.CosmicWraith
{
    [AutoloadBossHead]
	public class CosmicWraith : ModNPC
	{
		private const int CosmicProjectiles = 3;
		private const float CosmicAngleSpread = 170;
		private int CosmicCountdown = 0;
		private int phaseSwitch = 0;
		private int chargeSwitch = 0;
		private int dustTimer = 3;
		private int spawnX = 750;
		private int spawnY = 120;
		private int lifeToAlpha = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Signus, Envoy of the Devourer");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 32f;
			npc.damage = 175;
			npc.width = 130;
			npc.height = 130;
			npc.defense = 70;
			Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
			if (calamityModMusic != null)
				music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
			else
				music = MusicID.Boss4;
			npc.lifeMax = CalamityWorld.revenge ? 109500 : 70000;
			if (CalamityWorld.DoGSecondStageCountdown <= 0)
			{
				npc.value = Item.buyPrice(0, 35, 0, 0);
				if (calamityModMusic != null)
					music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Signus");
				else
					music = MusicID.Boss4;
				npc.lifeMax = CalamityWorld.revenge ? 445500 : 280000;
				if (CalamityWorld.death)
				{
					npc.lifeMax = 722250;
				}
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 2400000 : 2200000;
			}
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.knockBackResist = 0f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
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
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.netAlways = true;
			npc.HitSound = SoundID.NPCHit49;
			npc.DeathSound = SoundID.NPCDeath51;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(CosmicCountdown);
			writer.Write(phaseSwitch);
			writer.Write(chargeSwitch);
			writer.Write(dustTimer);
			writer.Write(spawnX);
			writer.Write(spawnY);
			writer.Write(lifeToAlpha);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			CosmicCountdown = reader.ReadInt32();
			phaseSwitch = reader.ReadInt32();
			chargeSwitch = reader.ReadInt32();
			dustTimer = reader.ReadInt32();
			spawnX = reader.ReadInt32();
			spawnY = reader.ReadInt32();
			lifeToAlpha = reader.ReadInt32();
		}

		public override void AI()
		{
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);

			double lifeRatio = (double)npc.life / (double)npc.lifeMax;
			lifeToAlpha = (int)(100.0 * (1.0 - lifeRatio));

			double mult = 1.0 -
				(revenge ? 0.25 : 0.0) -
				(CalamityWorld.death ? 0.25 : 0.0);

			bool cosmicDust = lifeToAlpha > (int)(15D * mult);
			bool speedBoost = lifeToAlpha > (int)(25D * mult);
			bool cosmicRain = lifeToAlpha > (int)(35D * mult);
			bool cosmicSpeed = lifeToAlpha > (int)(50D * mult);

			Player player = Main.player[npc.target];
			npc.TargetClosest(true);
			Vector2 vectorCenter = npc.Center;
			float lineUpVelocityMult = 8f;
			float lineUpYAdditive = 300f;
			float lineUpMaxChargeDist = 800f;
			float pauseTime = 5f;
			float upwardChangeSpeedMult = 0.75f;
			int num1002 = 0; //dunno wtf this is for. It's never changed from 0
			float pauseVelocityMult = 10f;
			float maxChargeTime = 30f;
			float minChargeDistance = 150f;
			float maxChargeDistance = 10f;
			float velocityChargeAdditive = cosmicSpeed ? 12f : 15f;
			float velocityChargeAdditive2 = 0.333333343f; //same as above but divided by 3?
			float chargeSpeedDividend = cosmicSpeed ? 12f : 15f;
			float chargeSpeedDivisor = cosmicSpeed ? 11.85f : 14.85f;
			velocityChargeAdditive2 *= velocityChargeAdditive;
			if (lifeToAlpha < 50)
			{
				for (int num1011 = 0; num1011 < 2; num1011++)
				{
					if (Main.rand.Next(3) < 1)
					{
						int num1012 = Dust.NewDust(vectorCenter - new Vector2(70f), 70 * 2, 70 * 2, 173, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
						Main.dust[num1012].noGravity = true;
						Main.dust[num1012].velocity *= 0.2f;
						Main.dust[num1012].fadeIn = 1f;
					}
				}
			}
			if (Vector2.Distance(player.Center, vectorCenter) > 6400f)
			{
				CalamityWorld.DoGSecondStageCountdown = 0;
				if (Main.netMode == NetmodeID.Server)
				{
					var netMessage = mod.GetPacket();
					netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
					netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
					netMessage.Send();
				}
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int speed2 = revenge ? 13 : 12;
						if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						{
							speed2 += 3;
						}
						float spawnX2 = Main.rand.Next(1000) - 500 + player.Center.X;
						float spawnY2 = -1000 + player.Center.Y;
						Vector2 baseSpawn = new Vector2(spawnX2, spawnY2);
						Vector2 baseVelocity = player.Center - baseSpawn;
						baseVelocity.Normalize();
						baseVelocity = baseVelocity * speed2;
						int damage = expertMode ? 52 : 65;
						for (int i = 0; i < CosmicProjectiles; i++)
						{
							Vector2 spawn2 = baseSpawn;
							spawn2.X = spawn2.X + i * 30 - (CosmicProjectiles * 15);
							Vector2 velocity = baseVelocity;
							velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-CosmicAngleSpread / 2 + (CosmicAngleSpread * i / (float)CosmicProjectiles)));
							velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
							Projectile.NewProjectile(spawn2.X, spawn2.Y, velocity.X, velocity.Y, mod.ProjectileType("CosmicFlameBurst"), damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}
			if (npc.ai[0] <= 2f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				npc.knockBackResist = 0.05f;
				if (expertMode)
				{
					npc.knockBackResist *= Main.expertKnockBack;
				}
				if (cosmicSpeed)
				{
					npc.knockBackResist = 0f;
				}
				float speed = expertMode ? 14f : 12f;
				if (speedBoost)
				{
					speed = expertMode ? 16f : 14f;
				}
				if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
				{
					speed += 3f;
				}
				npc.velocity = (npc.velocity * 50f + npc.DirectionTo(player.Center) * speed) / 51f;
			}
			else
			{
				npc.knockBackResist = 0f;
			}
			if (npc.ai[0] == 0f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= 120f)
					{
						npc.localAI[1] = 0f;
						npc.TargetClosest(true);
						int counter = 0;
						int playerTileCoordsX;
						int playerTileCoordsY;
						while (true)
						{
							counter++;
							playerTileCoordsX = (int)player.Center.X / 16;
							playerTileCoordsY = (int)player.Center.Y / 16;

							int min = 20;
							int max = 23; //actually 22 because of how randomizers work

							if (Main.rand.NextBool(2))
								playerTileCoordsX += Main.rand.Next(min, max);
							else
								playerTileCoordsX -= Main.rand.Next(min, max);

							if (Main.rand.NextBool(2))
								playerTileCoordsY += Main.rand.Next(min, max);
							else
								playerTileCoordsY -= Main.rand.Next(min, max);

							if (!WorldGen.SolidTile(playerTileCoordsX, playerTileCoordsY))
								break;

							if (counter > 100)
								return;
						}
						npc.ai[0] = 1f;
						npc.ai[1] = (float)playerTileCoordsX;
						npc.ai[2] = (float)playerTileCoordsY;
						npc.netUpdate = true;
						return;
					}
				}
			}
			else if (npc.ai[0] == 1f)
			{
				npc.alpha += 25;
				if (npc.alpha >= 255)
				{
					Main.PlaySound(SoundID.Item8, vectorCenter);
					npc.alpha = 255;
					npc.position.X = npc.ai[1] * 16f - (float)(npc.width / 2);
					npc.position.Y = npc.ai[2] * 16f - (float)(npc.height / 2);
					npc.ai[0] = 2f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 2f)
			{
				npc.alpha -= 25;
				if (npc.alpha <= lifeToAlpha)
				{
					Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 122);
					if (Main.netMode != NetmodeID.MultiplayerClient && revenge)
					{
						int bombIndex = NPC.NewNPC((int)(Main.player[npc.target].position.X + 750f), (int)(Main.player[npc.target].position.Y), mod.NPCType("SignusBomb"), 0, 0f, 0f, 0f, 0f, 255);
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(23, -1, -1, null, bombIndex, 0f, 0f, 0f, 0, 0, 0);
						}
						int bombIndex2 = NPC.NewNPC((int)(Main.player[npc.target].position.X - 750f), (int)(Main.player[npc.target].position.Y), mod.NPCType("SignusBomb"), 0, 0f, 0f, 0f, 0f, 255);
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(23, -1, -1, null, bombIndex2, 0f, 0f, 0f, 0, 0, 0);
						}
						for (int num621 = 0; num621 < 5; num621++)
						{
							int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
							Main.dust[num622].velocity *= 3f;
							Main.dust[num622].noGravity = true;
							if (Main.rand.NextBool(2))
							{
								Main.dust[num622].scale = 0.5f;
								Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
							}
							int num623 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
							Main.dust[num623].velocity *= 3f;
							Main.dust[num623].noGravity = true;
							if (Main.rand.NextBool(2))
							{
								Main.dust[num623].scale = 0.5f;
								Main.dust[num623].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
							}
						}
						for (int num623 = 0; num623 < 20; num623++)
						{
							int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
							Main.dust[num624].noGravity = true;
							Main.dust[num624].velocity *= 5f;
							num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X + 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
							Main.dust[num624].velocity *= 2f;
							int num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
							Main.dust[num625].noGravity = true;
							Main.dust[num625].velocity *= 5f;
							num625 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X - 750f, Main.player[npc.target].position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
							Main.dust[num625].velocity *= 2f;
						}
					}
					npc.ai[3] += 1f;
					npc.alpha = lifeToAlpha;
					if (npc.ai[3] >= 3f)
					{
						npc.ai[0] = 3f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
					else
						npc.ai[0] = 0f;

					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 3f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
				Vector2 vector121 = new Vector2(npc.position.X + (float)(npc.width / 2), npc.position.Y + (float)(npc.height / 2));
				npc.ai[1] += 1f;
				bool flag104 = false;
				if (npc.life < npc.lifeMax / 4 || CalamityWorld.death || CalamityWorld.bossRushActive)
				{
					if (npc.ai[1] % 30f == 29f)
					{
						flag104 = true;
					}
				}
				else if (npc.life < npc.lifeMax / 2)
				{
					if (npc.ai[1] % 35f == 34f)
					{
						flag104 = true;
					}
				}
				else if (npc.ai[1] % 40f == 39f)
				{
					flag104 = true;
				}
				if (flag104)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float scytheSpeed = 15f; //changed from 10
						if (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						{
							scytheSpeed += 3f;
						}
						if (cosmicRain)
						{
							scytheSpeed += 1f; //changed from 3 not a prob
						}
						if (cosmicSpeed)
						{
							scytheSpeed += 1f;
						}
						if (revenge)
						{
							scytheSpeed += 1f;
						}
						if (CalamityWorld.death || CalamityWorld.bossRushActive)
						{
							scytheSpeed += 1f;
						}
						int damage = expertMode ? 48 : 60; //projectile damage
						Projectile.NewProjectile(projectile.Center, npc.DirectionTo(player.Center) mod.ProjectileType("SignusScythe"), damage, 0f, Main.myPlayer, 0f, (float)(npc.target + 1));
					}
				}
				if (npc.position.Y > player.position.Y - 200f) //200
				{
					if (npc.velocity.Y > 0f)
					{
						npc.velocity.Y = npc.velocity.Y * 0.975f;
					}
					npc.velocity.Y = npc.velocity.Y - 0.1f;
					if (npc.velocity.Y > 4f)
					{
						npc.velocity.Y = 4f;
					}
				}
				else if (npc.position.Y < player.position.Y - 400f) //500
				{
					if (npc.velocity.Y < 0f)
					{
						npc.velocity.Y = npc.velocity.Y * 0.975f;
					}
					npc.velocity.Y = npc.velocity.Y + 0.1f;
					if (npc.velocity.Y < -4f)
					{
						npc.velocity.Y = -4f;
					}
				}
				if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 500f) //100
				{
					if (npc.velocity.X > 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.98f;
					}
					npc.velocity.X = npc.velocity.X - 0.1f;
					if (npc.velocity.X > 15f)
					{
						npc.velocity.X = 15f;
					}
				}
				if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 500f) //100
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X * 0.98f;
					}
					npc.velocity.X = npc.velocity.X + 0.1f;
					if (npc.velocity.X < -15f)
					{
						npc.velocity.X = -15f;
					}
				}
				if (npc.ai[1] > 300f)
				{
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
			else if (npc.ai[0] == 4f)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (NPC.CountNPCS(mod.NPCType("CosmicLantern")) < 5)
					{
						for (int x = 0; x < 5; x++)
						{
							int bombIndex = NPC.NewNPC((int)(Main.player[npc.target].position.X + (float)spawnX), (int)(Main.player[npc.target].position.Y + (float)spawnY), mod.NPCType("CosmicLantern"), 0, 0f, 0f, 0f, 0f, 255);
							if (Main.netMode == NetmodeID.Server)
							{
								NetMessage.SendData(23, -1, -1, null, bombIndex, 0f, 0f, 0f, 0, 0, 0);
							}
							int bombIndex2 = NPC.NewNPC((int)(Main.player[npc.target].position.X - (float)spawnX), (int)(Main.player[npc.target].position.Y + (float)spawnY), mod.NPCType("CosmicLantern"), 0, 0f, 0f, 0f, 0f, 255);
							if (Main.netMode == NetmodeID.Server)
							{
								NetMessage.SendData(23, -1, -1, null, bombIndex2, 0f, 0f, 0f, 0, 0, 0);
							}
							spawnY -= 60;
						}
						spawnY = 120;
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
				phaseSwitch += 1;
				if (chargeSwitch == 0) //line up the charge
				{
					Vector2 playerDistVector = player.Center - vectorCenter;
					Vector2 playerDistVectorAdditive = playerDistVector - Vector2.UnitY * lineUpYAdditive;
					float distanceToPlayer = playerDistVector.Length();
					playerDistVector = Vector2.Normalize(playerDistVector) * lineUpVelocityMult;
					playerDistVectorAdditive = Vector2.Normalize(playerDistVectorAdditive) * lineUpVelocityMult;
					bool flag64 = Collision.CanHit(vectorCenter, 1, 1, player.Center, 1, 1);
					if (npc.ai[3] >= 120f)
					{
						flag64 = true;
					}
					float num1014 = 8f;
					flag64 = (flag64 && playerDistVector.ToRotation() > 3.14159274f / num1014 && playerDistVector.ToRotation() < 3.14159274f - 3.14159274f / num1014);
					if (distanceToPlayer > lineUpMaxChargeDist || !flag64)
					{
						npc.velocity.X = (npc.velocity.X * (chargeSpeedDividend - 1f) + playerDistVectorAdditive.X) / chargeSpeedDivisor;
						npc.velocity.Y = (npc.velocity.Y * (chargeSpeedDividend - 1f) + playerDistVectorAdditive.Y) / chargeSpeedDivisor;
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
						chargeSwitch = 1;
						npc.ai[2] = playerDistVector.X;
						npc.ai[3] = playerDistVector.Y;
						npc.netUpdate = true;
					}
				}
				else if (chargeSwitch == 1) //pause before charge
				{
					npc.velocity *= upwardChangeSpeedMult;
					npc.ai[1] += 1f;
					if (npc.ai[1] >= pauseTime)
					{
						chargeSwitch = 2;
						npc.ai[1] = 0f;
						npc.netUpdate = true;
						Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) + new Vector2((float)Main.rand.Next(-num1002, num1002 + 1), (float)Main.rand.Next(-num1002, num1002 + 1)) * 0.04f;
						velocity.Normalize();
						velocity *= pauseVelocityMult;
						npc.velocity = velocity;
					}
				}
				else if (chargeSwitch == 2) //charging
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						dustTimer--;
						if (cosmicDust && dustTimer <= 0)
						{
							Main.PlaySound(SoundID.Item73, npc.position);
							int damage = expertMode ? 60 : 70;
							Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
							int projectile = Projectile.NewProjectile((int)vector173.X, (int)vector173.Y, (float)(npc.direction * 2), 4f, mod.ProjectileType("EssenceDust"), damage, 0f, Main.myPlayer, 0f, 0f);
							Main.projectile[projectile].timeLeft = 60;
							Main.projectile[projectile].velocity.X = 0f;
							Main.projectile[projectile].velocity.Y = 0f;
							dustTimer = 3;
						}
					}
					npc.ai[1] += 1f;
					bool flag65 = Vector2.Distance(vectorCenter, player.Center) > minChargeDistance && vectorCenter.Y > player.Center.Y;
					if ((npc.ai[1] >= maxChargeTime && flag65) || npc.velocity.Length() < maxChargeDistance)
					{
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.velocity /= 2f;
						npc.netUpdate = true;
						npc.ai[1] = 45f;
						chargeSwitch = 3;
					}
					else
					{
						Vector2 vec2 = player.Center - vectorCenter;
						vec2.SafeNormalize(Vector2.UnitX * (float)npc.direction);
						npc.velocity = (npc.velocity * (velocityChargeAdditive - 1f) + vec2 * (npc.velocity.Length() + velocityChargeAdditive2)) / chargeSpeedDivisor;
					}
				}
				else if (chargeSwitch == 3) //slow down after charging and reset
				{
					npc.ai[1] -= 2f;
					if (npc.ai[1] <= 0f)
					{
						chargeSwitch = 0;
						npc.ai[1] = 0f;
						npc.netUpdate = true;
					}
					npc.velocity *= 0.97f;
				}
				if (phaseSwitch > 300)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					chargeSwitch = 0;
					phaseSwitch = 0;
					npc.netUpdate = true;
				}
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
					npc.GetAlpha(drawColor),
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
			Vector2 spriteOutline = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / frameCount / 2));
			Main.spriteBatch.Draw(NPCTexture,
				new Vector2(npc.position.X - Main.screenPosition.X + (float)(npc.width / 2) - (float)Main.npcTexture[npc.type].Width * scale / 2f + spriteOutline.X * scale,
				npc.position.Y - Main.screenPosition.Y + (float)npc.height - (float)Main.npcTexture[npc.type].Height * scale / (float)frameCount + 4f + spriteOutline.Y * scale + 0f + offsetY),
				new Microsoft.Xna.Framework.Rectangle?(frame2),
				npc.GetAlpha(drawColor),
				rotation,
				spriteOutline,
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
            // Only drop items if fought alone
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                // Materials
                DropHelper.DropItem(npc, mod.ItemType("TwistingNether"), true, 2, 3);

                // Weapons
                DropHelper.DropItemChance(npc, mod.ItemType("Cosmilamp"), 3);
                DropHelper.DropItemChance(npc, mod.ItemType("CosmicKunai"), 3);

                // Vanity
                DropHelper.DropItemChance(npc, mod.ItemType("SignusTrophy"), 10);

                // Other
                bool lastSentinelKilled = CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2 && !CalamityWorld.downedSentinel3;
                DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeSentinels"), true, lastSentinelKilled);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel3, 5, 2, 1);
            }

            // If DoG's fight is active, set the timer precisely for DoG phase 2 to spawn
            if (CalamityWorld.DoGSecondStageCountdown > 600)
            {
                CalamityWorld.DoGSecondStageCountdown = 600;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Signus as dead
            CalamityWorld.downedSentinel3 = true;
            CalamityMod.UpdateServerBoolean();
        }

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.85f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
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
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 60; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
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
			player.AddBuff(mod.BuffType("WhisperingDeath"), 420, true);
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 300, true);
			}
		}
	}
}
