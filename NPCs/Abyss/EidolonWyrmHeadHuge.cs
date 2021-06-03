using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
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
		Vector2 chargeDestination = default;
		public const int minLength = 40;
        public const int maxLength = 41;
        bool TailSpawned = false;

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
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
			chargeDestination = reader.ReadVector2();
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

			// General AI pattern
			// Charge : Phase 5 - 
			// Charge : Phase 2 - Spin around target and summon shadow fireballs
			// Charge : Phase 4 - Swim to the right and dash towards the target, summon lightning bolts from above during it
			// Turn invisible, swim above the target and summon predictive lightning bolts
			// Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
			// Swim a certain distance away from the target for a bit to prepare for 3 charges again
			// Charge : Phase 5 - 
			// Charge : Phase 3 - Turn invisible and summon ancient dooms around the target
			// Charge : Phase 4 - Turn visible, swim to the left and dash towards the target, summon lightning bolts from above during it
			// Turn invisible, swim beneath the target and summon ice mist
			// Turn visible and charge towards the target quickly 1 time, soon after the previous attack ends
			// Swim a certain distance away from the target for a bit to prepare for 3 charges again

			// Vector to swim to
			Vector2 destination = player.Center;

			// Locations to charge from
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

			// Velocity and turn speed values
			float minVelocity = !player.wet ? 20f : 10f;
			float velocity = Math.Min(minVelocity, Math.Max(minVelocity * 0.4f, player.velocity.Length()));
			float turnSpeed = !player.wet ? MathHelper.ToRadians(4f) : MathHelper.ToRadians(2f);

			// Phase gate values
			float chargePhaseGateValue = 180f;
			float chargeGateValue = chargePhaseGateValue + 10f;

			// Phase switch
			switch ((int)calamityGlobalNPC.newAI[0])
			{
				// First charge combo
				case 0:

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

								velocity *= 0.5f;
								turnSpeed *= 2f;
							}
							else
							{
								// Charge
								velocity *= 2f;
								turnSpeed *= 0.5f;

								destination = chargeDestination;

								if ((destination - npc.Center).Length() < chargeLocationDistance)
								{
									// Spin around target and summon shadow fireballs
									if (phase2)
										calamityGlobalNPC.newAI[0] = 6f;

									npc.ai[3] += 1f;
									float maxCharges = phase4 ? 1 : phase2 ? 2 : 3;
									if (npc.ai[3] >= maxCharges)
									{
										npc.ai[3] = 0f;
										calamityGlobalNPC.newAI[0] = 1f;
									}

									calamityGlobalNPC.newAI[1] += 1f;
									if (calamityGlobalNPC.newAI[1] > 7f)
										calamityGlobalNPC.newAI[1] = 0f;

									calamityGlobalNPC.newAI[2] = 0f;
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
				case 1:
					break;
				// Turn visible and charge quickly
				case 2:
					break;
				// Swim away
				case 3:
					break;
				// Second charge combo
				case 4:

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

								velocity *= 0.5f;
								turnSpeed *= 2f;
							}
							else
							{
								// Charge
								velocity *= 2f;
								turnSpeed *= 0.5f;

								destination = chargeDestination;

								if ((destination - npc.Center).Length() < chargeLocationDistance)
								{
									// Turn invisible and summon ancient dooms around the target
									if (phase3)
										calamityGlobalNPC.newAI[0] = 7f;

									npc.ai[3] += 1f;
									float maxCharges = phase4 ? 1 : phase3 ? 2 : 3;
									if (npc.ai[3] >= maxCharges)
									{
										npc.ai[3] = 0f;
										calamityGlobalNPC.newAI[0] = 1f;
									}

									calamityGlobalNPC.newAI[1] += 1f;
									if (calamityGlobalNPC.newAI[1] > 7f)
										calamityGlobalNPC.newAI[1] = 0f;

									calamityGlobalNPC.newAI[2] = 0f;
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
				case 5:
					break;
				// Phase 2 attack - Get in position for spin, spin around target and summon shadow fireballs
				case 6:
					break;
				// Phase 3 attack - Turn invisible and summon ancient dooms around the target
				case 7:
					break;
				// Phase 4 attack - Swim to the right and dash towards the target, summon lightning bolts from above during it
				case 8:
					break;
				//Phase 4 attack - Turn visible, swim to the left and dash towards the target, summon lightning bolts from above during it
				case 9:
					break;
			}

			// Adjust opacity
			bool invisiblePhase = calamityGlobalNPC.newAI[0] == 1f || calamityGlobalNPC.newAI[0] == 5f || calamityGlobalNPC.newAI[0] == 7f;
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

			// Increase velocity if velocity is ever zero
			if (npc.velocity == Vector2.Zero)
				npc.velocity.X = 2f * player.direction;

			// Acceleration
			if (!((destination - npc.Center).Length() < turnDistance))
			{
				float targetAngle = npc.AngleTo(destination);
				float f = npc.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
				npc.velocity = f.ToRotationVector2() * velocity;
			}

			// Velocity upper limit
			if (npc.velocity.Length() > velocity)
				npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * velocity;

			// Reduce Y velocity if it's less than 1
			if (Math.Abs(npc.velocity.Y) < 1f)
				npc.velocity.Y -= 0.1f;

			// Rotation and direction
			npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			int direction = npc.direction;
			npc.direction = npc.spriteDirection = (npc.velocity.X > 0f) ? 1 : (-1);
			if (direction != npc.direction)
				npc.netUpdate = true;

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

			return minDist <= 70f;
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
