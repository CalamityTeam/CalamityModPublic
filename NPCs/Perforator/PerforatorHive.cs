using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHive : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private bool small = false;
        private bool medium = false;
        private bool large = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator Hive");
            Main.npcFrameCount[npc.type] = 10;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 18f;
			npc.GetNPCDamage();
			npc.width = 110;
            npc.height = 100;
            npc.defense = 4;
            npc.LifeMaxNERB(5000, 6000, 270000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 6, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath19;
			music = CalamityMod.Instance.GetMusicFromMusicMod("Perforators") ?? MusicID.Boss2;
            bossBag = ModContent.ItemType<PerforatorBag>();
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(biomeEnrageTimer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			biomeEnrageTimer = reader.ReadInt32();
		}

		public override void AI()
		{
			CalamityGlobalNPC.perfHive = npc.whoAmI;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Variables for ichor spore phase
			float sporePhaseGateValue = malice ? 450f : 600f;
			bool floatAboveToFireBlobs = npc.ai[2] >= sporePhaseGateValue - 120f;

			// Don't deal damage for 3 seconds after spawning or while firing blobs
			npc.damage = npc.defDamage;
			if (npc.ai[1] < 180f || floatAboveToFireBlobs)
			{
				if (npc.ai[1] < 180f)
					npc.ai[1] += 1f;

				npc.damage = 0;
			}

			Player player = Main.player[npc.target];

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases based on life percentage
			bool phase2 = lifeRatio < 0.7f;

			// Enrage
			if ((!player.ZoneCrimson || (npc.position.Y / 16f) < Main.worldSurface) && !BossRushEvent.BossRushActive)
			{
				if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
			}
			else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (biomeEnraged && (!player.ZoneCrimson || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}
			if (biomeEnraged && ((npc.position.Y / 16f) < Main.worldSurface || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}

			if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, npc.Center) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.04f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.1f;
					if (npc.velocity.Y > 12f)
						npc.velocity.Y = 12f;

					if (npc.timeLeft > 60)
						npc.timeLeft = 60;

					return;
				}
			}
			else if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			int wormsAlive = 0;
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
				wormsAlive++;
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>()))
				wormsAlive++;
			if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadSmall>()))
				wormsAlive++;

			if (npc.ai[3] == 0f && npc.life > 0)
				npc.ai[3] = npc.lifeMax;

			bool canSpawnWorms = !small || !medium || !large;
			if (npc.life > 0 && canSpawnWorms)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num660 = (int)(npc.lifeMax * 0.25);
					if ((npc.life + num660) < npc.ai[3])
					{
						npc.ai[3] = npc.life;
						int wormType = ModContent.NPCType<PerforatorHeadSmall>();
						if (!small)
						{
							small = true;
						}
						else if (!medium)
						{
							medium = true;
							wormType = ModContent.NPCType<PerforatorHeadMedium>();
						}
						else if (!large)
						{
							large = true;
							wormType = ModContent.NPCType<PerforatorHeadLarge>();
						}
						NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, wormType, 1);
						npc.TargetClosest();

						Main.PlaySound(SoundID.NPCDeath23, npc.position);

						for (int num621 = 0; num621 < 16; num621++)
						{
							int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 170, 0f, 0f, 100, default, 1f);
							Main.dust[num622].velocity *= 2f;
							if (Main.rand.NextBool(2))
							{
								Main.dust[num622].scale = 0.25f;
								Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
							}
						}

						for (int num623 = 0; num623 < 32; num623++)
						{
							int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
							Main.dust[num624].noGravity = true;
							Main.dust[num624].velocity *= 3f;
							num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1f);
							Main.dust[num624].velocity *= 2f;
						}
					}
				}
			}

			if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
			{
				float playerLocation = npc.Center.X - player.Center.X;
				npc.direction = playerLocation < 0f ? 1 : -1;
				npc.spriteDirection = npc.direction;
			}

			npc.rotation = npc.velocity.X * 0.04f;

			// Emit ichor blobs
			if (phase2)
			{
				if (wormsAlive == 0 || malice || floatAboveToFireBlobs)
				{
					npc.ai[2] += 1f;
					if (npc.ai[2] >= sporePhaseGateValue)
					{
						if (npc.ai[2] < sporePhaseGateValue + 300f)
						{
							if (npc.velocity.Length() > 0.5f)
								npc.velocity *= malice ? 0.94f : 0.96f;
							else
								npc.ai[2] = sporePhaseGateValue + 300f;
						}
						else
						{
							npc.ai[2] = 0f;

							Main.PlaySound(SoundID.NPCDeath23, npc.position);

							for (int num621 = 0; num621 < 32; num621++)
							{
								int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 170, 0f, 0f, 100, default, 1f);
								float dustVelocityYAdd = Math.Abs(Main.dust[num622].velocity.Y) * 0.5f;
								if (Main.dust[num622].velocity.Y < 0f)
									Main.dust[num622].velocity.Y = 2f + dustVelocityYAdd;
								if (Main.rand.NextBool(2))
								{
									Main.dust[num622].scale = 0.25f;
									Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
								}
							}

							int numBlobs = expertMode ? 6 : 3;
							int type = ModContent.ProjectileType<IchorBlob>();
							int damage = npc.GetProjectileDamage(type);

							for (int i = 0; i < numBlobs; i++)
							{
								Vector2 blobVelocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
								blobVelocity.Normalize();
								blobVelocity *= Main.rand.Next(200, 401) * (malice ? 0.02f : 0.01f);

								float sporeVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.5f;
								if (blobVelocity.Y < 2f)
									blobVelocity.Y = 2f + sporeVelocityYAdd;

								Projectile.NewProjectile(npc.Center, blobVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
							}
						}

						return;
					}
				}
			}

			// Movement velocities, increased while enraged
			float velocityEnrageIncrease = enrageScale;

			// When firing blobs, float above the target and don't call any other projectile firing or movement code
			if (floatAboveToFireBlobs)
			{
				if (revenge)
					Movement(player, 6f + velocityEnrageIncrease, 0.3f, 450f);
				else
					Movement(player, 5f + velocityEnrageIncrease, 0.2f, 450f);

				return;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int shoot = (revenge ? 6 : 4) - wormsAlive;
				npc.localAI[0] += Main.rand.Next(shoot);
				if (npc.localAI[0] >= Main.rand.Next(300, 901) && npc.position.Y + npc.height < player.position.Y && Vector2.Distance(player.Center, npc.Center) > 80f)
				{
					npc.localAI[0] = 0f;
					Main.PlaySound(SoundID.NPCHit20, npc.position);

					for (int num621 = 0; num621 < 8; num621++)
					{
						int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 170, 0f, 0f, 100, default, 1f);
						Main.dust[num622].velocity *= 3f;
						if (Main.rand.NextBool(2))
						{
							Main.dust[num622].scale = 0.25f;
							Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
						}
					}

					for (int num623 = 0; num623 < 16; num623++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1f);
						Main.dust[num624].velocity *= 2f;
					}

					int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
					int damage = npc.GetProjectileDamage(type);
					int totalProjectiles = death ? 16 : revenge ? 14 : expertMode ? 12 : 10;
					float maxVelocity = 8f;
					float velocityAdjustment = maxVelocity * 1.5f / totalProjectiles;
					Vector2 start = new Vector2(npc.Center.X, npc.Center.Y + 30f);
					Vector2 destination = wormsAlive > 0 ? new Vector2(Vector2.Normalize(player.Center - start).X, 0f) * maxVelocity * 0.4f : Vector2.Zero;
					Vector2 velocity = destination + Vector2.UnitY * -maxVelocity;
					for (int i = 0; i < totalProjectiles + 1; i++)
					{
						Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
						velocity.X += velocityAdjustment * npc.direction;
					}
				}
			}

			if (revenge)
			{
				if (wormsAlive == 1)
				{
					Movement(player, 4f + velocityEnrageIncrease, 0.1f, 350f);
				}
				else
				{
					if (large || death)
						Movement(player, 5.5f + velocityEnrageIncrease, death ? 0.075f : 0.065f, 20f);
					else if (medium)
						Movement(player, 5f + velocityEnrageIncrease, death ? 0.07f : 0.06f, 30f);
					else if (small)
						Movement(player, 4.5f + velocityEnrageIncrease, death ? 0.065f : 0.055f, 40f);
					else
						Movement(player, 4f + velocityEnrageIncrease, death ? 0.06f : 0.05f, 50f);
				}
			}
			else
				Movement(player, 4f + velocityEnrageIncrease, 0.05f, 350f);
		}

		private void Movement(Player target, float velocity, float acceleration, float y)
		{
			// Distance from destination where Perf Hive stops moving
			float movementDistanceGateValue = 100f;

			// This is where Perf Hive should be
			Vector2 destination = new Vector2(target.Center.X, target.Center.Y - y);

			// How far Perf Hive is from where it's supposed to be
			Vector2 distanceFromDestination = destination - npc.Center;

			// Inverse lerp returns the percentage of progress between A and B
			float lerpValue = Utils.InverseLerp(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

			// Min velocity
			float minVelocity = distanceFromDestination.Length();
			float minVelocityCap = velocity;
			if (minVelocity > minVelocityCap)
				minVelocity = minVelocityCap;

			// Max velocity
			Vector2 maxVelocity = distanceFromDestination / 24f;
			float maxVelocityCap = minVelocityCap * 3f;
			if (maxVelocity.Length() > maxVelocityCap)
				maxVelocity = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap;

			Vector2 desiredVelocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);
			npc.SimpleFlyMovement(desiredVelocity, acceleration);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorHiveGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CheckDead()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
            {
                return false;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<PerforatorTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePerforators>(), true, !CalamityWorld.downedPerforator);

			CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, CalamityWorld.downedPerforator);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
				// Materials
				DropHelper.DropItemSpray(npc, ModContent.ItemType<BloodSample>(), 35, 45, 5);
				DropHelper.DropItemSpray(npc, ItemID.CrimtaneBar, 12, 15);
				DropHelper.DropItemSpray(npc, ItemID.Vertebrae, 12, 15);
				if (Main.hardMode)
                    DropHelper.DropItemSpray(npc, ItemID.Ichor, 10, 20, 2);
                DropHelper.DropItem(npc, ItemID.CrimsonSeeds, 10, 15);

				// Weapons
				float w = DropHelper.NormalWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack<VeinBurster>(w),
					DropHelper.WeightStack<BloodyRupture>(w),
					DropHelper.WeightStack<SausageMaker>(w),
					DropHelper.WeightStack<Aorta>(w),
					DropHelper.WeightStack<Eviscerator>(w),
					DropHelper.WeightStack<BloodBath>(w),
					DropHelper.WeightStack<BloodClotStaff>(w),
					DropHelper.WeightStack<ToothBall>(w, 30, 50),
					DropHelper.WeightStack<BloodstainedGlove>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<BloodyWormTooth>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PerforatorMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodyVein>(), 10);
            }

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator)
            {
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
				CalamityUtils.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, 0.4f, 0.6f, 3, 8);

				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}

            // Mark The Perforator Hive as dead
            CalamityWorld.downedPerforator = true;
            CalamityNetcode.SyncWorld();
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Hive4"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 100;
                npc.height = 100;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
