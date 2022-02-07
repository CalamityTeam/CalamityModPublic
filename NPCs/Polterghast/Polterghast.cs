using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Polterghast
{
    public class Polterghast : ModNPC
    {
		public static int phase1IconIndex;
		public static int phase3IconIndex;

		internal static void LoadHeadIcons()
		{
			string phase1IconPath = "CalamityMod/NPCs/Polterghast/Polterghast_Head_Boss";
			string phase3IconPath = "CalamityMod/NPCs/Polterghast/Necroplasm_Head_Boss";

			CalamityMod.Instance.AddBossHeadTexture(phase1IconPath, -1);
			phase1IconIndex = ModContent.GetModBossHeadSlot(phase1IconPath);

			CalamityMod.Instance.AddBossHeadTexture(phase3IconPath, -1);
			phase3IconIndex = ModContent.GetModBossHeadSlot(phase3IconPath);
		}

		private int despawnTimer = 600;
		private bool reachedChargingPoint = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[npc.type] = 12;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
            npc.npcSlots = 50f;
			npc.GetNPCDamage();
			npc.width = 90;
            npc.height = 120;
            npc.defense = 90;
			npc.DR_NERD(0.2f);
            npc.LifeMaxNERB(350000, 420000, 325000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 60, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("Polterghast") ?? MusicID.Plantera;
            npc.HitSound = SoundID.NPCHit7;
            npc.DeathSound = SoundID.NPCDeath39;
            bossBag = ModContent.ItemType<PolterghastBag>();
			npc.Calamity().VulnerableToSickness = false;
		}

		public override void BossHeadSlot(ref int index)
		{
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			bool phase3 = npc.life / (float)npc.lifeMax < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
			if (phase3)
				index = phase3IconIndex;
			else
				index = phase1IconIndex;
		}

		public override void BossHeadRotation(ref float rotation)
		{
			if (npc.HasValidTarget && npc.Calamity().newAI[3] == 0f)
				rotation = (Main.player[npc.TranslatedTargetIndex].Center - npc.Center).ToRotation() + MathHelper.PiOver2;
			else
				rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
			writer.Write(reachedChargingPoint);
			CalamityGlobalNPC cgn = npc.Calamity();
			writer.Write(cgn.newAI[0]);
			writer.Write(cgn.newAI[1]);
			writer.Write(cgn.newAI[2]);
			writer.Write(cgn.newAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
			reachedChargingPoint = reader.ReadBoolean();
			CalamityGlobalNPC cgn = npc.Calamity();
			cgn.newAI[0] = reader.ReadSingle();
			cgn.newAI[1] = reader.ReadSingle();
			cgn.newAI[2] = reader.ReadSingle();
			cgn.newAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // Emit light
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.1f, 0.5f, 0.5f);

			// whoAmI variable
			CalamityGlobalNPC.ghostBoss = npc.whoAmI;

            // Detect clone
            bool cloneAlive = false;
            if (CalamityGlobalNPC.ghostBossClone != -1)
                cloneAlive = Main.npc[CalamityGlobalNPC.ghostBossClone].active;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Variables
			Vector2 vector = npc.Center;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool speedBoost = false;
            bool despawnBoost = false;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Phases
			bool phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f);
            bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
            bool phase4 = lifeRatio < (death ? 0.45f : revenge ? 0.35f : expertMode ? 0.2f : 0.1f);
            bool phase5 = lifeRatio < (death ? 0.2f : revenge ? 0.15f : expertMode ? 0.1f : 0.05f);

			// Get angry if the clone is dead and in phase 3
			bool getPissed = !cloneAlive && phase3;

			// Velocity and acceleration
			calamityGlobalNPC.newAI[0] += 1f;
			bool chargePhase = calamityGlobalNPC.newAI[0] >= 480f;
			int chargeAmt = getPissed ? 4 : phase3 ? 3 : phase2 ? 2 : 1;
			float chargeVelocity = getPissed ? 28f : phase3 ? 24f : phase2 ? 22f : 20f;
			float chargeAcceleration = getPissed ? 0.7f : phase3 ? 0.6f : phase2 ? 0.55f : 0.5f;
			float chargeDistance = 480f;
			bool charging = npc.ai[2] >= 300f;
			bool reset = npc.ai[2] >= 600f;
			float speedUpDistance = 480f - 360f * (1f - lifeRatio);

			// Only get a new target while not charging
			if (!chargePhase)
			{
				// Get a target
				if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
					npc.TargetClosest();

				// Despawn safety, make sure to target another player if the current player target is too far away
				if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
					npc.TargetClosest();
			}

			Player player = Main.player[npc.target];
			bool speedUp = Vector2.Distance(player.Center, vector) > speedUpDistance; // 30 or 40 tile distance
			float velocity = 10f; // Max should be 21
			float acceleration = 0.05f; // Max should be 0.13

			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					speedBoost = true;
					despawnBoost = true;
					reachedChargingPoint = false;
					npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;

					if (cloneAlive)
					{
						Main.npc[CalamityGlobalNPC.ghostBossClone].ai[0] = 0f;
						Main.npc[CalamityGlobalNPC.ghostBossClone].ai[1] = 0f;
						Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[0] = 0f;
						Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1] = 0f;
						Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2] = 0f;
						Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[3] = 0f;
					}
				}
			}

            // Stop rain
            CalamityMod.StopRain();

            // Set time left
            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Spawn hooks
            if (npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] = 1f;
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
				NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterghastHook>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
            }

            if (!player.ZoneDungeon && !BossRushEvent.BossRushActive && player.position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
				if (despawnTimer <= 0)
				{
					despawnBoost = true;
					npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;
				}

                speedBoost = true;
				velocity += 5f;
				acceleration += 0.05f;
            }
            else
                despawnTimer++;

            // Despawn
            if (Vector2.Distance(player.Center, vector) > (despawnBoost ? 1500f : 6000f))
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			if (phase2)
            {
				velocity += 2.5f;
				acceleration += 0.02f;
			}

			if (!phase3)
			{
				if (charging)
				{
					velocity += phase2 ? 4.5f : 3.5f;
					acceleration += phase2 ? 0.03f : 0.025f;
				}
			}
			else
			{
				if (charging)
				{
					velocity += phase5 ? 8.5f : 4.5f;
					acceleration += phase5 ? 0.06f : 0.03f;
				}
				else
				{
					if (phase5)
					{
						velocity += 1.5f;
						acceleration += 0.015f;
					}
					else if (phase4)
					{
						velocity += 1f;
						acceleration += 0.01f;
					}
					else
					{
						velocity += 0.5f;
						acceleration += 0.005f;
					}
				}
			}

			if (expertMode)
			{
				chargeVelocity += revenge ? 4f : 2f;
				velocity += revenge ? 5f : 3.5f;
				acceleration += revenge ? 0.035f : 0.025f;
			}

			// Slow down if close to target and not inside tiles
			if (!speedUp && !Collision.SolidCollision(npc.position, npc.width, npc.height) && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && !charging && !chargePhase)
			{
				velocity = 8f;
				acceleration = 0.035f;
			}

			// Detect active tiles around Polterghast
			int radius = 30; // 30 tile radius
			int diameter = radius * 2;
			int npcCenterX = (int)(vector.X / 16f);
			int npcCenterY = (int)(vector.Y / 16f);
			Rectangle area = new Rectangle(npcCenterX - radius, npcCenterY - radius, diameter, diameter);
			int nearbyActiveTiles = 0; // 0 to 3600
			for (int x = area.Left; x < area.Right; x++)
			{
				for (int y = area.Top; y < area.Bottom; y++)
				{
					if (Main.tile[x, y] != null)
					{
						if (Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type] && !Main.tileSolidTop[Main.tile[x, y].type] && !TileID.Sets.Platforms[Main.tile[x, y].type])
							nearbyActiveTiles++;
					}
				}
			}

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = 1f;
			if (nearbyActiveTiles < 1000)
				tileEnrageMult += (1000 - nearbyActiveTiles) * 0.00075f; // Ranges from 1f to 1.75f

			if (malice)
				tileEnrageMult = 1.75f;

			npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && tileEnrageMult >= 1.6f;

			// Used to inform clone and hooks about number of active tiles nearby
			npc.ai[3] = tileEnrageMult;

			// Increase projectile fire rate based on number of nearby active tiles
			float amount = 1f - ((tileEnrageMult - 1f) / 0.75f);
			if (amount < 0f)
				amount = 0f;
			float projectileFireRateMultiplier = MathHelper.Lerp(1f, 2f, amount);

			// Increase projectile stats based on number of nearby active tiles
			int baseProjectileTimeLeft = (int)(1200f * tileEnrageMult);
			int baseProjectileAmt = (int)(4f * tileEnrageMult);
			int baseProjectileSpread = (int)(45f * tileEnrageMult);
			float baseProjectileVelocity = 5f * tileEnrageMult;
			if (speedBoost)
				baseProjectileVelocity *= 1.25f;

			// Predictiveness
			Vector2 predictionVector = chargePhase && malice ? player.velocity * 20f : Vector2.Zero;
			Vector2 lookAt = player.Center + predictionVector;
			Vector2 rotationVector = lookAt - vector;

			// Rotation
			if (calamityGlobalNPC.newAI[3] == 0f)
			{
				float num740 = player.Center.X + predictionVector.X - vector.X;
				float num741 = player.Center.Y + predictionVector.Y - vector.Y;
				npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;
			}
			else
				npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

			if (!chargePhase)
			{
				npc.ai[2] += 1f;
				if (reset)
				{
					npc.ai[2] = 0f;
					npc.netUpdate = true;
				}

				float movementLimitX = 0f;
				float movementLimitY = 0f;
				int numHooks = 4;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<PolterghastHook>())
					{
						movementLimitX += Main.npc[i].Center.X;
						movementLimitY += Main.npc[i].Center.Y;
					}
				}
				movementLimitX /= numHooks;
				movementLimitY /= numHooks;

				Vector2 vector91 = new Vector2(movementLimitX, movementLimitY);
				float num736 = player.Center.X - vector91.X;
				float num737 = player.Center.Y - vector91.Y;

				if (despawnBoost)
				{
					num737 *= -1f;
					num736 *= -1f;
					velocity += 10f;
				}

				float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);
				float maxDistanceFromHooks = expertMode ? 650f : 500f;
				if (speedBoost || malice)
					maxDistanceFromHooks += 250f;
				if (death)
					maxDistanceFromHooks += maxDistanceFromHooks * 0.1f * (1f - lifeRatio);

				// Increase speed based on nearby active tiles
				velocity *= tileEnrageMult;
				acceleration *= tileEnrageMult;

				if (death)
				{
					velocity += velocity * 0.15f * (1f - lifeRatio);
					acceleration += acceleration * 0.15f * (1f - lifeRatio);
				}

				if (num738 >= maxDistanceFromHooks)
				{
					num738 = maxDistanceFromHooks / num738;
					num736 *= num738;
					num737 *= num738;
				}

				movementLimitX += num736;
				movementLimitY += num737;
				num736 = movementLimitX - vector.X;
				num737 = movementLimitY - vector.Y;
				num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

				if (num738 < velocity)
				{
					num736 = npc.velocity.X;
					num737 = npc.velocity.Y;
				}
				else
				{
					num738 = velocity / num738;
					num736 *= num738;
					num737 *= num738;
				}

				if (npc.velocity.X < num736)
				{
					npc.velocity.X += acceleration;
					if (npc.velocity.X < 0f && num736 > 0f)
						npc.velocity.X += acceleration * 2f;
				}
				else if (npc.velocity.X > num736)
				{
					npc.velocity.X -= acceleration;
					if (npc.velocity.X > 0f && num736 < 0f)
						npc.velocity.X -= acceleration * 2f;
				}
				if (npc.velocity.Y < num737)
				{
					npc.velocity.Y += acceleration;
					if (npc.velocity.Y < 0f && num737 > 0f)
						npc.velocity.Y += acceleration * 2f;
				}
				else if (npc.velocity.Y > num737)
				{
					npc.velocity.Y -= acceleration;
					if (npc.velocity.Y > 0f && num737 < 0f)
						npc.velocity.Y -= acceleration * 2f;
				}

				// Slow down considerably if near player
				if (!speedUp && nearbyActiveTiles > 1000 && !Collision.SolidCollision(npc.position, npc.width, npc.height) && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && !charging)
				{
					if (npc.velocity.Length() > velocity)
						npc.velocity *= 0.97f;
				}
			}
			else
			{
				// Charge
				if (calamityGlobalNPC.newAI[3] == 1f)
				{
					reachedChargingPoint = false;

					if (calamityGlobalNPC.newAI[1] == 0f)
					{
						npc.velocity = Vector2.Normalize(rotationVector) * chargeVelocity;
						calamityGlobalNPC.newAI[1] = 1f;
					}
					else
					{
						calamityGlobalNPC.newAI[2] += 1f;

						// Slow down for a few frames
						float totalChargeTime = chargeDistance * 4f / chargeVelocity;
						float slowDownTime = chargeVelocity;
						if (calamityGlobalNPC.newAI[2] >= totalChargeTime - slowDownTime)
							npc.velocity *= 0.9f;

						// Reset and either go back to normal or charge again
						if (calamityGlobalNPC.newAI[2] >= totalChargeTime)
						{
							calamityGlobalNPC.newAI[1] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							npc.ai[1] += 1f;

							if (npc.ai[1] >= chargeAmt)
							{
								// Reset and return to normal movement
								calamityGlobalNPC.newAI[0] = 0f;
								npc.ai[1] = 0f;
							}
							else
							{
								// Get a new target and charge again
								npc.TargetClosest();
							}
						}
					}
				}
				else
				{
					// Pick a charging location
					// Set charge locations X
					if (vector.X >= player.Center.X)
						calamityGlobalNPC.newAI[1] = player.Center.X + chargeDistance;
					else
						calamityGlobalNPC.newAI[1] = player.Center.X - chargeDistance;

					// Set charge locations Y
					if (vector.Y >= player.Center.Y)
						calamityGlobalNPC.newAI[2] = player.Center.Y + chargeDistance;
					else
						calamityGlobalNPC.newAI[2] = player.Center.Y - chargeDistance;

					Vector2 chargeVector = new Vector2(calamityGlobalNPC.newAI[1], calamityGlobalNPC.newAI[2]);
					Vector2 chargeLocationVelocity = Vector2.Normalize(chargeVector - vector) * chargeVelocity;
					Vector2 cloneChargeVector = cloneAlive ? new Vector2(Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1], Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2]) : default;

					// If clone is alive and not at proper location then keep trying to line up until it gets into position
					float chargeDistanceGateValue = 40f;
					bool clonePositionCheck = cloneAlive ? Vector2.Distance(Main.npc[CalamityGlobalNPC.ghostBossClone].Center, cloneChargeVector) <= chargeDistanceGateValue : true;

					// Line up a charge
					if (Vector2.Distance(vector, chargeVector) <= chargeDistanceGateValue || reachedChargingPoint)
					{
						// Emit dust
						if (!reachedChargingPoint)
						{
							Main.PlaySound(SoundID.Item125, npc.position);
							for (int i = 0; i < 30; i++)
							{
								int dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
								Main.dust[dust].noGravity = true;
								Main.dust[dust].velocity *= 5f;
							}
						}

						reachedChargingPoint = true;
						npc.velocity = Vector2.Zero;
						npc.Center = chargeVector;

						if (clonePositionCheck)
						{
							// Initiate charge
							calamityGlobalNPC.newAI[1] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 1f;

							// Tell clone to charge
							if (cloneAlive)
							{
								Main.npc[CalamityGlobalNPC.ghostBossClone].ai[0] = 0f;
								Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[1] = 0f;
								Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[2] = 0f;
								Main.npc[CalamityGlobalNPC.ghostBossClone].Calamity().newAI[3] = 1f;

								//
								// CODE TWEAKED BY: OZZATRON
								// September 21st, 2020
								// reason: fixing Polter charge MP desync bug
								//
								// removed Polter syncing the clone's newAI array. The clone now auto syncs its own newAI every frame.
							}
						}
					}
					else
						npc.SimpleFlyMovement(chargeLocationVelocity, chargeAcceleration);
				}

				npc.netUpdate = true;

				if (npc.netSpam > 10)
					npc.netSpam = 10;

				if (Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}

			if (!phase2 && !phase3)
            {
                npc.damage = npc.defDamage;
                npc.defense = npc.defDefense;

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost)
                        npc.localAI[1] += 2f;

                    if (npc.localAI[1] >= 90f * projectileFireRateMultiplier)
                    {
                        npc.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
                            int type = ModContent.ProjectileType<PhantomShot>();
                            if (Main.rand.NextBool(3))
                            {
                                npc.localAI[1] = -30f;
                                type = ModContent.ProjectileType<PhantomBlast>();
                            }

							int damage = npc.GetProjectileDamage(type);

							Vector2 vector93 = vector;
							float num743 = player.Center.X - vector93.X;
							float num744 = player.Center.Y - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = baseProjectileVelocity / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							float rotation = MathHelper.ToRadians(baseProjectileSpread);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / baseProjectileAmt;
							double offsetAngle;
							for (int i = 0; i < baseProjectileAmt; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
							}
						}
                        else
                        {
							int type = ModContent.ProjectileType<PhantomBlast>();
							int damage = npc.GetProjectileDamage(type);

							Vector2 vector93 = vector;
							float num743 = player.Center.X - vector93.X;
							float num744 = player.Center.Y - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = (baseProjectileVelocity + 5f) / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							float rotation = MathHelper.ToRadians(baseProjectileSpread + 15);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / baseProjectileAmt;
							double offsetAngle;
							for (int i = 0; i < baseProjectileAmt; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
							}
						}
                    }
                }
            }
            else if (!phase3)
            {
                if (npc.ai[0] == 0f)
                {
                    npc.ai[0] = 1f;

					// Reset charge attack arrays to prevent problems
					npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;

					Main.PlaySound(SoundID.Item122, npc.Center);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                npc.GivenName = "Necroghast";

                npc.damage = (int)(npc.defDamage * 1.2f);
                npc.defense = (int)(npc.defDefense * 0.8f);

                if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                {
                    npc.localAI[1] += expertMode ? 1.5f : 1f;
                    if (speedBoost)
                        npc.localAI[1] += 2f;

                    if (npc.localAI[1] >= 150f * projectileFireRateMultiplier)
                    {
                        npc.localAI[1] = 0f;

                        bool flag47 = Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height);
                        if (npc.localAI[3] > 0f)
                        {
                            flag47 = true;
                            npc.localAI[3] = 0f;
                        }

                        if (flag47)
                        {
							int type = ModContent.ProjectileType<PhantomShot2>();
							if (Main.rand.NextBool(3))
							{
								npc.localAI[1] = -30f;
								type = ModContent.ProjectileType<PhantomBlast2>();
							}

							int damage = npc.GetProjectileDamage(type);

							Vector2 vector93 = vector;
							float num743 = player.Center.X - vector93.X;
							float num744 = player.Center.Y - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = (baseProjectileVelocity + 1f) / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = baseProjectileAmt + 1;
							float rotation = MathHelper.ToRadians(baseProjectileSpread + 15);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = type == ModContent.ProjectileType<PhantomBlast2>() ? baseProjectileTimeLeft / 4 : baseProjectileTimeLeft;
							}
						}
                        else
                        {
							int type = ModContent.ProjectileType<PhantomBlast2>();
							int damage = npc.GetProjectileDamage(type);

							Vector2 vector93 = vector;
							float num743 = player.Center.X - vector93.X;
							float num744 = player.Center.Y - vector93.Y;
							float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

							num745 = (baseProjectileVelocity + 5f) / num745;
							num743 *= num745;
							num744 *= num745;
							vector93.X += num743 * 3f;
							vector93.Y += num744 * 3f;

							int numProj = baseProjectileAmt + 1;
							float rotation = MathHelper.ToRadians(baseProjectileSpread + 35);
							float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
							double startAngle = Math.Atan2(num743, num744) - rotation / 2;
							double deltaAngle = rotation / numProj;
							double offsetAngle;
							for (int i = 0; i < numProj; i++)
							{
								offsetAngle = startAngle + deltaAngle * i;
								int proj = Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
								Main.projectile[proj].timeLeft = baseProjectileTimeLeft / 4;
							}
						}
                    }
                }
            }
            else
            {
                npc.HitSound = SoundID.NPCHit36;

                if (npc.ai[0] == 1f)
                {
                    npc.ai[0] = 2f;

					// Reset charge attack arrays to prevent problems
					npc.ai[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
					calamityGlobalNPC.newAI[1] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					calamityGlobalNPC.newAI[3] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PolterPhantom>());

						if (expertMode)
						{
							for (int I = 0; I < 3; I++)
							{
								int spawn = NPC.NewNPC((int)(vector.X + (Math.Sin(I * 120) * 500)), (int)(vector.Y + (Math.Cos(I * 120) * 500)), ModContent.NPCType<PhantomFuckYou>(), npc.whoAmI, 0, 0, 0, -1);
								NPC npc2 = Main.npc[spawn];
								npc2.ai[0] = I * 120;
							}
						}
                    }

                    Main.PlaySound(SoundID.Item122, npc.Center);

                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt2"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt3"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt4"), 1f);
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Polt5"), 1f);

                    for (int num621 = 0; num621 < 10; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num622].velocity *= 3f;
                        Main.dust[num622].noGravity = true;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 30; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                }

                npc.GivenName = "Necroplasm";

                npc.damage = (int)(npc.defDamage * 1.4f);
                npc.defense = (int)(npc.defDefense * 0.5f);

				npc.localAI[1] += 1f;
				if (npc.localAI[1] >= (getPissed ? 150f : 210f) * projectileFireRateMultiplier && Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
				{
					npc.localAI[1] = 0f;
					if (Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
					{
						Vector2 vector93 = vector;
						float num743 = player.Center.X - vector93.X;
						float num744 = player.Center.Y - vector93.Y;
						float num745 = (float)Math.Sqrt(num743 * num743 + num744 * num744);

						num745 = baseProjectileVelocity / num745;
						num743 *= num745;
						num744 *= num745;
						vector93.X += num743 * 3f;
						vector93.Y += num744 * 3f;

						int numProj = baseProjectileAmt + (getPissed ? 4 : 2);
						float rotation = MathHelper.ToRadians(baseProjectileSpread + (getPissed ? 60 : 45));
						float baseSpeed = (float)Math.Sqrt(num743 * num743 + num744 * num744);
						double startAngle = Math.Atan2(num743, num744) - rotation / 2;
						double deltaAngle = rotation / numProj;
						double offsetAngle;

						int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<PhantomShot2>() : ModContent.ProjectileType<PhantomShot>();
						int damage = npc.GetProjectileDamage(type);

						for (int i = 0; i < numProj; i++)
						{
							offsetAngle = startAngle + deltaAngle * i;
							Projectile.NewProjectile(vector93.X, vector93.Y, baseSpeed * (float)Math.Sin(offsetAngle), baseSpeed * (float)Math.Cos(offsetAngle), type, damage, 0f, Main.myPlayer, 0f, 0f);
						}
					}
				}

				if (phase4)
                {
                    npc.localAI[2] += 1f;
                    if (npc.localAI[2] >= (getPissed ? 300f : 420f))
                    {
                        npc.localAI[2] = 0f;

                        float num757 = 6f;
                        Vector2 vector94 = vector;
                        float num758 = player.Center.X - vector94.X;
                        float num760 = player.Center.Y - vector94.Y;
                        float num761 = (float)Math.Sqrt(num758 * num758 + num760 * num760);
                        num761 = num757 / num761;
                        num758 *= num761;
                        num760 *= num761;
						vector94.X += num758 * 3f;
						vector94.Y += num760 * 3f;

						if (NPC.CountNPCS(ModContent.NPCType<PhantomSpiritL>()) < 2 && Main.netMode != NetmodeID.MultiplayerClient && !charging && !chargePhase)
                        {
                            int num762 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<PhantomSpiritL>());
                            Main.npc[num762].velocity.X = num758;
                            Main.npc[num762].velocity.Y = num760;
                            Main.npc[num762].netUpdate = true;
                        }
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<PolterghastTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePolterghast>(), true, !CalamityWorld.downedPolterghast);

			CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Cyborg }, CalamityWorld.downedPolterghast);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<RuinousSoul>(), 7, 15);
                DropHelper.DropItem(npc, ModContent.ItemType<Phantoplasm>(), 30, 40);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<PolterghastMask>(), 7);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<TerrorBlade>(w),
                    DropHelper.WeightStack<BansheeHook>(w),
                    DropHelper.WeightStack<DaemonsFlame>(w),
                    DropHelper.WeightStack<FatesReveal>(w),
                    DropHelper.WeightStack<GhastlyVisage>(w),
                    DropHelper.WeightStack<EtherealSubjugator>(w),
                    DropHelper.WeightStack<GhoulishGouger>(w)
                );

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<Affliction>(), true);
			}

            // If Polterghast has not been killed, notify players about the Abyss minibosses now dropping items
            if (!CalamityWorld.downedPolterghast)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ReaperSearchRoar"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

                string key = "Mods.CalamityMod.GhostBossText";
                Color messageColor = Color.RoyalBlue;
                string sulfSeaBoostMessage = "Mods.CalamityMod.GhostBossText4";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

				if (Main.rand.NextBool(20) && DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
				{
					sulfSeaBoostMessage = "Mods.CalamityMod.AprilFools2"; // Goddamn boomer duke moments
				}

				CalamityUtils.DisplayLocalizedText(key, messageColor);
				CalamityUtils.DisplayLocalizedText(sulfSeaBoostMessage, sulfSeaBoostColor);
			}

            // Mark Polterghast as dead
            CalamityWorld.downedPolterghast = true;
			CalamityNetcode.SyncWorld();
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastGlow2");
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 7;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastGlow");

			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
			Color lightRed = new Color(255, 100, 100, 255);
			if (npc.Calamity().newAI[0] > 300f)
				color37 = Color.Lerp(color37, lightRed, MathHelper.Clamp((npc.Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

			Color color42 = Color.Lerp(Color.White, (npc.ai[2] >= 300f || npc.Calamity().newAI[0] > 300f) ? Color.Red : Color.Black, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			bool phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f);
			bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);

            npc.frameCounter += 1D;
            if (npc.frameCounter > 6D)
            {
                npc.frameCounter = 0D;
                npc.frame.Y += frameHeight;
            }
            if (phase3)
            {
				if (npc.frame.Y < frameHeight * 8)
				{
					npc.frame.Y = frameHeight * 8;
				}
				if (npc.frame.Y > frameHeight * 11)
				{
					npc.frame.Y = frameHeight * 8;
				}
            }
            else if (phase2)
            {
                if (npc.frame.Y < frameHeight * 4)
                {
                    npc.frame.Y = frameHeight * 4;
                }
                if (npc.frame.Y > frameHeight * 7)
                {
                    npc.frame.Y = frameHeight * 4;
                }
            }
            else
            {
				if (npc.frame.Y > frameHeight * 3)
				{
					npc.frame.Y = 0;
				}
			}
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(BuffID.MoonLeech, 900, true);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Ectoplasm, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Ectoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
