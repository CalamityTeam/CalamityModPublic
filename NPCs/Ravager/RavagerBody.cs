using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Ravager
{
    [AutoloadBossHead]
    public class RavagerBody : ModNPC
    {
		private float velocityY = -16f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ravager");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.lavaImmune = true;
			npc.noGravity = true;
            npc.npcSlots = 20f;
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 332;
            npc.height = 214;
            npc.defense = 55;
            npc.value = Item.buyPrice(0, 25, 0, 0);
			npc.DR_NERD(0.35f);
            npc.LifeMaxNERB(44600, 53500, 460000);
            if (CalamityWorld.downedProvidence && !BossRushEvent.BossRushActive)
            {
                npc.damage = (int)(npc.damage * 1.5);
                npc.defense *= 2;
                npc.lifeMax *= 4;
                npc.value *= 1.5f;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            aiType = -1;
            npc.boss = true;
            npc.alpha = 255;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.NPCDeath14;
            music = CalamityMod.Instance.GetMusicFromMusicMod("Ravager") ?? MusicID.Boss4;
            bossBag = ModContent.ItemType<RavagerBag>();
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToWater = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
			writer.Write(velocityY);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
			velocityY = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
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
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

			// Large fire light
			Lighting.AddLight((int)(npc.Center.X - 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);
            Lighting.AddLight((int)(npc.Center.X + 110f) / 16, (int)(npc.Center.Y - 30f) / 16, 0f, 0.5f, 2f);

            // Small fire light
            Lighting.AddLight((int)(npc.Center.X - 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);
            Lighting.AddLight((int)(npc.Center.X + 40f) / 16, (int)(npc.Center.Y - 60f) / 16, 0f, 0.25f, 1f);

            CalamityGlobalNPC.scavenger = npc.whoAmI;

            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC((int)npc.Center.X - 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegLeft>());
                NPC.NewNPC((int)npc.Center.X + 70, (int)npc.Center.Y + 88, ModContent.NPCType<RavagerLegRight>());
                NPC.NewNPC((int)npc.Center.X - 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawLeft>());
                NPC.NewNPC((int)npc.Center.X + 120, (int)npc.Center.Y + 50, ModContent.NPCType<RavagerClawRight>());
                NPC.NewNPC((int)npc.Center.X + 1, (int)npc.Center.Y - 20, ModContent.NPCType<RavagerHead>());
            }

            if (npc.target >= 0 && Main.player[npc.target].dead)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead)
                    npc.noTileCollide = true;
            }

			Player player = Main.player[npc.target];

            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                npc.ai[1] = 0f;
            }

            bool leftLegActive = false;
            bool rightLegActive = false;
            bool headActive = false;
            bool rightClawActive = false;
            bool leftClawActive = false;
			bool freeHeadActive = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerHead>())
                    headActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerClawRight>())
                    rightClawActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerClawLeft>())
                    leftClawActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerLegRight>())
                    rightLegActive = true;
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerLegLeft>())
                    leftLegActive = true;
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<RavagerHead2>())
					freeHeadActive = true;
			}

			bool anyHeadActive = headActive || freeHeadActive;
			bool immunePhase = headActive || rightClawActive || leftClawActive || rightLegActive || leftLegActive;
			bool finalPhase = !leftClawActive && !rightClawActive && !headActive && !leftLegActive && !rightLegActive && expertMode;
			bool phase2 = npc.ai[0] == 2f;
			bool reduceFallSpeed = npc.velocity.Y > 0f && npc.Bottom.Y > player.Top.Y;

			if (immunePhase)
			{
				npc.dontTakeDamage = true;
				if (malice)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && revenge)
							Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
					}
				}
			}
			else
			{
				npc.dontTakeDamage = false;
				if (Main.netMode != NetmodeID.Server)
				{
					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && revenge)
						Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<WeakPetrification>(), 2);
				}
			}

            if (!headActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2.5f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.Y -= 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y - 30f), 8, 8, DustID.Fire, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.Y -= 4f;
                    }
                }
            }

            if (!rightClawActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[rightDust].alpha += Main.rand.Next(100);
                Main.dust[rightDust].velocity *= 0.2f;

                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.velocity.X += 3f + Main.rand.Next(10) * 0.1f;
                Main.dust[rightDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 80f, npc.Center.Y + 45f), 8, 8, DustID.Fire, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[rightDust].noGravity = true;
                        Main.dust[rightDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.velocity.X += 4f;
                    }
                }
            }

            if (!leftClawActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 3f);
                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.alpha += Main.rand.Next(100);
                leftDustExpr.velocity *= 0.2f;
                leftDustExpr.velocity.X -= 3f + Main.rand.Next(10) * 0.1f;
                leftDustExpr.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 80f, npc.Center.Y + 45f), 8, 8, DustID.Fire, 0f, 0f, 0, default, 2f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.noGravity = true;
                        leftDustExpr2.scale *= 1f + Main.rand.Next(10) * 0.1f;
                        leftDustExpr2.velocity.X -= 4f;
                    }
                }
            }

            if (!rightLegActive)
            {
                int rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2f);
                Dust rightDustExpr = Main.dust[rightDust];
                rightDustExpr.alpha += Main.rand.Next(100);
                rightDustExpr.velocity *= 0.2f;
                rightDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                rightDustExpr.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    rightDust = Dust.NewDust(new Vector2(npc.Center.X + 60f, npc.Center.Y + 60f), 8, 8, DustID.Fire, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Dust rightDustExpr2 = Main.dust[rightDust];
                        rightDustExpr2.noGravity = true;
                        rightDustExpr2.scale *= 1f + Main.rand.Next(10) * 0.1f;
                        rightDustExpr2.velocity.Y += 1f;
                    }
                }
            }

            if (!leftLegActive)
            {
                int leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, DustID.Blood, 0f, 0f, 100, default, 2f);
                Main.dust[leftDust].alpha += Main.rand.Next(100);
                Main.dust[leftDust].velocity *= 0.2f;

                Dust leftDustExpr = Main.dust[leftDust];
                leftDustExpr.velocity.Y += 0.5f + Main.rand.Next(10) * 0.1f;
                Main.dust[leftDust].fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;

                if (Main.rand.NextBool(10))
                {
                    leftDust = Dust.NewDust(new Vector2(npc.Center.X - 60f, npc.Center.Y + 60f), 8, 8, DustID.Fire, 0f, 0f, 0, default, 1.5f);
                    if (Main.rand.Next(20) != 0)
                    {
                        Main.dust[leftDust].noGravity = true;
                        Main.dust[leftDust].scale *= 1f + Main.rand.Next(10) * 0.1f;
                        Dust leftDustExpr2 = Main.dust[leftDust];
                        leftDustExpr2.velocity.Y += 1f;
                    }
                }
            }

			if (npc.noTileCollide && !player.dead)
			{
				if (npc.velocity.Y > 0f && npc.Bottom.Y > player.Top.Y)
					npc.noTileCollide = false;
				else if (Collision.CanHit(npc.position, npc.width, npc.height, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
					npc.noTileCollide = false;
			}

			if (npc.ai[0] == 0f)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
						if (revenge)
						{
							if (calamityGlobalNPC.newAI[0] % 3f == 0f)
								npc.ai[1] += 1f;
							else if (calamityGlobalNPC.newAI[0] % 2f == 0f)
								npc.ai[1] += 1f;
						}

						if (!rightClawActive && !leftClawActive)
                            npc.ai[1] += 1f;
                        if (!headActive)
                            npc.ai[1] += 1f;
                        if (!rightLegActive && !leftLegActive)
                            npc.ai[1] += 1f;
                    }

					float jumpGateValue = 180f;
					if (npc.ai[1] >= jumpGateValue)
					{
						npc.ai[1] = -20f;
					}
					else if (npc.ai[1] == -1f)
					{
						npc.noTileCollide = true;

						npc.TargetClosest();

						bool shouldFall = player.position.Y >= npc.Bottom.Y;
						float velocityXBoost = !anyHeadActive ? 6f : death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
						float velocityX = 4f + velocityXBoost;
						velocityY = -16f;

						float distanceBelowTarget = npc.position.Y - (player.position.Y + 80f);

						if (revenge)
						{
							float multiplier = malice ? 0.003f : 0.0015f;
							if (distanceBelowTarget > 0f)
								calamityGlobalNPC.newAI[1] += 1f + distanceBelowTarget * multiplier;

							float speedMultLimit = malice ? 3f : 2f;
							if (calamityGlobalNPC.newAI[1] > speedMultLimit)
								calamityGlobalNPC.newAI[1] = speedMultLimit;

							if (calamityGlobalNPC.newAI[1] > 1f)
								velocityY *= calamityGlobalNPC.newAI[1];
						}

						if (expertMode && !finalPhase)
						{
							if (shouldFall)
								velocityY = 1f;

							if (calamityGlobalNPC.newAI[0] % 3f == 0f)
							{
								velocityX *= 2f;
								if (!shouldFall)
									velocityY *= 0.5f;
							}
							else if (calamityGlobalNPC.newAI[0] % 2f == 0f)
							{
								velocityX *= 1.5f;
								if (!shouldFall)
									velocityY *= 0.75f;
							}
						}

						if (finalPhase)
							calamityGlobalNPC.newAI[2] = player.direction;

						float playerLocation = npc.Center.X - player.Center.X;
						npc.direction = playerLocation < 0 ? 1 : -1;

						npc.velocity.X = velocityX * npc.direction;
						npc.velocity.Y = velocityY;

						npc.ai[0] = finalPhase ? 2f : 1f;
						npc.ai[1] = 0f;
					}
                }

				CustomGravity();
			}
            else if (npc.ai[0] >= 1f)
            {
                if (npc.velocity.Y == 0f && (npc.ai[1] == 31f || npc.ai[0] == 1f))
                {
					Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 14, 1.25f, -0.25f);

					npc.ai[0] = 0f;
					npc.ai[1] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
						if (expertMode)
						{
							for (int i = 0; i < Main.maxNPCs; i++)
							{
                                if (Main.npc[i].type == ModContent.NPCType<RockPillar>() && Main.npc[i].ai[0] == 0f)
                                {
                                    Main.npc[i].ai[1] = -1f;
                                    Main.npc[i].direction = npc.direction;
                                    Main.npc[i].netUpdate = true;
                                }
							}

							int spawnDistance = 360;

							if (!NPC.AnyNPCs(ModContent.NPCType<RockPillar>()))
							{
								NPC.NewNPC((int)(player.Center.X - spawnDistance * 1.25f), (int)player.Center.Y - 100, ModContent.NPCType<RockPillar>());
								NPC.NewNPC((int)(player.Center.X + spawnDistance * 1.25f), (int)player.Center.Y - 100, ModContent.NPCType<RockPillar>());
							}
							else if (!NPC.AnyNPCs(ModContent.NPCType<FlamePillar>()))
							{
								float distanceMultiplier = finalPhase ? 2.5f : 2f;
								NPC.NewNPC((int)player.Center.X - (int)(spawnDistance * distanceMultiplier), (int)player.Center.Y - 100, ModContent.NPCType<FlamePillar>());
								NPC.NewNPC((int)player.Center.X + (int)(spawnDistance * distanceMultiplier), (int)player.Center.Y - 100, ModContent.NPCType<FlamePillar>());
							}
						}
                    }

					if (revenge)
						calamityGlobalNPC.newAI[0] += 1f;

					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
					npc.TargetClosest();

					for (int stompDustArea = (int)npc.position.X - 30; stompDustArea < (int)npc.position.X + npc.width + 60; stompDustArea += 30)
                    {
                        for (int stompDustAmount = 0; stompDustAmount < 6; stompDustAmount++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(npc.position.X - 30f, npc.position.Y + npc.height), npc.width + 30, 4, 31, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        int stompGore = Gore.NewGore(new Vector2(stompDustArea - 30, npc.position.Y + npc.height - 12f), default, Main.rand.Next(61, 64), 1f);
                        Main.gore[stompGore].velocity *= 0.4f;
                    }
                }
                else
                {
					Vector2 targetVector = player.position;
					float aimY = targetVector.Y - 640f;
					float distanceFromTargetPos = Math.Abs(npc.Top.Y - aimY);
					bool inRange = npc.Top.Y <= aimY + 160f && npc.Top.Y >= aimY - 16f;

					if (phase2 && npc.ai[1] == 0f)
					{
						npc.noTileCollide = true;

						calamityGlobalNPC.newAI[3] += 1f;

						if (inRange)
							npc.velocity.Y = 0f;
						else if (npc.Top.Y > aimY)
							npc.velocity.Y -= 0.2f + distanceFromTargetPos * 0.001f;
						else
							npc.velocity.Y += 0.2f + distanceFromTargetPos * 0.001f;

						if (npc.velocity.Y < velocityY)
							npc.velocity.Y = velocityY;
						if (npc.velocity.Y > -velocityY)
							npc.velocity.Y = -velocityY;
					}

					float maxOffsetScale = death ? 320f : 240f;
					float maxOffset = maxOffsetScale * (1f - lifeRatio);
					float offset = phase2 ? maxOffset * calamityGlobalNPC.newAI[2] : 0f;

					// Set offset to 0 if the target stops moving
					if (Math.Abs(player.velocity.X) < 0.5f)
						calamityGlobalNPC.newAI[2] = 0f;
					else
						calamityGlobalNPC.newAI[2] = player.direction;

					if ((npc.position.X < targetVector.X + offset && npc.position.X + npc.width > targetVector.X + player.width + offset && (inRange || npc.ai[0] != 2f)) || npc.ai[1] > 0f || calamityGlobalNPC.newAI[3] >= 180f)
                    {
						npc.damage = npc.defDamage;

						if (phase2)
						{
							float stopBeforeFallTime = malice ? 25f : 30f;
							if (!anyHeadActive)
								stopBeforeFallTime -= 15f;
							else if (expertMode)
								stopBeforeFallTime -= death ? 15f * (1f - lifeRatio) : 10f * (1f - lifeRatio);

							if (npc.ai[1] < stopBeforeFallTime)
							{
								npc.ai[1] += 1f;
								npc.velocity = Vector2.Zero;
							}
							else
							{
								float fallSpeedBoost = !anyHeadActive ? 1.8f : death ? 1.8f * (1f - lifeRatio) : 1.2f * (1f - lifeRatio);
								float fallSpeed = (malice ? 1.8f : 1.2f) + fallSpeedBoost;

								if (calamityGlobalNPC.newAI[1] > 1f)
									fallSpeed *= calamityGlobalNPC.newAI[1];

								npc.velocity.Y += fallSpeed;

								npc.ai[1] = 31f;
							}
						}
						else
						{
							npc.velocity.X *= 0.9f;

							if (npc.Bottom.Y < player.position.Y)
							{
								float fallSpeedBoost = !anyHeadActive ? 0.9f : death ? 0.9f * (1f - lifeRatio) : 0.6f * (1f - lifeRatio);
								float fallSpeed = (malice ? 0.9f : 0.6f) + fallSpeedBoost;

								if (calamityGlobalNPC.newAI[1] > 1f)
									fallSpeed *= calamityGlobalNPC.newAI[1];

								npc.velocity.Y += fallSpeed;
							}
						}
                    }
                    else
                    {
						float velocityMult = malice ? 2f : 1.8f;
						float velocityXChange = 0.2f + Math.Abs(npc.Center.X - player.Center.X) * 0.001f;

						float velocityXBoost = !anyHeadActive ? 6f : death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
						float velocityX = 8f + velocityXBoost + Math.Abs(npc.Center.X - player.Center.X) * 0.001f;

						if (!rightClawActive)
							velocityX += 1f;
						if (!leftClawActive)
							velocityX += 1f;
						if (!headActive)
							velocityX += 1f;
						if (!rightLegActive)
							velocityX += 1f;
						if (!leftLegActive)
							velocityX += 1f;

						if (phase2)
						{
							npc.damage = 0;
							velocityXChange *= velocityMult;
							velocityX *= velocityMult;
						}

						if (npc.direction < 0)
                            npc.velocity.X -= velocityXChange;
                        else if (npc.direction > 0)
                            npc.velocity.X += velocityXChange;

                        if (npc.velocity.X < -velocityX)
                            npc.velocity.X = -velocityX;
                        if (npc.velocity.X > velocityX)
                            npc.velocity.X = velocityX;
                    }

					CustomGravity();
				}
            }

			void CustomGravity()
			{
				float gravity = phase2 ? 0f : 0.45f;
				float maxFallSpeed = reduceFallSpeed ? 12f : phase2 ? 24f : 15f;
				if (malice && !reduceFallSpeed)
				{
					gravity *= 1.25f;
					maxFallSpeed *= 1.25f;
				}

				if (calamityGlobalNPC.newAI[1] > 1f && !reduceFallSpeed)
					maxFallSpeed *= calamityGlobalNPC.newAI[1];

				npc.velocity.Y += gravity;
				if (npc.velocity.Y > maxFallSpeed)
					npc.velocity.Y = maxFallSpeed;
			}

			player = Main.player[npc.target];
			if (npc.target <= 0 || npc.target == 255 || player.dead || !player.active)
			{
				npc.TargetClosest();
				player = Main.player[npc.target];
			}

            int distanceFromTarget = 5600;
            if (Vector2.Distance(npc.Center, player.Center) > distanceFromTarget)
            {
                npc.TargetClosest();
				player = Main.player[npc.target];

				if (Vector2.Distance(npc.Center, player.Center) > distanceFromTarget)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }
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
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow").Height / Main.npcFrameCount[npc.type]) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Color.Blue);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerBodyGlow"), vector,
                npc.frame, color, npc.rotation, vector11, 1f, spriteEffects, 0f);
            Color color2 = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight"), new Vector2(center.X - Main.screenPosition.X + 28f, center.Y - Main.screenPosition.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegRight").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft"), new Vector2(center.X - Main.screenPosition.X - 112f, center.Y - Main.screenPosition.Y + 20f), //72
                new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerLegLeft").Height)),
                color2, 0f, default, 1f, SpriteEffects.None, 0f);
            if (NPC.AnyNPCs(ModContent.NPCType<RavagerHead>()))
            {
                spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead"), new Vector2(center.X - Main.screenPosition.X - 70f, center.Y - Main.screenPosition.Y - 75f),
                    new Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead").Width, ModContent.GetTexture("CalamityMod/NPCs/Ravager/RavagerHead").Height)),
                    color2, 0f, default, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 2f);
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScavengerGores/ScavengerBody5"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 2f);
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Fire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

		// Can only hit the target if within certain distance
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			Vector2 npcCenter = npc.Center;

			// NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
			// Width = 83, Height = 107
			Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - (npc.width / 2f) + 8f), (int)(npcCenter.Y - (npc.height / 4f)), npc.width / 4, npc.height / 2);
			// Width = 166, Height = 214
			Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (npc.width / 4f)), (int)(npcCenter.Y - (npc.height / 2f) + 8f), npc.width / 2, npc.height);
			// Width = 83, Height = 107
			Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + (npc.width / 4f) - 8f), (int)(npcCenter.Y - (npc.height / 4f)), npc.width / 4, npc.height / 2);

			Vector2 leftHitboxCenter = new Vector2(leftHitbox.X + (leftHitbox.Width / 2), leftHitbox.Y + (leftHitbox.Height / 2));
			Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
			Vector2 rightHitboxCenter = new Vector2(rightHitbox.X + (rightHitbox.Width / 2), rightHitbox.Y + (rightHitbox.Height / 2));

			Rectangle targetHitbox = target.Hitbox;

			float leftDist1 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopLeft());
			float leftDist2 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopRight());
			float leftDist3 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomLeft());
			float leftDist4 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomRight());

			float minLeftDist = leftDist1;
			if (leftDist2 < minLeftDist)
				minLeftDist = leftDist2;
			if (leftDist3 < minLeftDist)
				minLeftDist = leftDist3;
			if (leftDist4 < minLeftDist)
				minLeftDist = leftDist4;

			bool insideLeftHitbox = minLeftDist <= 55f;

			float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
			float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
			float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
			float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

			float minBodyDist = bodyDist1;
			if (bodyDist2 < minBodyDist)
				minBodyDist = bodyDist2;
			if (bodyDist3 < minBodyDist)
				minBodyDist = bodyDist3;
			if (bodyDist4 < minBodyDist)
				minBodyDist = bodyDist4;

			bool insideBodyHitbox = minBodyDist <= 110f;

			float rightDist1 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopLeft());
			float rightDist2 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopRight());
			float rightDist3 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomLeft());
			float rightDist4 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomRight());

			float minRightDist = rightDist1;
			if (rightDist2 < minRightDist)
				minRightDist = rightDist2;
			if (rightDist3 < minRightDist)
				minRightDist = rightDist3;
			if (rightDist4 < minRightDist)
				minRightDist = rightDist4;

			bool insideRightHitbox = minRightDist <= 55f;

			return insideLeftHitbox || insideBodyHitbox || insideRightHitbox;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300, true);
		}

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "Ravager";
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<RavagerTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeRavager>(), true, !CalamityWorld.downedScavenger);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
				DropHelper.DropItemCondition(npc, ModContent.ItemType<FleshyGeodeT1>(), !CalamityWorld.downedProvidence);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<FleshyGeodeT2>(), CalamityWorld.downedProvidence);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<UltimusCleaver>(w),
                    DropHelper.WeightStack<RealmRavager>(w),
                    DropHelper.WeightStack<Hematemesis>(w),
                    DropHelper.WeightStack<SpikecragStaff>(w),
                    DropHelper.WeightStack<CraniumSmasher>(w),
					DropHelper.WeightStack<Vesuvius>(w)
				);

				// Equipment
				DropHelper.DropItemCondition(npc, ModContent.ItemType<BloodflareCore>(), true, CalamityWorld.downedProvidence);
				DropHelper.DropItemChance(npc, ModContent.ItemType<BloodPact>(), 3);
                DropHelper.DropItemChance(npc, ModContent.ItemType<FleshTotem>(), 3);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<RavagerMask>(), 7);
            }

            // Mark Ravager as dead
            CalamityWorld.downedScavenger = true;
            CalamityNetcode.SyncWorld();
        }
    }
}
