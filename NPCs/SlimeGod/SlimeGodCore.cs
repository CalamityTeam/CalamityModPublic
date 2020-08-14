using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.SlimeGod
{
	[AutoloadBossHead]
    public class SlimeGodCore : ModNPC
    {
		private bool slimesSpawned = false;
		private int buffedSlime = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Slime God");
        }

        public override void SetDefaults()
        {
            npc.damage = 40;
            npc.npcSlots = 10f;
            npc.width = 44;
            npc.height = 44;
            npc.defense = 6;
            npc.LifeMaxNERB(4000, 5000, 2500000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 8, 0, 0);
            npc.alpha = 55;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SlimeGod");
            else
                music = MusicID.Boss1;
            bossBag = ModContent.ItemType<SlimeGodBag>();
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			writer.Write(buffedSlime);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			buffedSlime = reader.ReadInt32();
		}

		public override void AI()
        {
            CalamityGlobalNPC.slimeGod = npc.whoAmI;

            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			npc.TargetClosest(true);
			Player player = Main.player[npc.target];

			Vector2 vectorCenter = npc.Center;
			if (Main.netMode != NetmodeID.MultiplayerClient && !slimesSpawned)
			{
				slimesSpawned = true;
				NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<SlimeGod>());
				NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<SlimeGodRun>());
			}

			// Emit dust
			int randomDust = Main.rand.NextBool(2) ? 173 : 260;
            int num658 = Dust.NewDust(npc.position, npc.width, npc.height, randomDust, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.5f);
            Main.dust[num658].noGravity = true;
            Main.dust[num658].velocity *= 0.5f;

			npc.dontTakeDamage = false;

			// Set damage
			npc.damage = npc.defDamage;

			// Enrage based on large slimes
			bool phase2 = lifeRatio < 0.4f;
			bool hyperMode = true;
			bool purpleSlimeAlive = false;
			bool redSlimeAlive = false;

			if (CalamityGlobalNPC.slimeGodPurple != -1)
			{
				if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
				{
					if (buffedSlime == 1)
						Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 1f;
					else
						Main.npc[CalamityGlobalNPC.slimeGodPurple].localAI[1] = 0f;

					npc.Calamity().newAI[0] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.X;
					npc.Calamity().newAI[1] = Main.npc[CalamityGlobalNPC.slimeGodPurple].Center.Y;

					purpleSlimeAlive = true;
					phase2 = lifeRatio < 0.2f;
					hyperMode = false;
				}
			}

			if (CalamityGlobalNPC.slimeGodRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
				{
					if (buffedSlime == 2)
						Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 1f;
					else
						Main.npc[CalamityGlobalNPC.slimeGodRed].localAI[1] = 0f;

					npc.localAI[2] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.X;
					npc.localAI[3] = Main.npc[CalamityGlobalNPC.slimeGodRed].Center.Y;

					redSlimeAlive = true;
					phase2 = lifeRatio < 0.2f;
					hyperMode = false;
				}
			}

			// Despawn
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.2f;
					if (npc.velocity.Y > 16f)
						npc.velocity.Y = 16f;

					if (npc.position.Y > Main.worldSurface * 16.0)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<SlimeGod>() || Main.npc[x].type == ModContent.NPCType<SlimeGodSplit>() ||
                                Main.npc[x].type == ModContent.NPCType<SlimeGodRun>() || Main.npc[x].type == ModContent.NPCType<SlimeGodRunSplit>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        npc.active = false;
                        npc.netUpdate = true;
                    }

					if (npc.ai[0] != 0f || npc.ai[1] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.localAI[0] = 0f;
						npc.localAI[1] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

			float ai1 = hyperMode ? 270f : 360f;

			// Hide inside large slime
			if (!hyperMode && npc.ai[1] < ai1)
			{
				if (npc.Calamity().newAI[2] == 0f && npc.life > 0)
				{
					npc.Calamity().newAI[2] = npc.lifeMax;
				}
				if (npc.life > 0)
				{
					int num660 = (int)(npc.lifeMax * 0.05);
					if ((npc.life + num660) < npc.Calamity().newAI[2])
					{
						npc.Calamity().newAI[2] = npc.life;
						npc.Calamity().newAI[3] = 1f;
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SlimeGodPossession"), (int)npc.position.X, (int)npc.position.Y);
					}
				}

				if (npc.Calamity().newAI[3] == 1f)
				{
					npc.dontTakeDamage = true;

					npc.rotation = npc.velocity.X * 0.1f;

					if (buffedSlime == 0)
					{
						if (purpleSlimeAlive && redSlimeAlive)
							buffedSlime = Main.rand.Next(2) + 1;
						else if (purpleSlimeAlive)
							buffedSlime = 1;
						else if (redSlimeAlive)
							buffedSlime = 2;
					}

					Vector2 purpleSlimeVector = new Vector2(npc.Calamity().newAI[0], npc.Calamity().newAI[1]);
					Vector2 redSlimeVector = new Vector2(npc.localAI[2], npc.localAI[3]);
					Vector2 goToVector = buffedSlime == 1 ? purpleSlimeVector : redSlimeVector;

					Vector2 goToPosition = goToVector - vectorCenter;
					npc.velocity = Vector2.Normalize(goToPosition) * (CalamityWorld.bossRushActive ? 24f : 16f);

					bool slimeDead = false;
					if (goToVector == purpleSlimeVector)
						slimeDead = CalamityGlobalNPC.slimeGodPurple < 0 || !Main.npc[CalamityGlobalNPC.slimeGodPurple].active;
					else
						slimeDead = CalamityGlobalNPC.slimeGodRed < 0 || !Main.npc[CalamityGlobalNPC.slimeGodRed].active;

					npc.ai[2] += 1f;
					if (npc.ai[2] >= 600f || slimeDead)
					{
						npc.ai[2] = 0f;
						npc.Calamity().newAI[3] = 0f;
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/SlimeGodExit"), (int)npc.position.X, (int)npc.position.Y);
						for (int i = 0; i < 20; i++)
						{
							int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
							Main.dust[dust].velocity *= 3f;
							if (Main.rand.NextBool(2))
							{
								Main.dust[dust].scale = 0.5f;
								Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
							}
						}
						for (int j = 0; j < 30; j++)
						{
							int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 3f);
							Main.dust[dust].noGravity = true;
							Main.dust[dust].velocity *= 5f;
							dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
							Main.dust[dust].velocity *= 2f;
						}
					}

					return;
				}

				buffedSlime = 0;
			}

			// Spin and shoot orbs
            if (phase2)
            {
				npc.ai[1] += 1f;
				if (revenge)
				{
					if (npc.ai[1] >= ai1)
					{
						if (npc.localAI[1] == 0f)
						{
							// Slow down, rotation
							npc.rotation = npc.velocity.X * 0.1f;

							// Set teleport location, turn invisible, spin direction
							npc.alpha += 20;
							if (npc.alpha >= 255)
							{
								npc.alpha = 255;
								npc.velocity.Normalize();

								int teleportX = player.velocity.X < 0f ? -20 : 20;
								int teleportY = player.velocity.Y < 0f ? -10 : 10;
								int playerPosX = (int)player.Center.X / 16 + teleportX;
								int playerPosY = (int)player.Center.Y / 16 - teleportY;

								npc.ai[2] = playerPosX;
								npc.ai[3] = playerPosY;
								npc.localAI[1] = 1f;
								npc.netUpdate = true;
							}
						}
						else if (npc.localAI[1] == 1f)
						{
							// Rotation
							npc.rotation = npc.velocity.X * 0.1f;

							// Teleport to location
							if (npc.alpha == 255)
							{
								Vector2 position = new Vector2(npc.ai[2] * 16f - (npc.width / 2), npc.ai[3] * 16f - (npc.height / 2));
								npc.position = position;
							}

							// Turn visible
							npc.alpha -= 20;
							if (npc.alpha < 55)
							{
								npc.alpha = 55;
								npc.localAI[0] = vectorCenter.X - player.Center.X < 0 ? 1f : -1f;
								npc.localAI[1] = 2f;
							}
							npc.netUpdate = true;
						}
						else
						{
							// No damage while spinning
							npc.damage = 0;

							// Rotation
							npc.rotation += npc.direction * 0.3f;

							// Velocity boost
							if (npc.localAI[1] == 2f)
							{
								npc.localAI[1] = 3f;
								npc.velocity *= 12f;
							}

							// Spin velocity
							float velocity = MathHelper.TwoPi / (180f - (npc.ai[1] - ai1));
							npc.velocity = npc.velocity.RotatedBy(-(double)velocity * npc.localAI[0]);

							// Reset and charge at target
							if (npc.ai[1] >= ai1 + 100f)
							{
								npc.ai[1] = 0f;
								npc.ai[2] = 0f;
								npc.ai[3] = 0f;
								npc.localAI[0] = 0f;
								npc.localAI[1] = 0f;
								float chargeVelocity = death ? 12f : 8f;
								npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeVelocity;
								return;
							}

							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								if (npc.ai[1] % 15f == 0f && Vector2.Distance(player.Center, vectorCenter) > 160f)
								{
									if (expertMode && Main.rand.NextBool(2))
									{
										float num179 = revenge ? 2f : 3f;
										if (CalamityWorld.bossRushActive)
											num179 = 12f;
										Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
										float num180 = player.position.X + player.width * 0.5f - value9.X;
										float num181 = Math.Abs(num180) * 0.1f;
										float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
										float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
										npc.netUpdate = true;
										num183 = num179 / num183;
										num180 *= num183;
										num182 *= num183;
										int num184 = 24;
										int num185 = Main.rand.Next(2);
										if (num185 == 0)
										{
											num185 = ModContent.ProjectileType<AbyssMine>();
										}
										else
										{
											num185 = ModContent.ProjectileType<AbyssMine2>();
											num184 = 22;
										}
										value9.X += num180;
										value9.Y += num182;
										num180 = player.position.X + player.width * 0.5f - value9.X;
										num182 = player.position.Y + player.height * 0.5f - value9.Y;
										num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
										num183 = num179 / num183;
										num180 *= num183;
										num182 *= num183;
										Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
									}
									else
									{
										float num179 = revenge ? 6f : 5f;
										if (CalamityWorld.bossRushActive)
											num179 = 12f;
										Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
										float num180 = player.position.X + player.width * 0.5f - value9.X;
										float num181 = Math.Abs(num180) * 0.1f;
										float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
										float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
										npc.netUpdate = true;
										num183 = num179 / num183;
										num180 *= num183;
										num182 *= num183;
										int num184 = expertMode ? 17 : 21;
										int num185 = Main.rand.Next(2);
										if (num185 == 0)
										{
											num185 = ModContent.ProjectileType<AbyssBallVolley>();
										}
										else
										{
											num185 = ModContent.ProjectileType<AbyssBallVolley2>();
											num184 = expertMode ? 16 : 19;
										}
										value9.X += num180;
										value9.Y += num182;
										num180 = player.position.X + player.width * 0.5f - value9.X;
										num182 = player.position.Y + player.height * 0.5f - value9.Y;
										num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
										num183 = num179 / num183;
										num180 *= num183;
										num182 *= num183;
										Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
									}
								}
							}
						}
						return;
					}
				}
				else
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && Vector2.Distance(player.Center, vectorCenter) > 160f)
					{
						if (npc.ai[1] % 40f == 0f)
						{
							if (expertMode && Main.rand.NextBool(2))
							{
								float num179 = revenge ? 2f : 3f;
								if (CalamityWorld.bossRushActive)
									num179 = 12f;
								Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
								float num180 = player.position.X + player.width * 0.5f - value9.X;
								float num181 = Math.Abs(num180) * 0.1f;
								float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
								float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
								npc.netUpdate = true;
								num183 = num179 / num183;
								num180 *= num183;
								num182 *= num183;
								int num184 = 24;
								int num185 = Main.rand.Next(2);
								if (num185 == 0)
								{
									num185 = ModContent.ProjectileType<AbyssMine>();
								}
								else
								{
									num185 = ModContent.ProjectileType<AbyssMine2>();
									num184 = 22;
								}
								value9.X += num180;
								value9.Y += num182;
								num180 = player.position.X + player.width * 0.5f - value9.X;
								num182 = player.position.Y + player.height * 0.5f - value9.Y;
								num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
								num183 = num179 / num183;
								num180 *= num183;
								num182 *= num183;
								Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
							}
							else
							{
								float num179 = revenge ? 6f : 5f;
								if (CalamityWorld.bossRushActive)
									num179 = 12f;
								Vector2 value9 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
								float num180 = player.position.X + player.width * 0.5f - value9.X;
								float num181 = Math.Abs(num180) * 0.1f;
								float num182 = player.position.Y + player.height * 0.5f - value9.Y - num181;
								float num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
								npc.netUpdate = true;
								num183 = num179 / num183;
								num180 *= num183;
								num182 *= num183;
								int num184 = expertMode ? 17 : 21;
								int num185 = Main.rand.Next(2);
								if (num185 == 0)
								{
									num185 = ModContent.ProjectileType<AbyssBallVolley>();
								}
								else
								{
									num185 = ModContent.ProjectileType<AbyssBallVolley2>();
									num184 = expertMode ? 16 : 19;
								}
								value9.X += num180;
								value9.Y += num182;
								for (int num186 = 0; num186 < 2; num186++)
								{
									num180 = player.position.X + player.width * 0.5f - value9.X;
									num182 = player.position.Y + player.height * 0.5f - value9.Y;
									num183 = (float)Math.Sqrt(num180 * num180 + num182 * num182);
									num183 = num179 / num183;
									num180 += Main.rand.Next(-30, 31);
									num182 += Main.rand.Next(-30, 31);
									num180 *= num183;
									num182 *= num183;
									Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
								}
							}
						}
					}
				}
            }

            float num1372 = death ? 14f : revenge ? 11f : expertMode ? 8.5f : 6f;
            if (phase2)
            {
                num1372 = revenge ? 14f : expertMode ? 12.5f : 11f;
            }
            if (CalamityWorld.bossRushActive || player.gravDir == -1f)
            {
                num1372 = 22f;
            }
            if (npc.Calamity().enraged > 0 || player.gravDir == -1f || (CalamityConfig.Instance.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                num1372 += 8f;
            }
			if (hyperMode)
			{
				num1372 *= 1.25f;
			}

            Vector2 vector167 = new Vector2(vectorCenter.X + (npc.direction * 20), vectorCenter.Y + 6f);
            float num1373 = player.position.X + player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;

            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
				npc.rotation += npc.direction * 0.3f;
				return;
            }

            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
			npc.rotation = npc.velocity.X * 0.1f;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color24 = npc.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && CalamityConfig.Instance.Afterimages)
            {
                Color color26 = npc.GetAlpha(color25);
                {
                    goto IL_6899;
                }
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
			}
			return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
            bool otherSlimeGodsAlive =
                NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>());
            if (!otherSlimeGodsAlive)
                DropSlimeGodLoot(npc);
        }

        // This loot code is shared with every other Slime God component.
        public static void DropSlimeGodLoot(NPC npc)
        {
            CalamityMod mod = ModContent.GetInstance<CalamityMod>();
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<SlimeGodTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSlimeGod>(), true, !CalamityWorld.downedSlimeGod);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSlimeGod, 3, 1, 0);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Dryad, ModContent.NPCType<THIEF>() }, CalamityWorld.downedSlimeGod);

			// Purified Jam is once per player, but drops for all players.
			CalamityPlayer mp = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity();
            if (!mp.revJamDrop)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<PurifiedJam>(), true, CalamityWorld.revenge && !CalamityWorld.downedSlimeGod, 6, 8);
                mp.revJamDrop = true;
            }

            // Gel always drops directly, even on Expert
            DropHelper.DropItemSpray(npc, ItemID.Gel, 180, 250);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<PurifiedGel>(), 30, 45);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<OverloadedBlaster>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AbyssalTome>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<EldritchTome>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CorroslimeStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CrimslimeStaff>(), 4);

                // Vanity
                DropHelper.DropItemFromSetChance(npc, 0.142857f, ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>());

                // Other
            }

            // Mark the Slime God as dead
            CalamityWorld.downedSlimeGod = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 40;
                npc.height = 40;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.VortexDebuff, 180, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 120, true);
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            }
        }
    }
}
