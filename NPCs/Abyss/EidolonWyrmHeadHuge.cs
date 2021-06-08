using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmHeadHuge : ModNPC
    {
		private enum Phase
		{
			ChargeOne = 0,
			LightningRain = 1,
			FastCharge = 2,
			EidolonWyrmSpawn = 3,
			ChargeTwo = 4,
			IceMist = 5,
			ShadowFireballSpin = 6,
			AncientDoomSummon = 7,
			LightningCharge = 8,
			EidolistSpawn = 9,
			FinalPhase = 10
		}

		private float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		Vector2 chargeDestination = default;
		private const int minLength = 40;
        private const int maxLength = 41;
        private bool TailSpawned = false;
		private int rotationDirection = 0;
		private float chargeVelocityScalar = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 50f;
			npc.GetNPCDamage();
			npc.width = 254;
            npc.height = 138;
            npc.defense = 100;
			npc.DR_NERD(0.4f);
			CalamityGlobalNPC global = npc.Calamity();
			global.multDRReductions.Add(BuffID.CursedInferno, 0.9f);
			npc.LifeMaxNERB(1000000, 1150000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.netAlways = true;
			npc.boss = true;
			music = CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ?? MusicID.Boss3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
			writer.WriteVector2(chargeDestination);
			writer.Write(rotationDirection);
			writer.Write(chargeVelocityScalar);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
			chargeDestination = reader.ReadVector2();
			rotationDirection = reader.ReadInt32();
			chargeVelocityScalar = reader.ReadSingle();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Difficulty modes
			bool malice = CalamityWorld.malice;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			// Phases
			bool phase2 = lifeRatio < 0.8f;
			bool phase3 = lifeRatio < 0.6f;
			bool phase4 = lifeRatio < 0.4f;
			bool phase5 = lifeRatio < 0.2f;
			bool phase6 = lifeRatio < 0.05f;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Play spawn sound
			if (!TailSpawned && npc.ai[0] == 0f)
			{
				if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, npc.Center) < 2800f)
				{
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Scare"),
						(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
				}
			}

			// Spawn segments
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol;
						if (num36 >= 0 && num36 < minLength)
						{
							if (num36 % 2 == 0)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyHuge>(), npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyAltHuge>(), npc.whoAmI);
						}
						else
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmTailHuge>(), npc.whoAmI);

						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = (float)npc.whoAmI;
						Main.npc[lol].ai[1] = (float)Previous;
						Main.npc[Previous].ai[0] = (float)lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					TailSpawned = true;
				}
			}

			// Despawn if target is dead
            if (player.dead)
            {
                npc.TargetClosest(false);

				npc.velocity.Y += 2f;
				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 2f;

				if (npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int a = 0; a < Main.maxNPCs; a++)
					{
						if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyAltHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmTailHuge>())
							Main.npc[a].active = false;
					}
				}
			}

			// Delete self if far from target or tail doesn't exist
			if (Vector2.Distance(player.Center, npc.Center) > 6400f || !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmTailHuge>()))
				npc.active = false;

			// Adjust opacity
			bool invisiblePhase = calamityGlobalNPC.newAI[0] == (float)Phase.LightningRain || calamityGlobalNPC.newAI[0] == (float)Phase.IceMist || calamityGlobalNPC.newAI[0] == (float)Phase.AncientDoomSummon;
			npc.dontTakeDamage = invisiblePhase;
			if (!invisiblePhase)
			{
				npc.Opacity += 0.15f;
				if (npc.Opacity > 1f)
					npc.Opacity = 1f;
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Rotation and direction
			npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			int direction = npc.direction;
			npc.direction = npc.spriteDirection = (npc.velocity.X > 0f) ? 1 : (-1);
			if (direction != npc.direction)
				npc.netUpdate = true;

			// General AI pattern
			// Charge
			// Charge : Phase 2 - Spin around target and summon Shadow Fireballs
			// Charge : Phase 4 - Swim to the right and dash towards the target, summon Lightning Bolts from above during it
			// Turn invisible, swim above the target and summon predictive Lightning Bolts
			// Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
			// Spawn an Eidolon Wyrm and swim below the target for 10 seconds, or less, if the Wyrm dies
			// Charge
			// Charge : Phase 3 - Turn invisible and summon Ancient Dooms around the target
			// Charge : Phase 4 - Turn visible, swim to the left and dash towards the target, summon Lightning Bolts from above during it
			// Turn invisible, swim beneath the target and summon Ice Mist
			// Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
			// Spawn Eidolists and swim below the target for 10 seconds, or less, if the Eidolists die

			// Final phase
			// Spin around the target and summon Ancient Dooms and Shadow Fireballs

			// Attack patterns
			// Phase 1 - 0, 0, 0, 1, 2, 3, 4, 4, 4, 5, 2, 9
			// Phase 2 - 0, 6, 0, 1, 2, 3, 4, 4, 4, 5, 2, 9
			// Phase 3 - 0, 6, 0, 1, 2, 3, 4, 7, 4, 5, 2, 9
			// Phase 4 - 0, 6, 8, 1, 2, 3, 4, 7, 8, 5, 2, 9
			// Phase 5 - 0, 6, 8, 1, 2, 2, 4, 7, 8, 5, 2, 2
			// Phase 6 - 10

			// Default vector to swim to
			Vector2 destination = player.Center;

			// Charge variables
			Vector2 chargeVector = default;
			float chargeDistance = 1000f;
			float turnDistance = 640f;
			float chargeLocationDistance = turnDistance * 0.2f;
			switch ((int)calamityGlobalNPC.newAI[1])
			{
				case 0:
					chargeVector.X -= chargeDistance;
					break;
				case 1:
					chargeVector.X += chargeDistance;
					break;
				case 2:
					chargeVector.Y -= chargeDistance;
					break;
				case 3:
					chargeVector.Y += chargeDistance;
					break;
				case 4:
					chargeVector.X -= chargeDistance;
					chargeVector.Y -= chargeDistance;
					break;
				case 5:
					chargeVector.X += chargeDistance;
					chargeVector.Y += chargeDistance;
					break;
				case 6:
					chargeVector.X -= chargeDistance;
					chargeVector.Y += chargeDistance;
					break;
				case 7:
					chargeVector.X += chargeDistance;
					chargeVector.Y -= chargeDistance;
					break;
			}
			Vector2 chargeLocation = destination + chargeVector;
			Vector2 chargeVectorFlipped = chargeVector * -1f;

			// Lightning Rain variables
			Vector2 lightningRainLocation = new Vector2(0f, -1200f);
			float lightningRainLocationDistance = turnDistance * 0.2f;

			// Wyrm and Eidolist variables
			Vector2 eidolonWyrmPhaseLocation = new Vector2(0f, 1200f);
			float eidolonWyrmPhaseLocationDistance = turnDistance * 0.5f;
			int maxEidolists = 10;

			// Ice Mist variables
			Vector2 iceMistLocation = new Vector2(0f, 1200f);
			float iceMistLocationDistance = turnDistance * 0.2f;

			// Spin variables
			float spinRadius = 800f;
			Vector2 spinLocation = new Vector2(0f, -spinRadius);
			float spinLocationDistance = turnDistance * 0.1f;
			float degreesOfRotation = 2f;
			int totalRotations = 3;

			// Ancient Doom variables
			Vector2 ancientDoomLocation = new Vector2(0f, -1200f);
			float ancientDoomLocationDistance = turnDistance * 0.2f;
			int ancientDoomLimit = 6;
			int ancientDoomDegrees = 360 / ancientDoomLimit;
			int ancientDoomDistance = 640;
			float maxAncientDoomRings = 3f;

			// Lightning charge variables
			Vector2 lightningChargeVector = npc.localAI[2] == 0f ? new Vector2(1000f, 0f) : new Vector2(-1000f, 0f);
			float lightningChargeLocationDistance = turnDistance * 0.2f;
			Vector2 lightningChargeLocation = destination + lightningChargeVector;
			Vector2 lightningChargeVectorFlipped = lightningChargeVector * -1f;
			float lightningSpawnY = 540f;
			Vector2 lightningSpawnLocation = new Vector2(lightningChargeVector.X, -lightningSpawnY);
			int numLightningBolts = 10;
			float distanceBetweenBolts = lightningSpawnY * 2f / numLightningBolts;

			// Velocity and turn speed values
			float baseVelocity = !player.wet ? player.velocity.Length() + 8f : player.velocity.Length() + 4f;
			float turnSpeed = !player.wet ? MathHelper.ToRadians(4f) : MathHelper.ToRadians(2f);
			float normalChargeVelocityMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
			float normalChargeTurnSpeedMult = 1f - MathHelper.Lerp(0f, 0.5f, chargeVelocityScalar);
			float invisiblePhaseVelocityMult = MathHelper.Lerp(1f, 1.5f, chargeVelocityScalar);
			float fastChargeVelocityMult = MathHelper.Lerp(1f, 3f, chargeVelocityScalar);
			float fastChargeTurnSpeedMult = 1f - MathHelper.Lerp(0f, 0.5f, chargeVelocityScalar);
			float chargeVelocityScalarIncrement = 0.05f;

			// Phase gate values
			float chargePhaseGateValue = 180f;
			float chargeGateValue = chargePhaseGateValue + 10f;
			float lightningRainDuration = 180f;
			float lightningRainPhaseDuration = lightningRainDuration + 30f;
			float eidolonWyrmPhaseDuration = 600f;
			float iceMistDuration = 180f;
			float iceMistPhaseDuration = iceMistDuration + 30f;
			float spinPhaseDuration = 360 * totalRotations / degreesOfRotation;
			float ancientDoomPhaseGateValue = 30f;
			float ancientDoomGateValue = 120f;
			float lightningChargePhaseGateValue = 90f;
			float lightningChargeGateValue = lightningChargePhaseGateValue + 10f;

			// Phase switch
			switch ((int)AIState)
			{
				// First charge combo
				case (int)Phase.ChargeOne:

					if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
					{
						turnDistance = chargeLocationDistance;
						if ((chargeLocation - npc.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
						{
							if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue)
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);

							if (calamityGlobalNPC.newAI[2] < chargeGateValue)
							{
								// Turn for 10 frames before the charge
								calamityGlobalNPC.newAI[2] += 1f;
								if (calamityGlobalNPC.newAI[2] == chargeGateValue)
									chargeDestination = destination + chargeVectorFlipped;
							}
							else
							{
								// Charge

								// Use a lerp to smoothly scale up velocity and turn speed
								chargeVelocityScalar += chargeVelocityScalarIncrement;
								if (chargeVelocityScalar > 1f)
									chargeVelocityScalar = 1f;

								baseVelocity *= normalChargeVelocityMult;
								turnSpeed *= normalChargeTurnSpeedMult;

								destination = chargeDestination;

								if ((destination - npc.Center).Length() < chargeLocationDistance)
								{
									npc.ai[3] += 1f;
									float maxCharges = phase4 ? 1 : phase2 ? 2 : 3;
									if (npc.ai[3] >= maxCharges)
									{
										npc.ai[3] = 0f;
										calamityGlobalNPC.newAI[0] = phase4 ? (float)Phase.ShadowFireballSpin : (float)Phase.LightningRain;
									}
									else if (phase2)
										calamityGlobalNPC.newAI[0] = (float)Phase.ShadowFireballSpin;

									calamityGlobalNPC.newAI[1] += 1f;
									if (calamityGlobalNPC.newAI[1] > 7f)
										calamityGlobalNPC.newAI[1] = 0f;

									calamityGlobalNPC.newAI[2] = 0f;

									chargeVelocityScalar = 0f;

									if (phase6)
									{
										npc.localAI[1] = 0f;
										calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
										calamityGlobalNPC.newAI[1] = 0f;
										calamityGlobalNPC.newAI[2] = 0f;
										rotationDirection = 0;
									}

									npc.TargetClosest();
								}
							}
						}
						else
							destination += chargeLocation;
					}
					else
						calamityGlobalNPC.newAI[2] += 1f;

					break;

				// Turn invisible, swim above and summon predictive lightning bolts
				case (int)Phase.LightningRain:

					// Swim up
					destination += lightningRainLocation;
					turnDistance = lightningRainLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					chargeVelocityScalar += chargeVelocityScalarIncrement;
					if (chargeVelocityScalar > 1f)
						chargeVelocityScalar = 1f;

					baseVelocity *= invisiblePhaseVelocityMult;
					turnSpeed *= invisiblePhaseVelocityMult;

					if ((destination - npc.Center).Length() < lightningRainLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						if (calamityGlobalNPC.newAI[2] == 0f)
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), npc.Center);

						calamityGlobalNPC.newAI[2] += 1f;

						if (calamityGlobalNPC.newAI[2] % 30f == 0f && calamityGlobalNPC.newAI[2] < lightningRainDuration && Main.netMode != NetmodeID.MultiplayerClient)
						{
							int maxTargets = 2;
							int[] whoAmIArray = new int[maxTargets];
							Vector2[] targetCenterArray = new Vector2[maxTargets];
							int numProjectiles = 0;
							float maxDistance = 2400f;

							for (int i = 0; i < Main.maxPlayers; i++)
							{
								if (!Main.player[i].active || Main.player[i].dead)
									continue;

								Vector2 playerCenter = Main.player[i].Center;
								float distance = Vector2.Distance(playerCenter, npc.Center);
								if (distance < maxDistance && Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
								{
									whoAmIArray[numProjectiles] = i;
									targetCenterArray[numProjectiles] = playerCenter;
									int num34 = numProjectiles + 1;
									numProjectiles = num34;
									if (num34 >= targetCenterArray.Length)
										break;
								}
							}

							float predictionAmt = 40f;
							float lightningVelocity = 8f;
							for (int i = 0; i < numProjectiles; i++)
							{
								// Predictive bolt
								Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt - npc.Center;
								float ai = Main.rand.Next(100);
								Vector2 projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
								int type = ProjectileID.CultistBossLightningOrbArc;
								int damage = npc.GetProjectileDamage(type);
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);

								// Opposite bolt
								projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt - npc.Center;
								ai = Main.rand.Next(100);
								projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);

								// Normal bolt
								projectileDestination = targetCenterArray[i] - npc.Center;
								ai = Main.rand.Next(100);
								projectileVelocity = Vector2.Normalize(projectileDestination.RotatedByRandom(MathHelper.PiOver4)) * lightningVelocity;
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
							}
						}

						Lighting.AddLight(npc.Center, 0.4f, 0.85f, 0.9f);

						float num737 = (float)Main.rand.NextDouble() * 1f - 0.5f;
						if (num737 < -0.5f)
							num737 = -0.5f;
						if (num737 > 0.5f)
							num737 = 0.5f;

						Vector2 value40 = new Vector2(-npc.width * 0.2f * npc.scale, 0f).RotatedBy(num737 * ((float)Math.PI * 2f)).RotatedBy(npc.velocity.ToRotation());
						int num738 = Dust.NewDust(npc.Center - Vector2.One * 5f, 10, 10, 226, (0f - npc.velocity.X) / 3f, (0f - npc.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
						Main.dust[num738].position = npc.Center + value40;
						Main.dust[num738].velocity = Vector2.Normalize(Main.dust[num738].position - npc.Center) * 2f;
						Main.dust[num738].noGravity = true;

						float num740 = (float)Main.rand.NextDouble() * 1f - 0.5f;
						if (num740 < -0.5f)
							num740 = -0.5f;
						if (num740 > 0.5f)
							num740 = 0.5f;

						Vector2 value41 = new Vector2(-npc.width * 0.6f * npc.scale, 0f).RotatedBy(num740 * ((float)Math.PI * 2f)).RotatedBy(npc.velocity.ToRotation());
						int num741 = Dust.NewDust(npc.Center - Vector2.One * 5f, 10, 10, 226, (0f - npc.velocity.X) / 3f, (0f - npc.velocity.Y) / 3f, 150, Color.Transparent, 0.7f);
						Main.dust[num741].velocity = Vector2.Zero;
						Main.dust[num741].position = npc.Center + value41;
						Main.dust[num741].noGravity = true;

						if (calamityGlobalNPC.newAI[2] >= lightningRainPhaseDuration)
						{
							npc.localAI[0] = 0f;
							calamityGlobalNPC.newAI[0] = (float)Phase.FastCharge;
							calamityGlobalNPC.newAI[2] = 90f;

							chargeVelocityScalar = 0f;

							if (phase6)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
								calamityGlobalNPC.newAI[1] = 0f;
								calamityGlobalNPC.newAI[2] = 0f;
								rotationDirection = 0;
							}

							npc.TargetClosest();
						}
					}

					break;

				// Turn visible and charge quickly
				case (int)Phase.FastCharge:

					if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
					{
						turnDistance = chargeLocationDistance;
						if ((chargeLocation - npc.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
						{
							if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue)
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);

							if (calamityGlobalNPC.newAI[2] < chargeGateValue)
							{
								// Slow down and quickly turn for 10 frames before the charge
								calamityGlobalNPC.newAI[2] += 1f;
								if (calamityGlobalNPC.newAI[2] == chargeGateValue)
									chargeDestination = destination + chargeVectorFlipped;
							}
							else
							{
								// Charge very quickly

								// Use a lerp to smoothly scale up velocity and turn speed
								chargeVelocityScalar += chargeVelocityScalarIncrement;
								if (chargeVelocityScalar > 1f)
									chargeVelocityScalar = 1f;

								baseVelocity *= fastChargeVelocityMult;
								turnSpeed *= fastChargeTurnSpeedMult;
								destination = chargeDestination;

								if ((destination - npc.Center).Length() < chargeLocationDistance)
								{
									if (!phase5)
									{
										calamityGlobalNPC.newAI[0] = npc.localAI[0] == 0f ? (float)Phase.EidolonWyrmSpawn : (float)Phase.EidolistSpawn;
										calamityGlobalNPC.newAI[2] = 0f;
									}
									else
									{
										npc.ai[3] += 1f;
										if (npc.ai[3] >= 2f)
										{
											npc.ai[3] = 0f;
											calamityGlobalNPC.newAI[0] = npc.localAI[0] == 0f ? (float)Phase.ChargeTwo : (float)Phase.ChargeOne;
											calamityGlobalNPC.newAI[2] = 0f;
										}
										else
											calamityGlobalNPC.newAI[2] = 90f;
									}

									calamityGlobalNPC.newAI[1] += 1f;
									if (calamityGlobalNPC.newAI[1] > 7f)
										calamityGlobalNPC.newAI[1] = 0f;

									chargeVelocityScalar = 0f;

									if (phase6)
									{
										npc.localAI[1] = 0f;
										calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
										calamityGlobalNPC.newAI[1] = 0f;
										calamityGlobalNPC.newAI[2] = 0f;
										rotationDirection = 0;
									}

									npc.TargetClosest();
								}
							}
						}
						else
							destination += chargeLocation;
					}
					else
						calamityGlobalNPC.newAI[2] += 1f;

					break;

				// Summon an Eidolon Wyrm and swim below the target for 10 seconds, or less, if the Wyrm dies
				case (int)Phase.EidolonWyrmSpawn:

					destination += eidolonWyrmPhaseLocation;
					turnDistance = eidolonWyrmPhaseLocationDistance;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						if (calamityGlobalNPC.newAI[2] == 0f)
							NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<EidolonWyrmHead>());
					}

					calamityGlobalNPC.newAI[2] += 1f;

					if (calamityGlobalNPC.newAI[2] >= eidolonWyrmPhaseDuration || !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()))
					{
						calamityGlobalNPC.newAI[0] = (float)Phase.ChargeTwo;
						calamityGlobalNPC.newAI[2] = 0f;

						if (phase6)
						{
							npc.localAI[1] = 0f;
							calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
							calamityGlobalNPC.newAI[1] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							rotationDirection = 0;
						}

						npc.TargetClosest();
					}

					break;

				// Second charge combo
				case (int)Phase.ChargeTwo:

					if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
					{
						turnDistance = chargeLocationDistance;
						if ((chargeLocation - npc.Center).Length() < chargeLocationDistance || calamityGlobalNPC.newAI[2] > chargePhaseGateValue)
						{
							if (calamityGlobalNPC.newAI[2] == chargePhaseGateValue)
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);

							if (calamityGlobalNPC.newAI[2] < chargeGateValue)
							{
								// Slow down and quickly turn for 10 frames before the charge
								calamityGlobalNPC.newAI[2] += 1f;
								if (calamityGlobalNPC.newAI[2] == chargeGateValue)
									chargeDestination = destination + chargeVectorFlipped;
							}
							else
							{
								// Charge

								// Use a lerp to smoothly scale up velocity and turn speed
								chargeVelocityScalar += chargeVelocityScalarIncrement;
								if (chargeVelocityScalar > 1f)
									chargeVelocityScalar = 1f;

								baseVelocity *= normalChargeVelocityMult;
								turnSpeed *= normalChargeTurnSpeedMult;

								destination = chargeDestination;

								if ((destination - npc.Center).Length() < chargeLocationDistance)
								{
									npc.ai[3] += 1f;
									float maxCharges = phase4 ? 1 : phase3 ? 2 : 3;
									if (npc.ai[3] >= maxCharges)
									{
										npc.ai[3] = 0f;
										calamityGlobalNPC.newAI[0] = phase4 ? (float)Phase.AncientDoomSummon : (float)Phase.IceMist;
									}
									else if (phase3)
										calamityGlobalNPC.newAI[0] = (float)Phase.AncientDoomSummon;

									calamityGlobalNPC.newAI[1] += 1f;
									if (calamityGlobalNPC.newAI[1] > 7f)
										calamityGlobalNPC.newAI[1] = 0f;

									calamityGlobalNPC.newAI[2] = 0f;

									chargeVelocityScalar = 0f;

									if (phase6)
									{
										npc.localAI[1] = 0f;
										calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
										calamityGlobalNPC.newAI[1] = 0f;
										calamityGlobalNPC.newAI[2] = 0f;
										rotationDirection = 0;
									}

									npc.TargetClosest();
								}
							}
						}
						else
							destination += chargeLocation;
					}
					else
						calamityGlobalNPC.newAI[2] += 1f;

					break;

				// Turn invisible, swim beneath the target and summon ice mist
				case (int)Phase.IceMist:

					// Swim down
					destination += iceMistLocation;
					turnDistance = iceMistLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					chargeVelocityScalar += chargeVelocityScalarIncrement;
					if (chargeVelocityScalar > 1f)
						chargeVelocityScalar = 1f;

					baseVelocity *= invisiblePhaseVelocityMult;
					turnSpeed *= invisiblePhaseVelocityMult;

					if ((destination - npc.Center).Length() < iceMistLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						calamityGlobalNPC.newAI[2] += 1f;

						if (calamityGlobalNPC.newAI[2] % 30f == 0f && calamityGlobalNPC.newAI[2] < iceMistDuration && Main.netMode != NetmodeID.MultiplayerClient)
						{
							int maxTargets = 2;
							int[] whoAmIArray = new int[maxTargets];
							Vector2[] targetCenterArray = new Vector2[maxTargets];
							int numProjectiles = 0;
							float maxDistance = 2400f;

							for (int i = 0; i < Main.maxPlayers; i++)
							{
								if (!Main.player[i].active || Main.player[i].dead)
									continue;

								Vector2 playerCenter = Main.player[i].Center;
								float distance = Vector2.Distance(playerCenter, npc.Center);
								if (distance < maxDistance && Collision.CanHit(npc.Center, 1, 1, playerCenter, 1, 1))
								{
									whoAmIArray[numProjectiles] = i;
									targetCenterArray[numProjectiles] = playerCenter;
									int num34 = numProjectiles + 1;
									numProjectiles = num34;
									if (num34 >= targetCenterArray.Length)
										break;
								}
							}

							float predictionAmt = 60f;
							float iceMistVelocity = 16f;
							for (int i = 0; i < numProjectiles; i++)
							{
								// Predictive mist
								Vector2 projectileDestination = targetCenterArray[i] + Main.player[whoAmIArray[i]].velocity * predictionAmt - npc.Center;
								Vector2 projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
								int type = ProjectileID.CultistBossIceMist;
								int damage = npc.GetProjectileDamage(type);
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);

								// Opposite mist
								projectileDestination = targetCenterArray[i] - Main.player[whoAmIArray[i]].velocity * predictionAmt - npc.Center;
								projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);

								// Normal mist
								projectileDestination = targetCenterArray[i] - npc.Center;
								projectileVelocity = Vector2.Normalize(projectileDestination) * iceMistVelocity;
								Projectile.NewProjectile(npc.Center, projectileVelocity, type, damage, 0f, Main.myPlayer, 0f, 1f);
							}
						}

						Lighting.AddLight(npc.Center, 0.3f, 0.75f, 0.9f);

						float num737 = (float)Main.rand.NextDouble() * 1f - 0.5f;
						if (num737 < -0.5f)
							num737 = -0.5f;
						if (num737 > 0.5f)
							num737 = 0.5f;

						Vector2 value40 = new Vector2(-npc.width * 0.2f * npc.scale, 0f).RotatedBy(num737 * ((float)Math.PI * 2f)).RotatedBy(npc.velocity.ToRotation());
						int num738 = Dust.NewDust(npc.Center - Vector2.One * 5f, 10, 10, 197, 0f, 0f, 100, Color.Transparent);
						Main.dust[num738].position = npc.Center + value40;
						Main.dust[num738].velocity = Vector2.Normalize(Main.dust[num738].position - npc.Center) * 2f;
						Main.dust[num738].noGravity = true;

						float num740 = (float)Main.rand.NextDouble() * 1f - 0.5f;
						if (num740 < -0.5f)
							num740 = -0.5f;
						if (num740 > 0.5f)
							num740 = 0.5f;

						Vector2 value41 = new Vector2(-npc.width * 0.6f * npc.scale, 0f).RotatedBy(num740 * ((float)Math.PI * 2f)).RotatedBy(npc.velocity.ToRotation());
						int num741 = Dust.NewDust(npc.Center - Vector2.One * 5f, 10, 10, 197, 0f, 0f, 100, Color.Transparent);
						Main.dust[num741].velocity = Vector2.Zero;
						Main.dust[num741].position = npc.Center + value41;
						Main.dust[num741].noGravity = true;

						if (calamityGlobalNPC.newAI[2] >= iceMistPhaseDuration)
						{
							npc.localAI[0] = 1f;
							calamityGlobalNPC.newAI[0] = (float)Phase.FastCharge;
							calamityGlobalNPC.newAI[2] = 90f;

							chargeVelocityScalar = 0f;

							if (phase6)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
								calamityGlobalNPC.newAI[1] = 0f;
								calamityGlobalNPC.newAI[2] = 0f;
								rotationDirection = 0;
							}

							npc.TargetClosest();
						}
					}

					break;

				// Phase 2 attack - Get in position for spin, spin around target and summon shadow fireballs
				case (int)Phase.ShadowFireballSpin:

					// Swim up
					destination += spinLocation;
					turnDistance = spinLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					chargeVelocityScalar += chargeVelocityScalarIncrement;
					if (chargeVelocityScalar > 1f)
						chargeVelocityScalar = 1f;

					baseVelocity *= invisiblePhaseVelocityMult;
					turnSpeed *= invisiblePhaseVelocityMult;

					// Spin around target
					if ((destination - npc.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						if (rotationDirection == 0)
							rotationDirection = player.direction;

						calamityGlobalNPC.newAI[2] += 1f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
							npc.Center = player.Center + new Vector2(spinRadius, 0).RotatedBy(npc.localAI[1]);

						npc.localAI[1] += MathHelper.ToRadians(degreesOfRotation) * rotationDirection;

						if (calamityGlobalNPC.newAI[2] >= spinPhaseDuration)
						{
							rotationDirection = 0;
							npc.localAI[1] = 0f;
							calamityGlobalNPC.newAI[0] = phase4 ? (float)Phase.LightningCharge : (float)Phase.ChargeOne;
							calamityGlobalNPC.newAI[2] = 0f;

							chargeVelocityScalar = 0f;

							if (phase6)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
								calamityGlobalNPC.newAI[1] = 0f;
								calamityGlobalNPC.newAI[2] = 0f;
								rotationDirection = 0;
							}

							npc.TargetClosest();
						}

						// Return to prevent other velocity code from being called
						return;
					}

					break;

				// Phase 3 attack - Swim above, turn invisible and summon ancient dooms around the target
				case (int)Phase.AncientDoomSummon:

					// Swim up
					destination += ancientDoomLocation;
					turnDistance = ancientDoomLocationDistance;

					if (npc.localAI[1] < maxAncientDoomRings)
					{
						calamityGlobalNPC.newAI[2] += 1f;
						if (calamityGlobalNPC.newAI[2] >= ancientDoomPhaseGateValue)
						{
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
								if (aiGateValue % ancientDoomGateValue == 0f)
								{
									// Spawn 3 (or more) circles of Ancient Dooms around the target
									int ancientDoomScale = (int)(aiGateValue / ancientDoomGateValue);
									ancientDoomLimit = 4 + ancientDoomScale;
									ancientDoomDegrees = 360 / ancientDoomLimit;
									ancientDoomDistance = 550 + ancientDoomScale * 45;
									for (int i = 0; i < ancientDoomLimit; i++)
									{
										float ai2 = i * ancientDoomDegrees;
										NPC.NewNPC((int)(player.Center.X + (float)(Math.Sin(i * ancientDoomDegrees) * ancientDoomDistance)), (int)(player.Center.Y + (float)(Math.Cos(i * ancientDoomDegrees) * ancientDoomDistance)),
											NPCID.AncientDoom, 0, npc.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
									}
									npc.localAI[1] += 1f;
									npc.TargetClosest();
								}
							}
						}
					}

					if (npc.localAI[1] >= maxAncientDoomRings)
					{
						npc.localAI[1] += 1f;
						if (npc.localAI[1] >= ancientDoomGateValue + maxAncientDoomRings)
						{
							npc.localAI[1] = 0f;
							calamityGlobalNPC.newAI[0] = phase4 ? (float)Phase.LightningCharge : (float)Phase.ChargeTwo;
							calamityGlobalNPC.newAI[2] = 0f;

							if (phase6)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
								calamityGlobalNPC.newAI[1] = 0f;
								calamityGlobalNPC.newAI[2] = 0f;
								rotationDirection = 0;
							}

							npc.TargetClosest();
						}
					}

					break;

				// Phase 4 attack - Swim to the right or left and charge towards the target, summon a wall of lightning bolts in the direction of the charge
				case (int)Phase.LightningCharge:

					if (calamityGlobalNPC.newAI[2] >= lightningChargePhaseGateValue)
					{
						turnDistance = lightningChargeLocationDistance;
						if ((lightningChargeLocation - npc.Center).Length() < lightningChargeLocationDistance || calamityGlobalNPC.newAI[2] > lightningChargePhaseGateValue)
						{
							if (calamityGlobalNPC.newAI[2] == lightningChargePhaseGateValue)
								Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);

							if (calamityGlobalNPC.newAI[2] < lightningChargeGateValue)
							{
								// Slow down and quickly turn for 10 frames before the charge
								calamityGlobalNPC.newAI[2] += 1f;
								if (calamityGlobalNPC.newAI[2] == lightningChargeGateValue)
									chargeDestination = destination + lightningChargeVectorFlipped;
							}
							else
							{
								// Charge

								// Use a lerp to smoothly scale up velocity and turn speed
								chargeVelocityScalar += chargeVelocityScalarIncrement;
								if (chargeVelocityScalar > 1f)
									chargeVelocityScalar = 1f;

								baseVelocity *= fastChargeVelocityMult;
								turnSpeed *= fastChargeTurnSpeedMult;

								destination = chargeDestination;

								// Lightning barrage
								if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[3] == 0f)
								{
									npc.localAI[3] = 1f;
									int type = ProjectileID.CultistBossLightningOrbArc;
									int damage = npc.GetProjectileDamage(type);
									for (int i = 0; i < numLightningBolts; i++)
									{
										lightningSpawnLocation.Y += distanceBetweenBolts * i;
										Vector2 projectileDestination = player.Center - lightningSpawnLocation;
										float ai = Main.rand.Next(100);
										Projectile.NewProjectile(lightningSpawnLocation, npc.velocity, type, damage, 0f, Main.myPlayer, projectileDestination.ToRotation(), ai);
									}
								}

								if ((destination - npc.Center).Length() < lightningChargeLocationDistance)
								{
									calamityGlobalNPC.newAI[0] = npc.localAI[2] == 0f ? (float)Phase.LightningRain : (float)Phase.IceMist;
									calamityGlobalNPC.newAI[2] = 0f;

									npc.localAI[2] += 1f;
									if (npc.localAI[2] > 1f)
										npc.localAI[2] = 0f;

									chargeVelocityScalar = 0f;

									if (phase6)
									{
										npc.localAI[1] = 0f;
										calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
										calamityGlobalNPC.newAI[1] = 0f;
										calamityGlobalNPC.newAI[2] = 0f;
										rotationDirection = 0;
									}

									npc.TargetClosest();
								}
							}
						}
						else
							destination += lightningChargeLocation;
					}
					else
						calamityGlobalNPC.newAI[2] += 1f;

					break;

				// Summon Eidolists and swim below the target for 10 seconds, or less, if the Eidolists die
				case (int)Phase.EidolistSpawn:

					destination += eidolonWyrmPhaseLocation;
					turnDistance = eidolonWyrmPhaseLocationDistance;

					if (calamityGlobalNPC.newAI[2] == 0f)
					{
						// Spawn Eidolists randomly around the target
						for (int i = 0; i < maxEidolists; i++)
						{
							Point npcCenter = npc.Center.ToTileCoordinates();
							Point playerCenter = player.Center.ToTileCoordinates();
							Vector2 distance = player.Center - npc.Center;

							int baseDistance = 20;
							int minDistance = 3;
							int maxDistance = 7;
							int collisionRange = 2;
							int iterations = 0;
							bool tooFar = distance.Length() > 2800f;
							while (!tooFar && iterations < 100)
							{
								iterations++;
								int randomX = Main.rand.Next(playerCenter.X - baseDistance, playerCenter.X + baseDistance + 1);
								int randomY = Main.rand.Next(playerCenter.Y - baseDistance, playerCenter.Y + baseDistance + 1);
								if ((randomY < playerCenter.Y - maxDistance || randomY > playerCenter.Y + maxDistance || randomX < playerCenter.X - maxDistance || randomX > playerCenter.X + maxDistance) && (randomY < npcCenter.Y - minDistance || randomY > npcCenter.Y + minDistance || randomX < npcCenter.X - minDistance || randomX > npcCenter.X + minDistance) && !Main.tile[randomX, randomY].nactive())
								{
									bool canSpawn = true;
									if (canSpawn && Collision.SolidTiles(randomX - collisionRange, randomX + collisionRange, randomY - collisionRange, randomY + collisionRange))
										canSpawn = false;

									if (canSpawn)
									{
										NPC.NewNPC(randomX * 16 + 8, randomY * 16 + 8, ModContent.NPCType<Eidolist>(), 0, npc.whoAmI);
										break;
									}
								}
							}
						}
					}

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= eidolonWyrmPhaseDuration || !NPC.AnyNPCs(ModContent.NPCType<Eidolist>()))
					{
						calamityGlobalNPC.newAI[0] = (float)Phase.ChargeOne;
						calamityGlobalNPC.newAI[2] = 0f;

						if (phase6)
						{
							npc.localAI[1] = 0f;
							calamityGlobalNPC.newAI[0] = (float)Phase.FinalPhase;
							calamityGlobalNPC.newAI[1] = 0f;
							calamityGlobalNPC.newAI[2] = 0f;
							rotationDirection = 0;
						}

						npc.TargetClosest();
					}

					break;

				// Spin around target, summon Ancient Dooms and shoot Shadow Fireballs from body segments
				case (int)Phase.FinalPhase:

					// Ancient Dooms
					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= ancientDoomPhaseGateValue)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							float aiGateValue = calamityGlobalNPC.newAI[2] - ancientDoomPhaseGateValue;
							if (aiGateValue % ancientDoomGateValue == 0f)
							{
								// Spawn 3 (or more) circles of Ancient Dooms around the target
								int ancientDoomScale = (int)(aiGateValue / ancientDoomGateValue);
								ancientDoomLimit = 4 + ancientDoomScale;
								ancientDoomDegrees = 360 / ancientDoomLimit;
								ancientDoomDistance = 550 + ancientDoomScale * 45;
								for (int i = 0; i < ancientDoomLimit; i++)
								{
									float ai2 = i * ancientDoomDegrees;
									NPC.NewNPC((int)(player.Center.X + (float)(Math.Sin(i * ancientDoomDegrees) * ancientDoomDistance)), (int)(player.Center.Y + (float)(Math.Cos(i * ancientDoomDegrees) * ancientDoomDistance)),
										NPCID.AncientDoom, 0, npc.whoAmI, 0f, ai2, 0f, Main.maxPlayers);
								}

								if (calamityGlobalNPC.newAI[2] >= 240f)
									calamityGlobalNPC.newAI[2] = -90f;

								npc.TargetClosest();
							}
						}
					}

					// Spin
					// Swim up
					destination += spinLocation;
					turnDistance = spinLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					chargeVelocityScalar += chargeVelocityScalarIncrement;
					if (chargeVelocityScalar > 1f)
						chargeVelocityScalar = 1f;

					baseVelocity *= invisiblePhaseVelocityMult;
					turnSpeed *= invisiblePhaseVelocityMult;

					// Spin around target
					if ((destination - npc.Center).Length() < spinLocationDistance || calamityGlobalNPC.newAI[1] > 0f)
					{
						if (rotationDirection == 0)
							rotationDirection = player.direction;

						calamityGlobalNPC.newAI[1] += 1f;

						if (Main.netMode != NetmodeID.MultiplayerClient)
							npc.Center = player.Center + new Vector2(spinRadius, 0).RotatedBy(npc.localAI[1]);

						npc.localAI[1] += MathHelper.ToRadians(degreesOfRotation) * rotationDirection;

						// Return to prevent other velocity code from being called
						return;
					}

					break;
			}

			// Increase velocity if velocity is ever zero
			if (npc.velocity == Vector2.Zero)
				npc.velocity.X = 2f * player.direction;

			// Acceleration
			if (!((destination - npc.Center).Length() < turnDistance))
			{
				float targetAngle = npc.AngleTo(destination);
				float f = npc.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
				npc.velocity = f.ToRotationVector2() * baseVelocity;
			}

			// Velocity upper limit
			if (npc.velocity.Length() > baseVelocity)
				npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * baseVelocity;

			// Reduce Y velocity if it's less than 1
			if (Math.Abs(npc.velocity.Y) < 1f)
				npc.velocity.Y -= 0.1f;

			// Storm code
			if (calamityGlobalNPC.newAI[3] == 0f)
			{
				// Start a storm
				if (Main.netMode == NetmodeID.MultiplayerClient || (Main.netMode == NetmodeID.SinglePlayer && Main.gameMenu))
					return;

				CalamityUtils.StartRain(true, true);
				calamityGlobalNPC.newAI[3] = 1f;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 70f && npc.Opacity == 1f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			return !CalamityUtils.AntiButcher(npc, ref damage, 0.1f);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return null;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 center = npc.Center;
            Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Width, ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Height) * 0.5f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.LightYellow) * npc.Opacity;
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 80, 100);
            DropHelper.DropItem(npc, ModContent.ItemType<EidolicWail>());
            DropHelper.DropItem(npc, ModContent.ItemType<SoulEdge>());
            DropHelper.DropItem(npc, ModContent.ItemType<HalibutCannon>());

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1, 50, 108);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas && Main.expertMode, 2, 15, 27);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1, 21, 32);
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);

			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 600, true);
        }
    }
}
