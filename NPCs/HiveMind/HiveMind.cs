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
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
/* states:
 * 0 = slow drift
 * 1 = reelback and teleport after spawn enemy
 * 2 = reelback for spin lunge + death legacy
 * 3 = spin lunge
 * 4 = semicircle spawn arc
 * 5 = raindash
 * 6 = deceleration
 */

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveMind : ModNPC
    {
		public static int normalIconIndex;
		public static int phase2IconIndex;

		internal static void LoadHeadIcons()
		{
			string normalIconPath = "CalamityMod/NPCs/HiveMind/HiveMind_Head_Boss";
			string phase2IconPath = "CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss";

			CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
			normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

			CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
			phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
		}

		// This block of values can be modified in SetDefaults() based on difficulty mode or something
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private int burrowTimer = 420;
		private int minimumDriftTime = 300;
		private int teleportRadius = 300;
		private int decelerationTime = 30;
		private int reelbackFade = 2; // Divide 255 by this for duration of reelback in ticks
		private float arcTime = 45f; // Ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)
		private float driftSpeed = 1f; // Default speed when slowly floating at player
		private float driftBoost = 1f; // Max speed added as health decreases
		private int lungeDelay = 90; // # of ticks long hive mind spends sliding to a stop before lunging
		private int lungeTime = 33;
		private int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
		private double lungeRots = 0.2; // Number of revolutions made while spinning/fading in for lunge
		private bool dashStarted = false;
		private int phase2timer = 360;
		private int rotationDirection;
		private double rotation;
		private double rotationIncrement;
		private int state = 0;
		private int previousState = 0;
		private int nextState = 0;
		private int reelCount = 0;
		private Vector2 deceleration;
		private int frameX = 0;
		private int frameY = 0;
		private const int maxFramesX_Phase2 = 2;
		private const int maxFramesY_Phase2 = 8;
		private const int height_Phase2 = 142;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Main.npcFrameCount[npc.type] = 16;
			NPCID.Sets.TrailingMode[npc.type] = 1;
			NPCID.Sets.TrailCacheLength[npc.type] = npc.oldPos.Length;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 178;
            npc.height = 122;
            npc.defense = 8;
            npc.LifeMaxNERB(8500, 10200, 350000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            npc.boss = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = CalamityMod.Instance.GetMusicFromMusicMod("HiveMind") ?? MusicID.Boss2;
            bossBag = ModContent.ItemType<HiveMindBag>();

            if (Main.expertMode)
            {
                minimumDriftTime = 120;
                reelbackFade = 4;
            }

            if (CalamityWorld.revenge)
            {
                lungeRots = 0.3;
                minimumDriftTime = 90;
                reelbackFade = 5;
                lungeTime = 28;
                driftSpeed = 2f;
                driftBoost = 2f;
            }

            if (CalamityWorld.death)
            {
                lungeRots = 0.4;
                minimumDriftTime = 60;
                reelbackFade = 6;
                lungeTime = 23;
                driftSpeed = 3f;
                driftBoost = 1f;
            }

			if (CalamityWorld.malice || BossRushEvent.BossRushActive)
			{
				lungeRots = 0.4;
				minimumDriftTime = 40;
				reelbackFade = 10;
				lungeTime = 16;
				driftSpeed = 6f;
				driftBoost = 1f;
			}

            phase2timer = minimumDriftTime;
            rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

		public override void BossHeadSlot(ref int index)
		{
			if (npc.life / (float)npc.lifeMax < 0.8f)
				index = phase2IconIndex;
			else
				index = normalIconIndex;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(biomeEnrageTimer);
			writer.Write(npc.dontTakeDamage);
			writer.Write(npc.noTileCollide);
			writer.Write(npc.noGravity);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[3]);
			writer.Write(burrowTimer);
			writer.Write(state);
            writer.Write(nextState);
            writer.Write(phase2timer);
            writer.Write(dashStarted);
            writer.Write(rotationDirection);
            writer.Write(rotation);
            writer.Write(previousState);
            writer.Write(reelCount);
            writer.Write(frameX);
			writer.Write(frameY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			biomeEnrageTimer = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
			npc.noTileCollide = reader.ReadBoolean();
			npc.noGravity = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			burrowTimer = reader.ReadInt32();
			state = reader.ReadInt32();
            nextState = reader.ReadInt32();
            phase2timer = reader.ReadInt32();
            dashStarted = reader.ReadBoolean();
            rotationDirection = reader.ReadInt32();
            rotation = reader.ReadDouble();
            previousState = reader.ReadInt32();
            reelCount = reader.ReadInt32();
            frameX = reader.ReadInt32();
			frameY = reader.ReadInt32();
        }

        public override void FindFrame(int frameHeight)
        {
			// When Hive Mind starts flying around
			bool phase2 = npc.life / (float)npc.lifeMax < 0.8f;

			if (phase2)
			{
				npc.frameCounter++;
				if (npc.frameCounter >= 6D)
				{
					// Reset frame counter
					npc.frameCounter = 0D;

					// Increment the Y frame
					frameY++;

					// Reset the Y frame if greater than 8
					if (frameY == maxFramesY_Phase2)
					{
						frameX++;
						frameY = 0;
					}

					// Reset the frames to frame 0
					if ((frameX * maxFramesY_Phase2) + frameY > 15)
						frameX = frameY = 0;
				}
			}
			else
			{
				npc.frameCounter += 1f / 6f;
				npc.frameCounter %= Main.npcFrameCount[npc.type];
				int frame = (int)npc.frameCounter;
				npc.frame.Y = frame * frameHeight;
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			// When Hive Mind starts flying around
			bool phase2 = npc.life / (float)npc.lifeMax < 0.8f;

			if (phase2)
			{
				SpriteEffects spriteEffects = npc.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				Texture2D texture = ModContent.GetTexture("CalamityMod/NPCs/HiveMind/HiveMindP2");
				Rectangle frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
				Vector2 vector = new Vector2(npc.width / 2, npc.height / 2);
				Color afterimageBaseColor = Color.White;
				int numAfterimages = 5;

				if (CalamityConfig.Instance.Afterimages && state != 0)
				{
					for (int i = 1; i < numAfterimages; i += 2)
					{
						Color afterimageColor = drawColor;
						afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
						afterimageColor = npc.GetAlpha(afterimageColor);
						afterimageColor *= (numAfterimages - i) / 15f;
						Vector2 afterimageCenter = npc.oldPos[i] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
						afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX_Phase2, maxFramesY_Phase2) * npc.scale / 2f;
						afterimageCenter += vector * npc.scale + new Vector2(0f, npc.gfxOffY);
						spriteBatch.Draw(texture, afterimageCenter, npc.frame, afterimageColor, npc.oldRot[i], vector, npc.scale, spriteEffects, 0f);
					}
				}

				Vector2 center = npc.Center - Main.screenPosition;
				spriteBatch.Draw(texture, center, frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);
				
				return false;
			}

			return true;
        }

        private void SpawnStuff()
        {
			int maxSpawns = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 5 : CalamityWorld.revenge ? 4 : Main.expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
			for (int i = 0; i < maxSpawns; i++)
			{
				int type = NPCID.EaterofSouls;
				int choice = -1;
				do
				{
					choice++;
					switch (choice)
					{
						case 0:
						case 1:
							type = NPCID.EaterofSouls;
							break;
						case 2:
							type = NPCID.DevourerHead;
							break;
						case 3:
						case 4:
							type = ModContent.NPCType<DankCreeper>();
							break;
						default:
							break;
					}
				}
				while (NPC.AnyNPCs(type) && choice < 5);

				if (choice < 5)
					NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), type);
			}
        }

        private void ReelBack()
        {
            npc.alpha = 0;
            phase2timer = 0;
            deceleration = npc.velocity / 255f * reelbackFade;

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                state = 2;
                Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SpawnStuff();

                state = nextState;
                nextState = 0;

                if (state == 2)
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                else
                    Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
            }
        }

        public override void AI()
        {
			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Enrage
			if ((!player.ZoneCorrupt || (npc.position.Y / 16f) < Main.worldSurface) && !BossRushEvent.BossRushActive)
			{
				if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
			}
			else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (biomeEnraged && (!player.ZoneCorrupt || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}
			if (biomeEnraged && ((npc.position.Y / 16f) < Main.worldSurface || malice))
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1f;
			}

			// When Hive Mind starts flying around
			bool phase2 = lifeRatio < 0.8f;

			// Phase 2 settings
			if (phase2)
			{
				// Spawn gores, play sound and reset every crucial variable at the start
				if (npc.localAI[1] == 0f)
				{
					npc.localAI[1] = 1f;

					int goreAmount = 7;
					for (int i = 1; i <= goreAmount; i++)
						Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindGore" + i), 1f);

					Main.PlaySound(SoundID.NPCDeath1, (int)npc.Center.X, (int)npc.Center.Y);

					npc.position = npc.Center;
					npc.height = height_Phase2;
					npc.position -= npc.Size * 0.5f;

					npc.frame.Y = 0;
					npc.noGravity = true;
					npc.noTileCollide = true;
					npc.scale = 1f;
					npc.alpha = 0;
					npc.dontTakeDamage = false;
					npc.damage = npc.defDamage;
				}

				npc.frame = new Rectangle(npc.width * frameX, npc.height * frameY, npc.width, npc.height);
			}
			else
			{
				CalamityGlobalNPC.hiveMind = npc.whoAmI;

				if (!player.active || player.dead)
				{
					npc.TargetClosest(false);
					player = Main.player[npc.target];
					if (!player.active || player.dead)
					{
						if (npc.timeLeft > 60)
							npc.timeLeft = 60;

						if (npc.localAI[3] < 120f)
							npc.localAI[3] += 1f;

						if (npc.localAI[3] > 60f)
						{
							npc.velocity.Y += (npc.localAI[3] - 60f) * 0.5f;

							npc.noGravity = true;
							npc.noTileCollide = true;

							if (burrowTimer > 30)
								burrowTimer = 30;
						}

						return;
					}
				}
				else if (npc.timeLeft < 1800)
					npc.timeLeft = 1800;

				if (npc.localAI[3] > 0f)
				{
					npc.localAI[3] -= 1f;
					return;
				}

				npc.noGravity = false;
				npc.noTileCollide = false;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (npc.localAI[0] == 0f)
					{
						npc.localAI[0] = 1f;
						int maxBlobs = death ? 15 : revenge ? 7 : expertMode ? 6 : 5;
						for (int i = 0; i < maxBlobs; i++)
							NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<HiveBlob>(), npc.whoAmI);
					}
				}

				if (npc.ai[3] == 0f && npc.life > 0)
					npc.ai[3] = npc.lifeMax;

				if (npc.life > 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int num660 = (int)(npc.lifeMax * 0.05);
						if ((npc.life + num660) < npc.ai[3])
						{
							npc.ai[3] = npc.life;

							int maxSpawns = malice ? 10 : death ? 5 : revenge ? 4 : expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
							int maxDankSpawns = malice ? 4 : death ? Main.rand.Next(2, 4) : revenge ? 2 : expertMode ? Main.rand.Next(1, 3) : 1;

							for (int num662 = 0; num662 < maxSpawns; num662++)
							{
								int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
								int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));

								int type = ModContent.NPCType<HiveBlob>();
								if (NPC.CountNPCS(ModContent.NPCType<DankCreeper>()) < maxDankSpawns)
									type = ModContent.NPCType<DankCreeper>();

								int num664 = NPC.NewNPC(x, y, type);
								Main.npc[num664].SetDefaults(type);
								if (Main.netMode == NetmodeID.Server && num664 < Main.maxNPCs)
									NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num664, 0f, 0f, 0f, 0, 0, 0);
							}

							return;
						}
					}
				}

				burrowTimer--;
				if (burrowTimer < -120)
				{
					burrowTimer = (death ? 180 : revenge ? 300 : expertMode ? 360 : 420) - (int)enrageScale * 55;
					if (burrowTimer < 30)
						burrowTimer = 30;

					npc.scale = 1f;
					npc.alpha = 0;
					npc.dontTakeDamage = false;
					npc.damage = npc.defDamage;
				}
				else if (burrowTimer < -60)
				{
					npc.scale += 0.0165f;
					npc.alpha -= 4;

					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
					Main.dust[num622].velocity *= 2f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}

					for (int i = 0; i < 2; i++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 3.5f * npc.scale);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 3.5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
						Main.dust[num624].velocity *= 1f;
					}
				}
				else if (burrowTimer == -60)
				{
					npc.scale = 0.01f;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						npc.Center = player.Center;
						npc.position.Y = player.position.Y - npc.height;
						int tilePosX = (int)npc.Center.X / 16;
						int tilePosY = (int)(npc.position.Y + npc.height) / 16 + 1;

						if (Main.tile[tilePosX, tilePosY] == null)
							Main.tile[tilePosX, tilePosY] = new Tile();

						while (!(Main.tile[tilePosX, tilePosY].nactive() && Main.tileSolid[Main.tile[tilePosX, tilePosY].type]))
						{
							tilePosY++;
							npc.position.Y += 16;
							if (Main.tile[tilePosX, tilePosY] == null)
								Main.tile[tilePosX, tilePosY] = new Tile();
						}
					}
					npc.netUpdate = true;
				}
				else if (burrowTimer < 0)
				{
					npc.scale -= 0.0165f;
					npc.alpha += 4;

					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
					Main.dust[num622].velocity *= 2f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}

					for (int i = 0; i < 2; i++)
					{
						int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 3.5f * npc.scale);
						Main.dust[num624].noGravity = true;
						Main.dust[num624].velocity *= 3.5f;
						num624 = Dust.NewDust(new Vector2(npc.position.X, npc.Center.Y), npc.width, npc.height / 2, 14, 0f, -3f, 100, default, 2.5f * npc.scale);
						Main.dust[num624].velocity *= 1f;
					}
				}
				else if (burrowTimer == 0)
				{
					if (!player.active || player.dead)
					{
						burrowTimer = 30;
					}
					else
					{
						npc.TargetClosest();
						npc.dontTakeDamage = true;
						npc.damage = 0;
					}
				}

				return;
			}

			if (npc.alpha != 0)
            {
                if (npc.damage != 0)
                    npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            switch (state)
            {
                case 0: // Slowdrift

                    if (npc.alpha > 0)
                        npc.alpha -= 3;

                    if (nextState == 0)
                    {
						npc.TargetClosest();
						if (revenge && lifeRatio < 0.53f)
                        {
							if (death)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else if (lifeRatio < 0.27f)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else
							{
								do
									nextState = Main.rand.Next(3, 5);
								while (nextState == previousState);
								previousState = nextState;
							}
                        }
                        else
                        {
                            if (revenge && (Main.rand.NextBool(3) || reelCount == 2))
                            {
                                reelCount = 0;
                                nextState = 2;
                            }
                            else
                            {
                                reelCount++;
								if (Main.expertMode && reelCount == 2)
								{
									reelCount = 0;
									nextState = 2;
								}
								else
									nextState = 1;

                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                            }
                        }

                        if (nextState == 3)
                            rotation = MathHelper.ToRadians(Main.rand.Next(360));

                        npc.netUpdate = true;
                    }

                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
                    {
                        npc.TargetClosest(false);
						player = Main.player[npc.target];
						if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
						{
							if (npc.timeLeft > 60)
								npc.timeLeft = 60;

							if (npc.localAI[3] < 120f)
								npc.localAI[3] += 1f;

							if (npc.localAI[3] > 60f)
								npc.velocity.Y += (npc.localAI[3] - 60f) * 0.5f;

							return;
						}
                    }
					else if (npc.timeLeft < 1800)
						npc.timeLeft = 1800;

					if (npc.localAI[3] > 0f)
                    {
						npc.localAI[3] -= 1f;
                        return;
                    }

                    npc.velocity = player.Center - npc.Center;

                    phase2timer--;
                    if (phase2timer <= -180) // No stalling drift mode forever
                    {
                        npc.velocity *= 2f / 255f * (reelbackFade + 2 * (int)enrageScale);
                        ReelBack();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.velocity.Normalize();
						if (expertMode) // Variable velocity in expert and up
                            npc.velocity *= driftSpeed + enrageScale + driftBoost * lifeRatio;
                        else
                            npc.velocity *= driftSpeed + enrageScale;
                    }

                    break;

                case 1: // Reelback and teleport

                    npc.alpha += reelbackFade + 2 * (int)enrageScale;
                    npc.velocity -= deceleration;

                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        state = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] != 0f && npc.ai[2] != 0f)
                        {
                            npc.position.X = npc.ai[1] * 16 - npc.width / 2;
                            npc.position.Y = npc.ai[2] * 16 - npc.height / 2;
                        }

                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] == 0f && npc.ai[2] == 0f)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int posX = (int)player.Center.X / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            int posY = (int)player.Center.Y / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            if (!WorldGen.SolidTile(posX, posY) && Collision.CanHit(new Vector2(posX * 16, posY * 16), 1, 1, player.position, player.width, player.height))
                            {
                                npc.ai[1] = posX;
                                npc.ai[2] = posY;
                                npc.netUpdate = true;
                                break;
                            }
                        }
                    }

                    break;

                case 2: // Reelback for lunge + death legacy

                    npc.alpha += reelbackFade + 2 * (int)enrageScale;
                    npc.velocity -= deceleration;

                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        dashStarted = false;

                        if (revenge && lifeRatio < 0.53f)
                        {
							state = nextState;
                            nextState = 0;
                            previousState = state;
                        }
                        else
                            state = 3;

                        if (player.velocity.X > 0)
                            rotationDirection = 1;
                        else if (player.velocity.X < 0)
                            rotationDirection = -1;
                        else
                            rotationDirection = player.direction;
                    }

                    break;

                case 3: // Lunge

                    npc.netUpdate = true;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= lungeFade;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);

                        rotation += rotationIncrement * rotationDirection;
                        phase2timer = lungeDelay;
                    }
                    else
                    {
                        phase2timer--;
                        if (!dashStarted)
                        {
                            if (phase2timer <= 0)
                            {
                                phase2timer = lungeTime - 4 * (int)enrageScale;
                                npc.velocity = player.Center + (malice ? player.velocity * 20f : Vector2.Zero) - npc.Center;
                                npc.velocity.Normalize();
                                npc.velocity *= teleportRadius / (lungeTime - (int)enrageScale);
                                dashStarted = true;
                                Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);

                                rotation += rotationIncrement * rotationDirection * phase2timer / lungeDelay;
                            }
                        }
                        else
                        {
                            if (phase2timer <= 0)
                            {
                                state = 6;
                                phase2timer = 0;
                                deceleration = npc.velocity / decelerationTime;
                            }
                        }
                    }

                    break;

                case 4: // Enemy spawn arc

                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y += teleportRadius;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = MathHelper.Pi * teleportRadius / arcTime;
                            npc.velocity *= rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);

                            phase2timer++;
                            if (phase2timer == (int)arcTime / 6)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient && Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height))
                                {
                                    if (npc.ai[0] == 2 || npc.ai[0] == 4)
                                    {
                                        if (expertMode && !NPC.AnyNPCs(ModContent.NPCType<DarkHeart>()))
                                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DarkHeart>());
									}
                                    else if (!NPC.AnyNPCs(NPCID.EaterofSouls))
                                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                                }

                                if (npc.ai[0] == 6)
                                {
                                    npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);
                                    SpawnStuff();
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 5: // Rain dash

                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y -= teleportRadius;
                            npc.position.X += teleportRadius * rotationDirection;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = teleportRadius / arcTime * 3;
                            npc.velocity *= -rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 20)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
									int type = ModContent.ProjectileType<ShadeNimbusHostile>();
									int damage = npc.GetProjectileDamage(type);
									Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), 0, 0, type, damage, 0, Main.myPlayer, 11, 0);
                                }

                                if (npc.ai[0] == 10)
                                {
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 6: // Deceleration

                    npc.velocity -= deceleration;
                    phase2timer++;
                    if (phase2timer == decelerationTime)
                    {
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        state = 0;
                        npc.netUpdate = true;
                    }

                    break;
            }
        }

		public override bool? CanHitNPC(NPC target) => npc.alpha == 0; // Can only be hit while fully visible

		// Can only hit the target if within certain distance
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
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

			return minDist <= 60f && npc.alpha == 0 && npc.scale == 1f; // No damage while not fully visible or shrunk
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => npc.scale == 1f; // Only draw HP bar while at full size

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (phase2timer < 0 && damage > 1)
            {
                npc.velocity *= -4f;
                ReelBack();
                npc.netUpdate = true;
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
            for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);

			// When Hive Mind starts flying around
			bool phase2 = npc.life / (float)npc.lifeMax < 0.8f;

			if (phase2)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(15) && NPC.CountNPCS(ModContent.NPCType<HiveBlob2>()) < 2)
				{
					Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
					NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveBlob2>());
				}
			}
			else
			{
				if (NPC.CountNPCS(NPCID.EaterofSouls) < 3 && NPC.CountNPCS(NPCID.DevourerHead) < 1)
				{
					if (Main.rand.NextBool(60) && Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
						NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.EaterofSouls);
					}

					if (Main.rand.NextBool(150) && Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
						NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, NPCID.DevourerHead);
					}
				}
			}

            if (npc.life <= 0)
            {
                int goreAmount = 10;
                for (int i = 1; i <= goreAmount; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindP2Gore" + i), 1f);

                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 200;
                npc.height = 150;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeHiveMind>(), true, !CalamityWorld.downedHiveMind);

			CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, CalamityWorld.downedHiveMind);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<TrueShadowScale>(), 25, 30, 5);
                DropHelper.DropItemSpray(npc, ItemID.DemoniteBar, 8, 12, 2);
                DropHelper.DropItemSpray(npc, ItemID.RottenChunk, 9, 15, 3);
                if (Main.hardMode)
                    DropHelper.DropItemSpray(npc, ItemID.CursedFlame, 10, 20, 2);
                DropHelper.DropItem(npc, ItemID.CorruptSeeds, 10, 15);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<PerfectDark>(w),
                    DropHelper.WeightStack<LeechingDagger>(w),
                    DropHelper.WeightStack<Shadethrower>(w),
                    DropHelper.WeightStack<ShadowdropStaff>(w),
                    DropHelper.WeightStack<ShaderainStaff>(w),
                    DropHelper.WeightStack<DankStaff>(w),
                    DropHelper.WeightStack<RotBall>(w, 30, 50),
					DropHelper.WeightStack<FilthyGlove>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<RottenBrain>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RottingEyeball>(), 10);
            }

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator)
            {
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
                CalamityUtils.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, 0.4f, 0.6f, 3, 8);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark The Hive Mind as dead
            CalamityWorld.downedHiveMind = true;
            CalamityNetcode.SyncWorld();
        }
    }
}
