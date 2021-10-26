using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Cryogen
{
    [AutoloadBossHead]
    public class Cryogen : ModNPC
    {
        private int time = 0;
        private int iceShard = 0;
        private int currentPhase = 1;
        private int teleportLocationX = 0;

        public override string Texture => "CalamityMod/NPCs/Cryogen/Cryogen_Phase1";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 24f;
			npc.GetNPCDamage();
			npc.width = 86;
            npc.height = 88;
            npc.defense = 12;
			npc.DR_NERD(0.3f);
            npc.LifeMaxNERB(18795, 27615, 300000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 10, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit5;
            npc.DeathSound = SoundID.NPCDeath15;
			music = CalamityMod.Instance.GetMusicFromMusicMod("Cryogen") ?? MusicID.FrostMoon;
            bossBag = ModContent.ItemType<CryogenBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(time);
            writer.Write(iceShard);
            writer.Write(teleportLocationX);
            writer.Write(npc.dontTakeDamage);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            time = reader.ReadInt32();
            iceShard = reader.ReadInt32();
            teleportLocationX = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 1f, 1f);

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			bool enraged = calamityGlobalNPC.enraged > 0;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || malice;

			float enrageScale = death ? 0.5f : 0f;
			if (!player.ZoneSnow || malice)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 2f;
			}

			if (enrageScale > 2f)
				enrageScale = 2f;

			if (BossRushEvent.BossRushActive)
				enrageScale = 3f;

			if (enraged)
			{
				npc.Calamity().CurrentlyEnraged = true;
				enrageScale = 4f;
			}

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			if (revenge)
			{
				// Increase aggression if player is taking a long time to kill the boss
				if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
					lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;
			}

			// Phases
			bool phase2 = lifeRatio < (revenge ? 0.85f : 0.8f);
			bool phase3 = lifeRatio < (revenge ? 0.7f : 0.6f);
			bool phase4 = lifeRatio < (revenge ? 0.55f : 0.4f);
			bool phase5 = lifeRatio < (revenge ? 0.4f : 0.2f);
			bool phase6 = lifeRatio < 0.25f && revenge;
			bool phase7 = lifeRatio < 0.15f && revenge;

			if ((int)npc.ai[0] + 1 > currentPhase)
                HandlePhaseTransition((int)npc.ai[0] + 1);

			if (npc.ai[2] == 0f && npc.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient && (npc.ai[0] < 3f || BossRushEvent.BossRushActive)) //spawn shield for phase 0 1 2, not 3 4 5
            {
                int num6 = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<CryogenIce>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                npc.ai[2] = num6 + 1;
                npc.localAI[1] = -1f;
                npc.netUpdate = true;
                Main.npc[num6].ai[0] = npc.whoAmI;
                Main.npc[num6].netUpdate = true;
            }

            int num7 = (int)npc.ai[2] - 1;
            if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<CryogenIce>())
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
                npc.ai[2] = 0f;

                if (npc.localAI[1] == -1f)
                    npc.localAI[1] = expertMode ? 720f : 1080f;
                if (npc.localAI[1] > 0f)
                    npc.localAI[1] -= 1f;
            }

			CalamityMod.StopRain();

			if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[1] != 0f)
					{
						npc.ai[1] = 0f;
						teleportLocationX = 0;
						iceShard = 0;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            if (Main.netMode != NetmodeID.MultiplayerClient && expertMode && (npc.ai[0] < 5f || !phase6))
            {
                time++;
                if (time >= (malice ? 480 : 600))
                {
					time = 0;
					Main.PlaySound(SoundID.Item28, npc.Center);
					int totalProjectiles = 4;
					float radians = MathHelper.TwoPi / totalProjectiles;
					int type = ModContent.ProjectileType<IceBomb>();
					int damage = npc.GetProjectileDamage(type);
					float velocity = 4f;
					double angleA = radians * 0.5;
					double angleB = MathHelper.ToRadians(90f) - angleA;
					float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
					Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
					}
                }
            }

			float chargeGateValue = malice ? 240f : 360f;

			if (npc.ai[0] == 0f)
            {
				npc.rotation = npc.velocity.X * 0.1f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        npc.localAI[0] = 0f;
						npc.TargetClosest();
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
							Main.PlaySound(SoundID.Item28, npc.Center);
							int totalProjectiles = malice ? 24 : 16;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int type = ModContent.ProjectileType<IceBlast>();
							int damage = npc.GetProjectileDamage(type);
							float velocity = 9f + enrageScale;
							Vector2 spinningPoint = new Vector2(0f, -velocity);
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
							}
                        }
                    }
                }

                Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float num1246 = revenge ? 5f : 4f;
				num1246 += 2f * enrageScale;

                num1245 = num1246 / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;

                if (phase2)
                {
					npc.TargetClosest();
					npc.ai[0] = 1f;
                    npc.localAI[0] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
				if (npc.ai[1] < chargeGateValue)
				{
					npc.ai[1] += 1f;

					npc.rotation = npc.velocity.X * 0.1f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[0] += 1f;
						if (npc.localAI[0] >= 120f)
						{
							npc.localAI[0] = 0f;
							npc.TargetClosest();
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								Main.PlaySound(SoundID.Item28, npc.Center);
								int totalProjectiles = malice ? 18 : 12;
								float radians = MathHelper.TwoPi / totalProjectiles;
								int type = ModContent.ProjectileType<IceBlast>();
								int damage = npc.GetProjectileDamage(type);
								float velocity2 = 9f + enrageScale;
								Vector2 spinningPoint = new Vector2(0f, -velocity2);
								for (int k = 0; k < totalProjectiles; k++)
								{
									Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
									Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
								}
							}
						}
					}

					float velocity = revenge ? 3.5f : 4f;
					float acceleration = 0.15f;
					velocity -= enrageScale * 0.8f;
					acceleration += 0.07f * enrageScale;

					if (npc.position.Y > player.position.Y - 375f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y *= 0.98f;

						npc.velocity.Y -= acceleration;

						if (npc.velocity.Y > velocity)
							npc.velocity.Y = velocity;
					}
					else if (npc.position.Y < player.position.Y - 425f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y *= 0.98f;

						npc.velocity.Y += acceleration;

						if (npc.velocity.Y < -velocity)
							npc.velocity.Y = -velocity;
					}

					if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 300f)
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X *= 0.98f;

						npc.velocity.X -= acceleration;

						if (npc.velocity.X > velocity)
							npc.velocity.X = velocity;
					}
					if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 300f)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X *= 0.98f;

						npc.velocity.X += acceleration;

						if (npc.velocity.X < -velocity)
							npc.velocity.X = -velocity;
					}
				}
				else if (npc.ai[1] < chargeGateValue + 30f)
				{
					npc.ai[1] += 1f;

					calamityGlobalNPC.newAI[0] += 0.025f;
					if (calamityGlobalNPC.newAI[0] > 0.5f)
						calamityGlobalNPC.newAI[0] = 0.5f;

					npc.rotation += calamityGlobalNPC.newAI[0];
					npc.velocity *= 0.98f;
				}
				else
				{
					if (npc.ai[1] == chargeGateValue + 30f)
					{
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * (18f + enrageScale * 2f);

						npc.ai[1] = chargeGateValue + 90f;
						calamityGlobalNPC.newAI[0] = 0f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								Main.PlaySound(SoundID.Item28, npc.Center);
								int type = ModContent.ProjectileType<IceRain>();
								int damage = npc.GetProjectileDamage(type);
								float velocity = 9f + enrageScale;
								for (int i = 0; i < 2; i++)
								{
									int totalProjectiles = 10;
									float radians = MathHelper.TwoPi / totalProjectiles;
									float newVelocity = velocity - (velocity * 0.5f * i);
									double angleA = radians * 0.5;
									double angleB = MathHelper.ToRadians(90f) - angleA;
									float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
									Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
									for (int k = 0; k < totalProjectiles; k++)
									{
										Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
										Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
									}
								}
							}
						}
					}

					npc.ai[1] -= 1f;
					if (npc.ai[1] == chargeGateValue + 30f)
					{
						npc.TargetClosest();

						npc.ai[1] = 0f;
						npc.localAI[0] = 0f;

						npc.rotation = npc.velocity.X * 0.1f;
					}
					else if (npc.ai[1] <= chargeGateValue + 45f)
					{
						npc.velocity *= 0.95f;
						npc.rotation = npc.velocity.X * 0.15f;
					}
					else
						npc.rotation += npc.direction * 0.5f;
				}

				if (phase3)
                {
					npc.TargetClosest();
					npc.ai[0] = 2f;
					npc.ai[1] = 0f;
					npc.localAI[0] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					iceShard = 0;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
				if (npc.ai[1] < chargeGateValue)
				{
					npc.ai[1] += 1f;

					npc.rotation = npc.velocity.X * 0.1f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.localAI[0] += 1f;
						if (npc.localAI[0] >= 120f)
						{
							npc.localAI[0] = 0f;
							npc.TargetClosest();
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								Main.PlaySound(SoundID.Item28, npc.Center);
								int totalProjectiles = malice ? 18 : 12;
								float radians = MathHelper.TwoPi / totalProjectiles;
								int type = ModContent.ProjectileType<IceBlast>();
								int damage = npc.GetProjectileDamage(type);
								float velocity = 9f + enrageScale;
								Vector2 spinningPoint = new Vector2(0f, -velocity);
								for (int k = 0; k < totalProjectiles; k++)
								{
									Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
									Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
								}
							}
						}
					}

					Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
					float num1243 = player.Center.X - vector142.X;
					float num1244 = player.Center.Y - vector142.Y;
					float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

					float num1246 = revenge ? 7f : 6f;
					num1246 += 2f * enrageScale;

					num1245 = num1246 / num1245;
					num1243 *= num1245;
					num1244 *= num1245;
					npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
					npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;
				}
				else if (npc.ai[1] < chargeGateValue + 20f)
				{
					npc.ai[1] += 1f;

					calamityGlobalNPC.newAI[0] += 0.025f;
					if (calamityGlobalNPC.newAI[0] > 0.5f)
						calamityGlobalNPC.newAI[0] = 0.5f;

					npc.rotation += calamityGlobalNPC.newAI[0];
					npc.velocity *= 0.98f;
				}
				else
				{
					if (npc.ai[1] == chargeGateValue + 20f)
					{
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * (18f + enrageScale * 2f);

						npc.ai[1] = chargeGateValue + 80f;
						calamityGlobalNPC.newAI[0] = 0f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								Main.PlaySound(SoundID.Item28, npc.Center);
								int type = ModContent.ProjectileType<IceRain>();
								int damage = npc.GetProjectileDamage(type);
								float velocity = 9f + enrageScale;
								for (int i = 0; i < 3; i++)
								{
									int totalProjectiles = 8;
									float radians = MathHelper.TwoPi / totalProjectiles;
									float newVelocity = velocity - (velocity * 0.33f * i);
									double angleA = radians * 0.5;
									double angleB = MathHelper.ToRadians(90f) - angleA;
									float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
									Vector2 spinningPoint = i == 1 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
									for (int k = 0; k < totalProjectiles; k++)
									{
										Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
										Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
									}
								}
							}
						}
					}

					npc.ai[1] -= 1f;
					if (npc.ai[1] == chargeGateValue + 20f)
					{
						npc.TargetClosest();

						calamityGlobalNPC.newAI[1] += 1f;
						if (calamityGlobalNPC.newAI[1] > 1f)
						{
							npc.ai[1] = 0f;
							npc.localAI[0] = 0f;
							calamityGlobalNPC.newAI[1] = 0f;
						}

						npc.rotation = npc.velocity.X * 0.1f;
					}
					else if (npc.ai[1] <= chargeGateValue + 35f)
					{
						npc.velocity *= 0.95f;
						npc.rotation = npc.velocity.X * 0.15f;
					}
					else
						npc.rotation += npc.direction * 0.5f;
				}

				if (phase4)
                {
					npc.TargetClosest();
					npc.ai[0] = 3f;
					npc.ai[1] = 0f;
                    npc.localAI[0] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
				npc.rotation = npc.velocity.X * 0.1f;

				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 90f && npc.Opacity == 1f)
                    {
                        npc.localAI[0] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
							Main.PlaySound(SoundID.Item28, npc.Center);
							int totalProjectiles = malice ? 18 : 12;
							float radians = MathHelper.TwoPi / totalProjectiles;
							int type = ModContent.ProjectileType<IceBlast>();
							int damage = npc.GetProjectileDamage(type);
							float velocity = 10f + enrageScale;
							Vector2 spinningPoint = new Vector2(0f, -velocity);
							for (int k = 0; k < totalProjectiles; k++)
							{
								Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
								Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
							}
                        }
                    }
                }

                Vector2 vector142 = new Vector2(npc.Center.X, npc.Center.Y);
                float num1243 = player.Center.X - vector142.X;
                float num1244 = player.Center.Y - vector142.Y;
                float num1245 = (float)Math.Sqrt(num1243 * num1243 + num1244 * num1244);

                float speed = revenge ? 5.5f : 5f;
				speed += 1.5f * enrageScale;

                num1245 = speed / num1245;
                num1243 *= num1245;
                num1244 *= num1245;
                npc.velocity.X = (npc.velocity.X * 50f + num1243) / 51f;
                npc.velocity.Y = (npc.velocity.Y * 50f + num1244) / 51f;

                if (npc.ai[1] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= 180f)
                        {
							npc.TargetClosest();
							npc.localAI[2] = 0f;
                            int num1249 = 0;
                            int num1250;
                            int num1251;
                            while (true)
                            {
                                num1249++;
                                num1250 = (int)player.Center.X / 16;
                                num1251 = (int)player.Center.Y / 16;

                                int min = 16;
                                int max = 20;

                                if (Main.rand.NextBool(2))
                                    num1250 += Main.rand.Next(min, max);
                                else
                                    num1250 -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool(2))
                                    num1251 += Main.rand.Next(min, max);
                                else
                                    num1251 -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(num1250, num1251) && Collision.CanHit(new Vector2(num1250 * 16, num1251 * 16), 1, 1, player.position, player.width, player.height))
                                    break;

                                if (num1249 > 100)
                                    goto Block;
                            }
                            npc.ai[1] = 1f;
                            teleportLocationX = num1250;
                            iceShard = num1251;
                            npc.netUpdate = true;
                            Block:
                            ;
                        }
                    }
                }
                else if (npc.ai[1] == 1f)
                {
                    Vector2 position = new Vector2(teleportLocationX * 16f - (npc.width / 2), iceShard * 16f - (npc.height / 2));
                    for (int m = 0; m < 5; m++)
                    {
                        int dust = Dust.NewDust(position, npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                    }

                    npc.Opacity -= 0.008f;
                    if (npc.Opacity <= 0f)
                    {
                        npc.Opacity = 0f;
                        npc.position = position;

                        for (int n = 0; n < 15; n++)
                        {
                            int num39 = Dust.NewDust(npc.position, npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                            Main.dust[num39].noGravity = true;
                        }

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
							{
								npc.localAI[0] = 0f;
								Main.PlaySound(SoundID.Item28, npc.Center);
								int type = ModContent.ProjectileType<IceRain>();
								int damage = npc.GetProjectileDamage(type);
								float velocity = 9f + enrageScale;
								for (int i = 0; i < 3; i++)
								{
									int totalProjectiles = malice ? 9 : 6;
									float radians = MathHelper.TwoPi / totalProjectiles;
									float newVelocity = velocity - (velocity * 0.33f * i);
									float velocityX = 0f;
									if (i > 0)
									{
										double angleA = radians * 0.33 * (3 - i);
										double angleB = MathHelper.ToRadians(90f) - angleA;
										velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
									}
									Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
									for (int k = 0; k < totalProjectiles; k++)
									{
										Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
										Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
									}
								}
							}
						}

						npc.ai[1] = 2f;
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[1] == 2f)
                {
                    npc.Opacity += 0.2f;
                    if (npc.Opacity >= 1f)
                    {
                        npc.Opacity = 1f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                }

				if (phase5)
                {
					npc.TargetClosest();
					npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
					npc.ai[3] = 0f;
                    npc.localAI[0] = 0f;
                    npc.localAI[2] = 0f;
					npc.Opacity = 1f;
                    teleportLocationX = 0;
                    iceShard = 0;
                    npc.netUpdate = true;

					int chance = 100;
					if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
						chance = 20;

					if (Main.rand.NextBool(chance))
					{
						string key = "Mods.CalamityMod.CryogenBossText";
						Color messageColor = Color.Cyan;
                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }
                }
            }
			else if (npc.ai[0] == 4f)
			{
                npc.damage = npc.defDamage;

				if (phase6)
				{
					if (npc.ai[1] == 60f)
					{
						npc.velocity = Vector2.Normalize(player.Center - npc.Center) * (18f + enrageScale * 2f);

						if (phase7)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
								{
									Main.PlaySound(SoundID.Item28, npc.Center);
									int type = ModContent.ProjectileType<IceRain>();
									int damage = npc.GetProjectileDamage(type);
									float velocity = 9f + enrageScale;
									for (int i = 0; i < 4; i++)
									{
										int totalProjectiles = malice ? 6 : 4;
										float radians = MathHelper.TwoPi / totalProjectiles;
										float newVelocity = velocity - (velocity * 0.25f * i);
										float velocityX = 0f;
										if (i > 0)
										{
											double angleA = radians * 0.25 * (4 - i);
											double angleB = MathHelper.ToRadians(90f) - angleA;
											velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
										}
										Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
										for (int k = 0; k < totalProjectiles; k++)
										{
											Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
											Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, velocity);
										}
									}
								}
							}
						}
					}

					npc.ai[1] -= 1f;
					if (npc.ai[1] <= 0f)
					{
						npc.ai[3] += 1f;
						npc.TargetClosest();
						if (npc.ai[3] > 2f)
						{
							npc.damage = 0;
							npc.defense = 0;
							npc.ai[0] = 5f;
							npc.ai[1] = 0f;
							npc.ai[3] = 0f;
							time = 0;
						}
						else
							npc.ai[1] = 60f;

						npc.rotation = npc.velocity.X * 0.1f;
					}
					else if (npc.ai[1] <= 15f)
					{
						npc.velocity *= 0.95f;
						npc.rotation = npc.velocity.X * 0.15f;
					}
					else
						npc.rotation += npc.direction * 0.5f;

					return;
				}

				float num1372 = 16f + enrageScale * 2f;

                Vector2 vector167 = new Vector2(npc.Center.X + (npc.direction * 20), npc.Center.Y + 6f);
                float num1373 = player.position.X + player.width * 0.5f - vector167.X;
                float num1374 = player.Center.Y - vector167.Y;
                float num1375 = (float)Math.Sqrt(num1373 * num1373 + num1374 * num1374);
                float num1376 = num1372 / num1375;
                num1373 *= num1376;
                num1374 *= num1376;
                iceShard--;

				if (num1375 < 200f || iceShard > 0)
				{
					if (num1375 < 200f)
						iceShard = 20;

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

                npc.rotation = npc.velocity.X * 0.15f;
            }
			else
			{
				npc.rotation = npc.velocity.X * 0.1f;

				time++;
				if (time >= (malice ? 50 : 75))
				{
					time = 0;
					Main.PlaySound(SoundID.Item28, npc.Center);
					int totalProjectiles = 4;
					float radians = MathHelper.TwoPi / totalProjectiles;
					int type = ModContent.ProjectileType<IceBomb>();
					int damage = npc.GetProjectileDamage(type);
					float velocity2 = 6f;
					double angleA = radians * 0.5;
					double angleB = MathHelper.ToRadians(90f) - angleA;
					float velocityX = (float)(velocity2 * Math.Sin(angleA) / Math.Sin(angleB));
					Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity2) : new Vector2(velocityX, -velocity2);
					for (int k = 0; k < totalProjectiles; k++)
					{
						Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
						Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
					}
				}

				npc.ai[1] += 1f;
				if (npc.ai[1] >= (malice ? 120f : 180f))
				{
					npc.TargetClosest();
					npc.ai[0] = 4f;
					npc.ai[1] = 60f;
					time = 0;
					iceShard = 0;
					npc.netUpdate = true;
				}

				float velocity = revenge ? 5f : 6f;
				float acceleration = 0.2f;
				velocity -= enrageScale;
				acceleration += 0.07f * enrageScale;

				if (npc.position.Y > player.position.Y - 375f)
				{
					if (npc.velocity.Y > 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y -= acceleration;

					if (npc.velocity.Y > velocity)
						npc.velocity.Y = velocity;
				}
				else if (npc.position.Y < player.position.Y - 400f)
				{
					if (npc.velocity.Y < 0f)
						npc.velocity.Y *= 0.98f;

					npc.velocity.Y += acceleration;

					if (npc.velocity.Y < -velocity)
						npc.velocity.Y = -velocity;
				}

				if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 350f)
				{
					if (npc.velocity.X > 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X -= acceleration;

					if (npc.velocity.X > velocity)
						npc.velocity.X = velocity;
				}
				if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 350f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X *= 0.98f;

					npc.velocity.X += acceleration;

					if (npc.velocity.X < -velocity)
						npc.velocity.X = -velocity;
				}
			}

			if (!phase5)
			{
				if (npc.ai[3] == 0f && npc.life > 0)
					npc.ai[3] = npc.lifeMax;

				if (npc.life > 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num660 = (int)(npc.lifeMax * 0.075);
						if ((npc.life + num660) < npc.ai[3])
						{
							npc.ai[3] = npc.life;
							for (int num662 = 0; num662 < 2; num662++)
							{
								int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
								int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));
								int random = 1;
								switch ((int)npc.ai[0])
								{
									case 0:
									case 1:
										break;
									case 2:
									case 3:
										random = 2;
										break;
									case 4:
										random = 3;
										break;
									default:
										break;
								}

								int randomSpawn = Main.rand.Next(random);
								if (randomSpawn == 0)
									randomSpawn = ModContent.NPCType<Cryocore>();
								else if (randomSpawn == 1)
									randomSpawn = ModContent.NPCType<IceMass>();
								else
									randomSpawn = ModContent.NPCType<Cryocore2>();

								int num664 = NPC.NewNPC(x, y, randomSpawn);
								if (Main.netMode == NetmodeID.Server && num664 < 200)
									NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664);
							}
						}
					}
				}
			}
        }

        private void HandlePhaseTransition(int newPhase)
        {
			Main.PlaySound(SoundID.NPCDeath15, npc.Center);
			int chipGoreAmount = newPhase >= 5 ? 3 : newPhase >= 3 ? 2 : 1;
            for (int i = 1; i < chipGoreAmount; i++)
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CryoChipGore" + i), 1f);

            currentPhase = newPhase;

			switch (currentPhase)
			{
				case 0:
				case 1:
					break;
				case 2:
					npc.Calamity().DR = 0.27f;
					break;
				case 3:
					npc.Calamity().DR = 0.21f;
					break;
				case 4:
					npc.Calamity().DR = 0.12f;
					break;
				case 5:
				case 6:
					npc.Calamity().DR = 0f;
					break;
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor) //for alt textures
        {
            if (currentPhase > 1)
            {
                string phase = "CalamityMod/NPCs/Cryogen/Cryogen_Phase" + currentPhase;
                Texture2D texture = ModContent.GetTexture(phase);

				SpriteEffects spriteEffects = SpriteEffects.None;
				if (npc.spriteDirection == 1)
					spriteEffects = SpriteEffects.FlipHorizontally;

				Vector2 origin = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
				Vector2 drawPos = npc.Center - Main.screenPosition;
				drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
				drawPos += origin * npc.scale + new Vector2(0f, npc.gfxOffY);
				spriteBatch.Draw(texture, drawPos, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);
				return false;
            }
            return true;
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
                Dust.NewDust(npc.position, npc.width, npc.height, 67, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 67, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = Main.rand.Next(-200, 200) / 100;
                for (int i = 1; i < 4; i++)
                {
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoDeathGore" + i), 1f);
                    Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/CryoChipGore" + i), 1f);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			// Legendary drops for Cryogen
			DropHelper.DropItemCondition(npc, ModContent.ItemType<ColdDivinity>(), true, CalamityWorld.malice);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<Cryophobia>(), true, CalamityWorld.malice);

			DropHelper.DropItemChance(npc, ModContent.ItemType<CryogenTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCryogen>(), true, !CalamityWorld.downedCryogen);

            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<EssenceofEleum>(), 4, 8);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<Avalanche>(w),
                    DropHelper.WeightStack<GlacialCrusher>(w),
                    DropHelper.WeightStack<EffluviumBow>(w),
                    DropHelper.WeightStack<BittercoldStaff>(w),
                    DropHelper.WeightStack<SnowstormStaff>(w),
                    DropHelper.WeightStack<Icebreaker>(w)
                );

				// Equipment
				DropHelper.DropItemChance(npc, ModContent.ItemType<FrostFlare>(), 5);
				DropHelper.DropItemChance(npc, ModContent.ItemType<CryoStone>(), 5);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<CryogenMask>(), 7);

                // Other
                DropHelper.DropItemChance(npc, ItemID.FrozenKey, 5);
            }

            // Spawn Permafrost if he isn't in the world
            int permafrostNPC = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permafrostNPC == -1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DILF>(), 0, 0f, 0f, 0f, 0f, 255);
            }

            // If Cryogen has not been killed, notify players about Cryonic Ore
            if (!CalamityWorld.downedCryogen)
            {
                string key = "Mods.CalamityMod.IceOreText";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.SpawnOre(ModContent.TileType<CryonicOre>(), 15E-05, 0.45f, 0.65f, 3, 8, TileID.SnowBlock, TileID.IceBlock, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, ModContent.TileType<AstralSnow>(), ModContent.TileType<AstralIce>());

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Cryogen as dead
            CalamityWorld.downedCryogen = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Frostburn, 120, true);
            player.AddBuff(BuffID.Chilled, 90, true);
        }
    }
}
