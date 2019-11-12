using CalamityMod.Dusts;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityAI
    {
		#region Brimstone Elemental
		public static void BrimstoneElementalAI(NPC npc, Mod mod)
		{
			// Used for Brimling AI states
			CalamityGlobalNPC.brimstoneElemental = npc.whoAmI;

			// Emit light
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 2f, 0f, 0f);

			// Get a target
			npc.TargetClosest(true);

			// Target variables
			Player player = Main.player[npc.target];
			CalamityPlayer modPlayer = player.Calamity();

			// Variables for buffing the AI
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool calamity = modPlayer.ZoneCalamity;

			// Reset defense
			npc.defense = npc.defDefense;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Center
			Vector2 vectorCenter = npc.Center;

			// Emit dust
			int dustAmt = (npc.ai[0] == 2f) ? 2 : 1;
			int size = (npc.ai[0] == 2f) ? 50 : 35;
			if (npc.ai[0] != 1f)
			{
				for (int num1011 = 0; num1011 < 2; num1011++)
				{
					if (Main.rand.Next(3) < dustAmt)
					{
						int dust = Dust.NewDust(vectorCenter - new Vector2((float)size), size * 2, size * 2, 235, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.2f;
						Main.dust[dust].fadeIn = 1f;
					}
				}
			}

			// Despawn if target is too far away
			if (Vector2.Distance(player.Center, vectorCenter) > 5600f)
			{
				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}
			else if (npc.timeLeft > 1800)
				npc.timeLeft = 1800;

			// Speed while moving in phase 1
			float speed = expertMode ? 5f : 4.5f;
			if (CalamityWorld.bossRushActive)
				speed = 12f;
			else if (!calamity)
				speed = 7f;
			else if (CalamityWorld.death)
				speed = 6f;
			else if (revenge)
				speed = 5.5f;
			speed += 2f * (1f - lifeRatio);

			// Variables for target location relative to npc location
			float xDistance = player.Center.X - vectorCenter.X;
			float yDistance = player.Center.Y - vectorCenter.Y;
			float totalDistance = (float)Math.Sqrt((double)(xDistance * xDistance + yDistance * yDistance));

			// Static movement towards target
			if (npc.ai[0] <= 2f)
			{
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
				totalDistance = speed / totalDistance;
				xDistance *= totalDistance;
				yDistance *= totalDistance;
				npc.velocity.X = (npc.velocity.X * 50f + xDistance) / 51f;
				npc.velocity.Y = (npc.velocity.Y * 50f + yDistance) / 51f;
			}

			// Pick a location to teleport to
			if (npc.ai[0] == 0f)
			{
				npc.chaseable = true;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (npc.localAI[1] >= (CalamityWorld.bossRushActive ? 90f : 180f))
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

							int min = 12;
							int max = 16;

							if (Main.rand.NextBool(2))
								playerPosX += Main.rand.Next(min, max);
							else
								playerPosX -= Main.rand.Next(min, max);

							if (Main.rand.NextBool(2))
								playerPosY += Main.rand.Next(min, max);
							else
								playerPosY -= Main.rand.Next(min, max);

							if (!WorldGen.SolidTile(playerPosX, playerPosY))
								break;

							if (timer > 100)
								return;
						}
						npc.ai[0] = 1f;
						npc.ai[1] = (float)playerPosX;
						npc.ai[2] = (float)playerPosY;
						npc.netUpdate = true;
					}
				}
			}

			// Teleport to location
			else if (npc.ai[0] == 1f)
			{
				npc.chaseable = true;
				Vector2 position = new Vector2(npc.ai[1] * 16f - (float)(npc.width / 2), npc.ai[2] * 16f - (float)(npc.height / 2));
				for (int m = 0; m < 5; m++)
				{
					int dust = Dust.NewDust(position, npc.width, npc.height, 235, 0f, -1f, 90, default, 2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].fadeIn = 1f;
				}
				npc.alpha += 2;
				if (npc.alpha >= 255)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(ModContent.NPCType<Brimling>()) < 2 && revenge)
					{
						NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<Brimling>(), 0, 0f, 0f, 0f, 0f, 255);
					}
					Main.PlaySound(SoundID.Item8, vectorCenter);
					npc.alpha = 255;
					npc.position = position;
					for (int n = 0; n < 15; n++)
					{
						int warpDust = Dust.NewDust(npc.position, npc.width, npc.height, 235, 0f, -1f, 90, default, 3f);
						Main.dust[warpDust].noGravity = true;
					}
					npc.ai[0] = 2f;
					npc.netUpdate = true;
				}
			}

			// Either teleport again or go to next AI state
			else if (npc.ai[0] == 2f)
			{
				npc.alpha -= 50;
				if (npc.alpha <= 0)
				{
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
				}
			}

			// Float above target and fire projectiles
			else if (npc.ai[0] == 3f)
			{
				npc.chaseable = true;
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
				npc.ai[1] += 1f;

				bool shootProjectile = false;
				if (lifeRatio < 0.1f || CalamityWorld.bossRushActive)
				{
					if (npc.ai[1] % 30f == 29f)
						shootProjectile = true;
				}
				else if (lifeRatio < 0.5f || CalamityWorld.death)
				{
					if (npc.ai[1] % 35f == 34f)
						shootProjectile = true;
				}
				else if (npc.ai[1] % 40f == 39f)
					shootProjectile = true;

				if (shootProjectile)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						float projectileSpeed = CalamityWorld.bossRushActive ? 7f : 5f;
						if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
							projectileSpeed += 4f;
						if (revenge)
							projectileSpeed += 1f;
						if (!calamity)
							projectileSpeed += 2f;
						projectileSpeed += 3f * (1f - lifeRatio);

						float num742 = CalamityWorld.bossRushActive ? 6f : 4f;
						float num743 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vectorCenter.X;
						float num744 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vectorCenter.Y;
						float num745 = (float)Math.Sqrt((double)(num743 * num743 + num744 * num744));

						num745 = num742 / num745;
						num743 *= num745;
						num744 *= num745;
						vectorCenter.X += num743 * 3f;
						vectorCenter.Y += num744 * 3f;

						int damage = expertMode ? 25 : 30;
						int numProj = 4;
						int spread = 45;
						float rotation = MathHelper.ToRadians(spread);
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / (float)numProj;
						double offsetAngle;

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), ModContent.ProjectileType<BrimstoneBarrage>(), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
						}

						vectorCenter = npc.Center;
						float relativeSpeedX = player.position.X + (float)player.width * 0.5f - vectorCenter.X;
						float relativeSpeedY = player.position.Y + (float)player.height * 0.5f - vectorCenter.Y;
						float totalRelativeSpeed = (float)Math.Sqrt((double)(relativeSpeedX * relativeSpeedX + relativeSpeedY * relativeSpeedY));
						totalRelativeSpeed = projectileSpeed / totalRelativeSpeed;
						relativeSpeedX *= totalRelativeSpeed;
						relativeSpeedY *= totalRelativeSpeed;
						vectorCenter.X += relativeSpeedX * 3f;
						vectorCenter.Y += relativeSpeedY * 3f;
						int projectileDamage = expertMode ? 28 : 35;
						int projectileType = ModContent.ProjectileType<BrimstoneHellfireball>();
						int projectileShot = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, relativeSpeedX, relativeSpeedY, projectileType, projectileDamage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						Main.projectile[projectileShot].timeLeft = 240;
					}
				}

				if (npc.position.Y > player.position.Y - 150f) //200
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
				}
				else if (npc.position.Y < player.position.Y - 350f) //500
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y = npc.velocity.Y * 0.98f;

					npc.velocity.Y = npc.velocity.Y + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
				}
				if (npc.position.X + (float)(npc.width / 2) > player.position.X + (float)(player.width / 2) + 150f) //100
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X = npc.velocity.X * 0.985f;

					npc.velocity.X = npc.velocity.X - (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X > 8f)
						npc.velocity.X = 8f;
				}
				if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)(player.width / 2) - 150f) //100
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X = npc.velocity.X * 0.985f;

					npc.velocity.X = npc.velocity.X + (CalamityWorld.bossRushActive ? 0.15f : 0.1f);

					if (npc.velocity.X < -8f)
						npc.velocity.X = -8f;
				}

				if (npc.ai[1] >= 300f)
				{
					npc.ai[0] = 4f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}

			// Cocoon bullet hell
			else if (npc.ai[0] == 4f)
			{
				npc.defense = 99999;
				npc.chaseable = false;
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[0] += 1f + 2f * (1f - lifeRatio);
					if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
						npc.localAI[0] += 2f;
					if (CalamityWorld.death || !calamity)
						npc.localAI[0] += 1f;

					if (npc.localAI[0] >= 120f)
					{
						npc.localAI[0] = 0f;

						npc.TargetClosest(true);

						float projectileSpeed = revenge ? 8f : 6f;
						if (CalamityWorld.bossRushActive)
							projectileSpeed = 12f;

						float num180 = player.position.X + (float)player.width * 0.5f - vectorCenter.X;
						float num181 = Math.Abs(num180) * 0.1f;
						float num182 = player.position.Y + (float)player.height * 0.5f - vectorCenter.Y - num181;
						float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
						npc.netUpdate = true;
						num183 = projectileSpeed / num183;
						num180 *= num183;
						num182 *= num183;

						int num184 = expertMode ? 25 : 30;
						int num185 = ModContent.ProjectileType<BrimstoneHellblast>();
						vectorCenter.X += num180;
						vectorCenter.Y += num182;

						for (int num186 = 0; num186 < 6; num186++)
						{
							num180 = player.position.X + (float)player.width * 0.5f - vectorCenter.X;
							num182 = player.position.Y + (float)player.height * 0.5f - vectorCenter.Y;
							num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
							num183 = projectileSpeed / num183;
							num180 += (float)Main.rand.Next(-80, 81);
							num182 += (float)Main.rand.Next(-80, 81);
							num180 *= num183;
							num182 *= num183;
							int projectile = Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, num180, num182, num185, num184 + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
							Main.projectile[projectile].timeLeft = 300;
							Main.projectile[projectile].tileCollide = false;
						}

						vectorCenter = npc.Center;
						int totalProjectiles = 12;
						float spread = MathHelper.ToRadians(30); // 30 degrees in radians = 0.523599
						double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2; // Where the projectiles start spawning at, don't change this
						double deltaAngle = spread / (float)totalProjectiles; // Angle between each projectile, 0.04363325
						double offsetAngle;
						float velocity = CalamityWorld.bossRushActive ? 9f : 6f;

						int i;
						for (i = 0; i < 6; i++)
						{
							offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i; // Used to be 32
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, (float)(Math.Sin(offsetAngle) * velocity), (float)(Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<BrimstoneBarrage>(), num184 + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
							Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, (float)(-Math.Sin(offsetAngle) * velocity), (float)(-Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<BrimstoneBarrage>(), num184 + (provy ? 30 : 0), 0f, Main.myPlayer, 1f, 0f);
						}
					}
				}

				npc.TargetClosest(true);

				npc.velocity *= 0.95f;
				npc.rotation = npc.velocity.X * 0.04f;
				npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

				npc.ai[1] += 1f;
				if (npc.ai[1] >= 300f)
				{
					npc.ai[0] = 0f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		#region Calamitas Clone
		public static void CalamitasCloneAI(NPC npc, Mod mod, bool phase2)
		{
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Spawn phase 2 Cal
			if (lifeRatio <= 0.75f && Main.netMode != NetmodeID.MultiplayerClient && !phase2)
			{
				NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun3>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				string key = "Mods.CalamityMod.CalamitasBossText";
				Color messageColor = Color.Orange;
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			// Variables for increasing difficulty
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool dayTime = Main.dayTime && !CalamityWorld.bossRushActive;
			bool provy = CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive;

			// Variable for live brothers
			bool brotherAlive = false;

			if (phase2)
			{
				// For seekers
				CalamityGlobalNPC.calamitas = npc.whoAmI;

				// Seeker ring
				if (calamityGlobalNPC.newAI[1] == 0f && lifeRatio <= 0.5f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 74);
						for (int I = 0; I < 5; I++)
						{
							int FireEye = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(I * 72) * 150)), (int)(npc.Center.Y + (Math.Cos(I * 72) * 150)), ModContent.NPCType<SoulSeeker>(), npc.whoAmI, 0, 0, 0, -1);
							NPC Eye = Main.npc[FireEye];
							Eye.ai[0] = I * 72;
							Eye.ai[3] = I * 72;
						}
					}

					string key = "Mods.CalamityMod.CalamitasBossText3";
					Color messageColor = Color.Orange;
					if (Main.netMode == NetmodeID.SinglePlayer)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == NetmodeID.Server)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

					calamityGlobalNPC.newAI[1] = 1f;
				}

				// Spawn brothers
				if (calamityGlobalNPC.newAI[0] == 0f && npc.life > 0)
					calamityGlobalNPC.newAI[0] = (float)npc.lifeMax;

				if (npc.life > 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num660 = (int)((double)npc.lifeMax * 0.3); //70%, 40%, and 10%
						if ((float)(npc.life + num660) < calamityGlobalNPC.newAI[0])
						{
							calamityGlobalNPC.newAI[0] = (float)npc.life;
							if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.1)
							{
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);

								string key = "Mods.CalamityMod.CalamitasBossText2";
								Color messageColor = Color.Orange;
								if (Main.netMode == NetmodeID.SinglePlayer)
									Main.NewText(Language.GetTextValue(key), messageColor);
								else if (Main.netMode == NetmodeID.Server)
									NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
							}
							else if (calamityGlobalNPC.newAI[0] <= (float)npc.lifeMax * 0.4)
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
							else
								NPC.NewNPC((int)npc.Center.X, (int)npc.position.Y + npc.height, ModContent.NPCType<CalamitasRun>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
						}
					}
				}

				// Huge defense boost if brothers are alive
				int num568 = 0;
				if (expertMode)
				{
					if (CalamityGlobalNPC.cataclysm != -1)
					{
						if (Main.npc[CalamityGlobalNPC.cataclysm].active)
						{
							brotherAlive = true;
							num568 += 255;
						}
					}
					if (CalamityGlobalNPC.catastrophe != -1)
					{
						if (Main.npc[CalamityGlobalNPC.catastrophe].active)
						{
							brotherAlive = true;
							num568 += 255;
						}
					}
					npc.defense += num568 * 50;
					if (!brotherAlive)
						npc.defense = provy ? 150 : 25;
				}

				// Disable homing if brothers are alive
				npc.chaseable = !brotherAlive;
			}

			// Get a target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest(true);

			// Target variable
			Player player = Main.player[npc.target];

			// Rotation
			float num801 = npc.position.X + (float)(npc.width / 2) - player.position.X - (float)(player.width / 2);
			float num802 = npc.position.Y + (float)npc.height - 59f - player.position.Y - (float)(player.height / 2);
			float num803 = (float)Math.Atan2((double)num802, (double)num801) + 1.57f;
			if (num803 < 0f)
				num803 += 6.283f;
			else if ((double)num803 > 6.283)
				num803 -= 6.283f;

			float num804 = 0.1f;
			if (npc.rotation < num803)
			{
				if ((double)(num803 - npc.rotation) > 3.1415)
					npc.rotation -= num804;
				else
					npc.rotation += num804;
			}
			else if (npc.rotation > num803)
			{
				if ((double)(npc.rotation - num803) > 3.1415)
					npc.rotation += num804;
				else
					npc.rotation -= num804;
			}

			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;
			if (npc.rotation < 0f)
				npc.rotation += 6.283f;
			else if ((double)npc.rotation > 6.283)
				npc.rotation -= 6.283f;
			if (npc.rotation > num803 - num804 && npc.rotation < num803 + num804)
				npc.rotation = num803;

			// Despawn
			if (!player.active || player.dead || (dayTime && !Main.eclipse))
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || (dayTime && !Main.eclipse))
				{
					npc.velocity = new Vector2(0f, -10f);
					if (npc.timeLeft > 150)
						npc.timeLeft = 150;

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			// Float above target and fire lasers or fireballs
			if (npc.ai[1] == 0f)
			{
				float num823 = expertMode ? 9.5f : 8f;
				float num824 = expertMode ? 0.175f : 0.15f;
				if (phase2)
				{
					num823 = expertMode ? 10f : 8.5f;
					num824 = expertMode ? 0.18f : 0.155f;
				}
				if (provy)
				{
					num823 *= 1.25f;
					num824 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num823 *= 1.5f;
					num824 *= 1.5f;
				}

				Vector2 vector82 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num825 = player.position.X + (float)(player.width / 2) - vector82.X;
				float num826 = player.position.Y + (float)(player.height / 2) - ((CalamityWorld.bossRushActive ? 400f : 300f) + (phase2 ? 60f : 0f)) - vector82.Y;
				float num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
				num827 = num823 / num827;
				num825 *= num827;
				num826 *= num827;

				if (npc.velocity.X < num825)
				{
					npc.velocity.X = npc.velocity.X + num824;
					if (npc.velocity.X < 0f && num825 > 0f)
						npc.velocity.X = npc.velocity.X + num824;
				}
				else if (npc.velocity.X > num825)
				{
					npc.velocity.X = npc.velocity.X - num824;
					if (npc.velocity.X > 0f && num825 < 0f)
						npc.velocity.X = npc.velocity.X - num824;
				}
				if (npc.velocity.Y < num826)
				{
					npc.velocity.Y = npc.velocity.Y + num824;
					if (npc.velocity.Y < 0f && num826 > 0f)
						npc.velocity.Y = npc.velocity.Y + num824;
				}
				else if (npc.velocity.Y > num826)
				{
					npc.velocity.Y = npc.velocity.Y - num824;
					if (npc.velocity.Y > 0f && num826 < 0f)
						npc.velocity.Y = npc.velocity.Y - num824;
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (phase2 ? 200f : 300f))
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

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (phase2)
					{
						if (!brotherAlive)
						{
							npc.localAI[1] += 1f * (1f - lifeRatio);
							if (revenge)
								npc.localAI[1] += 0.5f;
							if (CalamityWorld.death || CalamityWorld.bossRushActive)
								npc.localAI[1] += 0.5f;
						}

						if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num828 = CalamityWorld.bossRushActive ? 16f : (expertMode ? 14f : 12.5f);
							if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
								num828 += 5f;

							int num829 = expertMode ? 34 : 42;
							int num830 = ModContent.ProjectileType<BrimstoneHellfireball>();
							num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
							num827 = num828 / num827;
							num825 *= num827;
							num826 *= num827;
							vector82.X += num825 * 12f;
							vector82.Y += num826 * 12f;
							Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
					else
					{
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] > 180f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num828 = CalamityWorld.bossRushActive ? 16f : (expertMode ? 13f : 10.5f);
							int num829 = expertMode ? 28 : 35;
							int num830 = ModContent.ProjectileType<BrimstoneLaser>();
							num827 = (float)Math.Sqrt((double)(num825 * num825 + num826 * num826));
							num827 = num828 / num827;
							num825 *= num827;
							num826 *= num827;
							vector82.X += num825 * 12f;
							vector82.Y += num826 * 12f;
							Projectile.NewProjectile(vector82.X, vector82.Y, num825, num826, num830, num829 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}
			}

			// Float to the side of the target and fire lasers
			else
			{
				int num831 = 1;
				if (npc.position.X + (float)(npc.width / 2) < player.position.X + (float)player.width)
					num831 = -1;

				float num832 = expertMode ? 9.5f : 8f;
				float num833 = expertMode ? 0.25f : 0.2f;
				if (phase2)
				{
					num832 = expertMode ? 10f : 8.5f;
					num833 = expertMode ? 0.255f : 0.205f;
				}
				if (provy)
				{
					num832 *= 1.25f;
					num833 *= 1.25f;
				}
				if (CalamityWorld.bossRushActive)
				{
					num832 *= 1.5f;
					num833 *= 1.5f;
				}

				Vector2 vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num834 = player.position.X + (float)(player.width / 2) + (float)(num831 * (CalamityWorld.bossRushActive ? 460 : 360)) - vector83.X;
				float num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
				float num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
				num836 = num832 / num836;
				num834 *= num836;
				num835 *= num836;

				if (npc.velocity.X < num834)
				{
					npc.velocity.X = npc.velocity.X + num833;
					if (npc.velocity.X < 0f && num834 > 0f)
						npc.velocity.X = npc.velocity.X + num833;
				}
				else if (npc.velocity.X > num834)
				{
					npc.velocity.X = npc.velocity.X - num833;
					if (npc.velocity.X > 0f && num834 < 0f)
						npc.velocity.X = npc.velocity.X - num833;
				}
				if (npc.velocity.Y < num835)
				{
					npc.velocity.Y = npc.velocity.Y + num833;
					if (npc.velocity.Y < 0f && num835 > 0f)
						npc.velocity.Y = npc.velocity.Y + num833;
				}
				else if (npc.velocity.Y > num835)
				{
					npc.velocity.Y = npc.velocity.Y - num833;
					if (npc.velocity.Y > 0f && num835 < 0f)
						npc.velocity.Y = npc.velocity.Y - num833;
				}

				vector83 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				num834 = player.position.X + (float)(player.width / 2) - vector83.X;
				num835 = player.position.Y + (float)(player.height / 2) - vector83.Y;
				npc.rotation = (float)Math.Atan2((double)num835, (double)num834) - 1.57f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.localAI[1] += 1f;
					if (phase2)
					{
						if (!brotherAlive)
						{
							if (revenge)
								npc.localAI[1] += 0.5f;
							if (CalamityWorld.death || CalamityWorld.bossRushActive)
								npc.localAI[1] += 0.5f;
							if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
								npc.localAI[1] += 0.5f;
							if (expertMode)
								npc.localAI[1] += 0.5f;
						}

						if (npc.localAI[1] >= 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num837 = CalamityWorld.bossRushActive ? 15f : 11f;
							int num838 = expertMode ? 28 : 35;
							int num839 = ModContent.ProjectileType<BrimstoneLaser>();
							num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 12f;
							vector83.Y += num835 * 12f;
							Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
					else
					{
						if (revenge)
							npc.localAI[1] += 0.5f;

						if (npc.localAI[1] >= 60f && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
						{
							npc.localAI[1] = 0f;
							float num837 = CalamityWorld.bossRushActive ? 14f : 10.5f;
							int num838 = expertMode ? 20 : 24;
							int num839 = ModContent.ProjectileType<BrimstoneLaser>();
							num836 = (float)Math.Sqrt((double)(num834 * num834 + num835 * num835));
							num836 = num837 / num836;
							num834 *= num836;
							num835 *= num836;
							vector83.X += num834 * 12f;
							vector83.Y += num835 * 12f;
							Projectile.NewProjectile(vector83.X, vector83.Y, num834, num835, num839, num838 + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				npc.ai[2] += 1f;
				if (npc.ai[2] >= (phase2 ? 120f : 180f))
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.TargetClosest(true);
					npc.netUpdate = true;
				}
			}
		}
		#endregion

		#region Astrum Aureus
		public static void AstrumAureusAI(NPC npc, Mod mod)
        {
            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f || CalamityWorld.bossRushActive;
            bool phase3 = lifeRatio < 0.5f || CalamityWorld.bossRushActive;

            // Variables
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            int shootBuff = (int)(2f * (1f - lifeRatio));
            float shootTimer = 1f + ((float)shootBuff);
            bool dayTime = Main.dayTime;
            Player player = Main.player[npc.target];
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;

            // Despawn
            if (!player.active || player.dead || dayTime)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];

                if (!player.active || player.dead)
                {
                    npc.noTileCollide = true;
                    npc.velocity = new Vector2(0f, 10f);

                    if (npc.timeLeft > 150)
                        npc.timeLeft = 150;

                    return;
                }
            }
            else
            {
                if (npc.timeLeft < 1800)
                    npc.timeLeft = 1800;
            }

            // Emit light when not Idle
            if (npc.ai[0] != 1f)
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 2.55f, 1f, 0f);

            // Fire projectiles while walking, teleporting, or falling
            if (npc.ai[0] == 2f || npc.ai[0] >= 5f || (npc.ai[0] == 4f && npc.velocity.Y > 0f) ||
                npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += (npc.ai[0] == 2f || (npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode)) ? 4f : shootTimer;
                    if (npc.localAI[0] >= 180f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest(true);
                        Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 33);
                        int laserDamage = expertMode ? 32 : 37;
                        if (NPC.downedMoonlord && revenge && !CalamityWorld.bossRushActive)
                            laserDamage *= 3;

                        // Fire astral flames while teleporting
                        if ((npc.ai[0] >= 5f && npc.ai[0] != 7) || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
                        {
                            Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float spread = 45f * 0.0174f;
                            double startAngle = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spread / 2;
                            double deltaAngle = spread / 8f;
                            double offsetAngle;
                            int i;
                            float velocity = CalamityWorld.bossRushActive ? 10f : 7f;
                            for (i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(Math.Sin(offsetAngle) * velocity),
                                    (float)(Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<AstralFlame>(), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, (float)(-Math.Sin(offsetAngle) * velocity),
                                    (float)(-Math.Cos(offsetAngle) * velocity), ModContent.ProjectileType<AstralFlame>(), laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }

                        // Fire astral lasers while falling or walking
                        else if ((npc.ai[0] == 4f && npc.velocity.Y > 0f && expertMode) || npc.ai[0] == 2f)
                        {
                            float num179 = CalamityWorld.bossRushActive ? 24f : 18.5f;
                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num185 = ModContent.ProjectileType<AstralLaser>();
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 5; num186++)
                            {
                                num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                num183 = num179 / num183;
                                num180 += (float)Main.rand.Next(-60, 61);
                                num182 += (float)Main.rand.Next(-60, 61);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, laserDamage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }

            // Start up
            if (npc.ai[0] == 0f)
            {
                // If hit or after two seconds start Idle phase
                npc.ai[1] += 1f;
                if (npc.justHit || npc.ai[1] >= 120f)
                {
                    // Set AI to next phase (Idle) and reset other AI
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Idle
            else if (npc.ai[0] == 1f)
            {
                // Decrease defense
                npc.defense = 0;

                // Slow down
                npc.velocity.X *= 0.98f;
                npc.velocity.Y *= 0.98f;

                // Stay vulnerable for a maximum of 1.5 or 2.5 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((phase3 || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 90f : 150f))
                {
                    // Increase defense
                    npc.defense = 70;

                    // Stop colliding with tiles
                    npc.noGravity = true;
                    npc.noTileCollide = true;

                    // Set AI to next phase (Walk) and reset other AI
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Walk
            else if (npc.ai[0] == 2f)
            {
                // Set walking speed
                float num823 = (CalamityWorld.bossRushActive ? 8f : 5f) + (3f * (1f - lifeRatio));

                // Set walking direction
                if (Math.Abs(npc.Center.X - player.Center.X) < 200f)
                {
                    npc.velocity.X *= 0.9f;
                    if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                        npc.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0 ? 1 : -1;

                    if (npc.direction > 0)
                        npc.velocity.X = (npc.velocity.X * 20f + num823) / 21f;
                    if (npc.direction < 0)
                        npc.velocity.X = (npc.velocity.X * 20f - num823) / 21f;
                }

                // Walk through tiles if colliding with tiles and player is out of reach
                int num854 = 80;
                int num855 = 20;
                Vector2 position2 = new Vector2(npc.Center.X - (float)(num854 / 2), npc.position.Y + (float)npc.height - (float)num855);

                bool flag52 = false;
                if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width && npc.position.Y + (float)npc.height < player.position.Y + (float)player.height - 16f)
                    flag52 = true;

                if (flag52)
                    npc.velocity.Y += 0.5f;
                else if (Collision.SolidCollision(position2, num854, num855))
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y = 0f;

                    if ((double)npc.velocity.Y > -0.2)
                        npc.velocity.Y -= 0.025f;
                    else
                        npc.velocity.Y -= 0.2f;

                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;
                }
                else
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y = 0f;

                    if ((double)npc.velocity.Y < 0.1)
                        npc.velocity.Y += 0.025f;
                    else
                        npc.velocity.Y += 0.5f;
                }

                // Walk for a maximum of 6 seconds
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 360f)
                {
                    // Collide with tiles again
                    npc.noGravity = false;
                    npc.noTileCollide = false;

                    // Set AI to next phase (Jump) and reset other AI
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                // Limit downward velocity
                if (npc.velocity.Y > 10f)
                    npc.velocity.Y = 10f;
            }

            // Jump
            else if (npc.ai[0] == 3f)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    // Slow down
                    npc.velocity.X *= 0.8f;

                    // Half second delay before jumping
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 30f)
                        npc.ai[1] = -20f;
                    else if (npc.ai[1] == -1f)
                    {
                        // Set jump velocity, reset and set AI to next phase (Stomp)
                        npc.TargetClosest(true);

                        float velocityX = (CalamityWorld.bossRushActive ? 9f : 6f) + (6f * (1f - lifeRatio));
                        npc.velocity.X = velocityX * (float)npc.direction;

                        if (revenge)
                        {
                            if (Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                                npc.velocity.Y = -14.5f;
                            else
                                npc.velocity.Y = 1f;

                            npc.noTileCollide = true;
                        }
                        else
                            npc.velocity.Y = -14.5f;

                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                    }
                }
            }

            // Stomp
            else if (npc.ai[0] == 4f)
            {
                if (npc.velocity.Y == 0f)
                {
                    // Play stomp sound
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LegStomp"), (int)npc.position.X, (int)npc.position.Y);

                    // Stomp and jump again, if stomped twice then reset and set AI to next phase (Teleport or Idle)
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 3f)
                    {
                        npc.ai[0] = (phase2 || revenge) ? 5f : 1f;
                        npc.ai[2] = 0f;
                    }
                    else
                        npc.ai[0] = 3f;

                    // Spawn dust for visual effect
                    for (int num622 = (int)npc.position.X - 20; num622 < (int)npc.position.X + npc.width + 40; num622 += 20)
                    {
                        for (int num623 = 0; num623 < 4; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y + (float)npc.height), npc.width + 20, 4, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].velocity *= 0.2f;
                        }
                    }
                }
                else
                {
                    // Set velocities while falling, this happens before the stomp
                    npc.TargetClosest(true);

                    // Fall through
                    if (npc.target >= 0 && revenge && ((player.position.Y > npc.position.Y + (float)npc.height && npc.velocity.Y > 0f) || (player.position.Y < npc.position.Y + (float)npc.height && npc.velocity.Y < 0f)))
                        npc.noTileCollide = true;
                    else
                        npc.noTileCollide = false;

                    if (npc.position.X < player.position.X && npc.position.X + (float)npc.width > player.position.X + (float)player.width)
                    {
                        npc.velocity.X *= 0.9f;

                        if (player.position.Y > npc.position.Y + (float)npc.height)
                        {
                            float fallSpeed = 0.8f + (0.8f * (1f - lifeRatio));
                            npc.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        if (npc.direction < 0)
                            npc.velocity.X -= 0.2f;
                        else if (npc.direction > 0)
                            npc.velocity.X += 0.2f;

                        float num626 = (CalamityWorld.bossRushActive ? 12f : 9f) + (6f * (1f - lifeRatio));
                        if (npc.velocity.X < -num626)
                            npc.velocity.X = -num626;
                        if (npc.velocity.X > num626)
                            npc.velocity.X = num626;
                    }
                }
            }

            // Teleport
            else if (npc.ai[0] == 5f)
            {
                // Slow down
                npc.velocity *= 0.95f;

                // Spawn slimes and start teleport
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += 1f;
                    if (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        npc.localAI[1] += 5f;

                    if (npc.localAI[1] >= 240f)
                    {
                        // Spawn slimes
                        bool spawnFlag = revenge;
                        if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                            spawnFlag = false;
                        if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

                        // Reset localAI and find a teleport destination
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        int num1249 = 0;
                        int num1250;
                        int num1251;

                        while (true)
                        {
                            num1249++;
                            num1250 = (int)player.Center.X / 16;
                            num1251 = (int)player.Center.Y / 16;
                            num1250 += Main.rand.Next(-30, 31);
                            num1251 += Main.rand.Next(-30, 31);

                            if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2((float)(num1250 * 16), (float)(num1251 * 16)), 1, 1, player.position, player.width, player.height))
                                break;

                            if (num1249 > 100)
                                goto Block;
                        }

                        // Set AI to next phase (Mid-teleport), set AI 2 and 3 to teleport coordinates X and Y respectively
                        npc.ai[0] = 6f;
                        npc.ai[2] = (float)num1250;
                        npc.ai[3] = (float)num1251;
                        npc.netUpdate = true;
                        Block:
                        ;
                    }
                }
            }

            // Mid-teleport
            else if (npc.ai[0] == 6f)
            {
                // Become immune
                npc.chaseable = false;
                npc.dontTakeDamage = true;

                // Turn invisible
                npc.alpha += 10;
                if (npc.alpha >= 255)
                {
                    // Set position to teleport destination
                    npc.position.X = npc.ai[2] * 16f - (float)(npc.width / 2);
                    npc.position.Y = npc.ai[3] * 16f - (float)(npc.height / 2);

                    // Reset alpha and set AI to next phase (End of teleport)
                    npc.alpha = 255;
                    npc.ai[0] = 7f;
                    npc.netUpdate = true;
                }

                // Play sound for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }

            // End of teleport
            else if (npc.ai[0] == 7f)
            {
                // Turn visible
                npc.alpha -= 10;
                if (npc.alpha <= 0)
                {
                    // Spawn slimes
                    bool spawnFlag = revenge;
                    if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) > 1)
                        spawnFlag = false;
                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 25, ModContent.NPCType<AureusSpawn>(), 0, 0f, 0f, 0f, 0f, 255);

                    // Become vulnerable
                    npc.chaseable = true;
                    npc.dontTakeDamage = false;

                    // Reset alpha and set AI to next phase (Idle)
                    npc.alpha = 0;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }

                // Play sound at teleport destination for cool effect
                if (npc.soundDelay == 0)
                {
                    npc.soundDelay = 15;
                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                }

                // Emit dust to make the teleport pretty
                int num;
                for (int num245 = 0; num245 < 10; num245 = num + 1)
                {
                    int num244 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<AstralOrange>(), npc.velocity.X, npc.velocity.Y, 255, default, 2f);
                    Main.dust[num244].noGravity = true;
                    Main.dust[num244].velocity *= 0.5f;
                    num = num245;
                }
            }
        }
		#endregion

		#region Bumblebirb
		public static void BumblebirbAI(NPC npc, Mod mod)
		{
			// Variables
			Player player = Main.player[npc.target];
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			Vector2 vector = npc.Center;

			// Percent life remaining
			float lifeRatio = (float)npc.life / (float)npc.lifeMax;

			// Phases
			float mult = 1f +
				(revenge ? 0.25f : 0f) +
				((CalamityWorld.death || CalamityWorld.bossRushActive) ? 0.25f : 0f);
			bool phase2 = lifeRatio < 0.5f * mult;
			bool phase3 = lifeRatio < 0.1f * mult;

			// Max spawn amount
			int num1305 = revenge ? 4 : 3;
			if (CalamityWorld.death || CalamityWorld.bossRushActive)
				num1305 = 5;
			if (phase2)
				num1305 = 2;

			// Don't collide with tiles, disable gravity
			npc.noTileCollide = false;
			npc.noGravity = true;

			// Reset damage
			npc.damage = npc.defDamage;

			// Despawn
			if (Vector2.Distance(player.Center, vector) > 5600f)
			{
				if (npc.timeLeft > 10)
					npc.timeLeft = 10;
			}

			// Fly to target if target is too far away and not in idle or switch phase
			Vector2 vector205 = player.Center - npc.Center;
			if (npc.ai[0] > 1f && vector205.Length() > 3600f)
				npc.ai[0] = 1f;

			// Phase switch
			if (npc.ai[0] == 0f)
			{
				// Target
				npc.TargetClosest(true);

				if (npc.Center.X < player.Center.X - 2f)
					npc.direction = 1;
				if (npc.Center.X > player.Center.X + 2f)
					npc.direction = -1;

				// Direction and rotation
				npc.spriteDirection = npc.direction;
				npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

				// Slow down if colliding with tiles
				if (npc.collideX)
				{
					npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
					if (npc.velocity.X > 4f)
						npc.velocity.X = 4f;
					if (npc.velocity.X < -4f)
						npc.velocity.X = -4f;
				}
				if (npc.collideY)
				{
					npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
					if (npc.velocity.Y > 4f)
						npc.velocity.Y = 4f;
					if (npc.velocity.Y < -4f)
						npc.velocity.Y = -4f;
				}

				// Fly to target if target is too far away, otherwise get close to target and then slow down
				Vector2 value51 = player.Center - npc.Center;
				value51.Y -= 200f;
				if (value51.Length() > 2800f)
				{
					npc.ai[0] = 1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
				}
				else if (value51.Length() > 240f)
				{
					float scaleFactor15 = 12f;
					float num1306 = 30f;
					value51.Normalize();
					value51 *= scaleFactor15;
					npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
				}
				else if (npc.velocity.Length() > 2f)
					npc.velocity *= 0.95f;
				else if (npc.velocity.Length() < 1f)
					npc.velocity *= 1.05f;

				// Phase switch
				npc.ai[1] += 1f;
				if (npc.ai[1] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					npc.netUpdate = true;
					while (npc.ai[0] == 0f)
					{
						int damage = Main.expertMode ? 50 : 60;

						int num1307 = phase2 ? Main.rand.Next(2) + 1 : Main.rand.Next(3);
						if (phase3)
							num1307 = 1;

						if (num1307 == 0 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
							npc.ai[0] = 2f;
						else if (num1307 == 1)
						{
							npc.ai[0] = 3f;
							if (phase2)
							{
								Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 102);
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-2, 3), -4f, ModContent.ProjectileType<RedLightningFeather>(), damage, 0f, Main.myPlayer, 0f, 0f);
							}
						}
						else if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < num1305)
						{
							npc.ai[0] = 4f;
							Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 102);
							int featherAmt = phase2 ? 3 : 6;
							for (int num186 = 0; num186 < featherAmt; num186++)
							{
								Projectile.NewProjectile(npc.Center.X, npc.Center.Y, (float)Main.rand.Next(-4, 5), -3f, ModContent.ProjectileType<RedLightningFeather>(), damage, 0f, Main.myPlayer, 0f, 0f);
							}
						}
					}
				}
			}
			else
			{
				// Fly to target
				if (npc.ai[0] == 1f)
				{
					npc.collideX = false;
					npc.collideY = false;
					npc.noTileCollide = true;

					if (npc.target < 0 || !player.active || player.dead)
						npc.TargetClosest(true);

					if (npc.velocity.X < 0f)
						npc.direction = -1;
					else if (npc.velocity.X > 0f)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.04f) / 10f;

					Vector2 value52 = player.Center - npc.Center;
					if (value52.Length() < 800f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					float scaleFactor16 = 14f + value52.Length() / 100f; //7
					float num1308 = 25f;
					value52.Normalize();
					value52 *= scaleFactor16;
					npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
					return;
				}

				// Fly towards target quickly
				if (npc.ai[0] == 2f)
				{
					if (npc.target < 0 || !player.active || player.dead)
					{
						npc.TargetClosest(true);
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}

					if (player.Center.X - 10f < npc.Center.X)
						npc.direction = -1;
					else if (player.Center.X + 10f > npc.Center.X)
						npc.direction = 1;

					npc.spriteDirection = npc.direction;
					npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.05f) / 5f;

					if (npc.collideX)
					{
						npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
						if (npc.velocity.X > 4f)
							npc.velocity.X = 4f;
						if (npc.velocity.X < -4f)
							npc.velocity.X = -4f;
					}
					if (npc.collideY)
					{
						npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
						if (npc.velocity.Y > 4f)
							npc.velocity.Y = 4f;
						if (npc.velocity.Y < -4f)
							npc.velocity.Y = -4f;
					}

					Vector2 value53 = player.Center - npc.Center;
					value53.Y -= 20f;
					npc.ai[2] += 0.0222222228f;
					if (Main.expertMode)
						npc.ai[2] += 0.0166666675f;

					float scaleFactor17 = 8f + npc.ai[2] + value53.Length() / 120f; //4
					float num1309 = 20f;
					value53.Normalize();
					value53 *= scaleFactor17;
					npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;

					npc.ai[1] += 1f;
					if (npc.ai[1] >= 120f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
					}
				}
				else
				{
					// Variable for charging
					float chargeDistance = 600f;
					if (phase2)
						chargeDistance -= 50f;
					if (phase3)
						chargeDistance -= 50f;

					// Line up charge
					if (npc.ai[0] == 3f)
					{
						npc.noTileCollide = true;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;
						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;

						Vector2 value54 = player.Center - npc.Center;
						value54.Y -= 12f;
						if (npc.Center.X > player.Center.X)
							value54.X += chargeDistance;
						else
							value54.X -= chargeDistance;

						if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance - 50f && Math.Abs(npc.Center.Y - player.Center.Y) < (phase3 ? 100f : 20f))
						{
							npc.ai[0] = 3.1f;
							npc.ai[1] = 0f;
						}

						npc.ai[1] += 0.0333333351f;
						float scaleFactor18 = 18f + npc.ai[1];
						float num1310 = 4f;
						value54.Normalize();
						value54 *= scaleFactor18;
						npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
						return;
					}

					// Prepare to charge
					if (npc.ai[0] == 3.1f)
					{
						npc.noTileCollide = true;

						npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;

						Vector2 vector206 = player.Center - npc.Center;
						vector206.Y -= 12f;
						float scaleFactor19 = 32f; //16
						float num1311 = 8f;
						vector206.Normalize();
						vector206 *= scaleFactor19;
						npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;

						if (npc.velocity.X < 0f)
							npc.direction = -1;
						else
							npc.direction = 1;

						npc.spriteDirection = npc.direction;

						npc.ai[1] += 1f;
						if (npc.ai[1] > 10f)
						{
							npc.velocity = vector206;

							if (npc.velocity.X < 0f)
								npc.direction = -1;
							else
								npc.direction = 1;

							npc.ai[0] = 3.2f;
							npc.ai[1] = 0f;
							npc.ai[1] = (float)npc.direction;
						}
					}
					else
					{
						// Charge
						if (npc.ai[0] == 3.2f)
						{
							npc.damage = (int)((double)npc.defDamage * 1.5);

							npc.collideX = false;
							npc.collideY = false;
							npc.noTileCollide = true;

							npc.ai[2] += 0.0333333351f;
							npc.velocity.X = (32f + npc.ai[2]) * npc.ai[1];

							if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + (chargeDistance - 140f)) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - (chargeDistance - 140f)))
							{
								if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
								else if (Math.Abs(npc.Center.X - player.Center.X) > chargeDistance + 200f)
								{
									npc.ai[0] = 1f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;
								}
							}

							npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
							return;
						}

						// Find tile coordinates for birb spawn
						if (npc.ai[0] == 4f)
						{
							npc.ai[0] = 0f;

							npc.TargetClosest(true);

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								npc.ai[1] = -1f;
								npc.ai[2] = -1f;

								for (int num1312 = 0; num1312 < 1000; num1312++)
								{
									int num1313 = (int)player.Center.X / 16;
									int num1314 = (int)player.Center.Y / 16;

									int num1315 = 30 + num1312 / 50;
									int num1316 = 20 + num1312 / 75;

									num1313 += Main.rand.Next(-num1315, num1315 + 1);
									num1314 += Main.rand.Next(-num1316, num1316 + 1);

									if (!WorldGen.SolidTile(num1313, num1314))
									{
										while (!WorldGen.SolidTile(num1313, num1314) && (double)num1314 < Main.worldSurface)
											num1314++;

										if ((new Vector2((float)(num1313 * 16 + 8), (float)(num1314 * 16 + 8)) - player.Center).Length() < 3600f)
										{
											npc.ai[0] = 4.1f;
											npc.ai[1] = (float)num1313;
											npc.ai[2] = (float)num1314;
											break;
										}
									}
								}
							}

							npc.netUpdate = true;
							return;
						}

						// Move to birb spawn location
						if (npc.ai[0] == 4.1f)
						{
							if (npc.velocity.X < -2f)
								npc.direction = -1;
							else if (npc.velocity.X > 2f)
								npc.direction = 1;

							npc.spriteDirection = npc.direction;
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

							npc.noTileCollide = true;

							int num1317 = (int)npc.ai[1];
							int num1318 = (int)npc.ai[2];

							float x2 = (float)(num1317 * 16 + 8);
							float y2 = (float)(num1318 * 16 - 20);

							Vector2 vector207 = new Vector2(x2, y2);
							vector207 -= npc.Center;
							float num1319 = 12f + vector207.Length() / 150f;
							if (num1319 > 20f)
								num1319 = 20f;

							float num1320 = 10f;
							if (vector207.Length() < 10f)
								npc.ai[0] = 4.2f;

							vector207.Normalize();
							vector207 *= num1319;
							npc.velocity = (npc.velocity * (num1320 - 1f) + vector207) / num1320;
							return;
						}

						// Spawn birbs
						if (npc.ai[0] == 4.2f)
						{
							npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;

							npc.noTileCollide = true;

							int num1321 = (int)npc.ai[1];
							int num1322 = (int)npc.ai[2];

							float x3 = (float)(num1321 * 16 + 8);
							float y3 = (float)(num1322 * 16 - 20);

							Vector2 vector208 = new Vector2(x3, y3);
							vector208 -= npc.Center;

							float num1323 = 4f; //4
							float num1324 = 2f; //2

							if (Main.netMode != NetmodeID.MultiplayerClient && vector208.Length() < 4f)
							{
								int num1325 = 10;
								if (Main.expertMode)
									num1325 = (int)((double)num1325 * 0.75);

								npc.ai[3] += 1f;
								if (npc.ai[3] == (float)num1325)
									NPC.NewNPC(num1321 * 16 + 8, num1322 * 16, ModContent.NPCType<Bumblefuck2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
								else if (npc.ai[3] == (float)(num1325 * 2))
								{
									npc.ai[0] = 0f;
									npc.ai[1] = 0f;
									npc.ai[2] = 0f;
									npc.ai[3] = 0f;

									if (NPC.CountNPCS(ModContent.NPCType<Bumblefuck2>()) < num1305 && Main.rand.Next(5) != 0)
										npc.ai[0] = 4f;
									else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
										npc.ai[0] = 1f;
								}
							}

							if (vector208.Length() > num1323)
							{
								vector208.Normalize();
								vector208 *= num1323;
							}

							npc.velocity = (npc.velocity * (num1324 - 1f) + vector208) / num1324;
						}
					}
				}
			}
		}
		#endregion
	}
}
