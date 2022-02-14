using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Leviathan
{
    [AutoloadBossHead]
    public class Siren : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private bool spawnedLevi = false;
        private bool forceChargeFrames = false;
        private int frameUsed = 0;
		public bool HasBegunSummoningLeviathan = false;
		public bool WaitingForLeviathan
        {
            get
            {
				if (Main.npc.IndexInRange(CalamityGlobalNPC.leviathan) && Main.npc[CalamityGlobalNPC.leviathan].life / (float)Main.npc[CalamityGlobalNPC.leviathan].lifeMax >= 0.4f)
					return true;

				return CalamityUtils.FindFirstProjectile(ModContent.ProjectileType<LeviathanSpawner>()) != -1;
            }
        }

		//IMPORTANT: Do NOT remove the empty space on the sprites.  This is intentional for framing.  The sprite is centered and hitbox is fine already.

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anahita");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 16f;
            npc.width = 100;
            npc.height = 100;
            npc.defense = 20;
			npc.DR_NERD(0.2f);
            npc.LifeMaxNERB(34700, 41600, 260000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 15, 0, 0);
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
			music = CalamityMod.Instance.GetMusicFromMusicMod("Anahita") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<LeviathanBag>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(biomeEnrageTimer);
			writer.Write(spawnedLevi);
            writer.Write(forceChargeFrames);
            writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			writer.Write(frameUsed);
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.Calamity().newAI[0]);
			writer.Write(HasBegunSummoningLeviathan);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			biomeEnrageTimer = reader.ReadInt32();
			spawnedLevi = reader.ReadBoolean();
            forceChargeFrames = reader.ReadBoolean();
            npc.localAI[0] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			frameUsed = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
			npc.Calamity().newAI[0] = reader.ReadSingle();
			HasBegunSummoningLeviathan = reader.ReadBoolean();
		}

		public override void AI()
        {
            // whoAmI variable
            CalamityGlobalNPC.siren = npc.whoAmI;

			// Set to false so she doesn't do it constantly
			npc.Calamity().canBreakPlayerDefense = false;

			// Light
			Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0f, 0.5f, 0.3f);

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Check for Leviathan
			bool leviAlive = false;
			if (CalamityGlobalNPC.leviathan != -1)
				leviAlive = Main.npc[CalamityGlobalNPC.leviathan].active;

			// Variables
			Player player = Main.player[npc.target];
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool notOcean = player.position.Y < 800f || player.position.Y > Main.worldSurface * 16.0 || (player.position.X > 6400f && player.position.X < (Main.maxTilesX * 16 - 6400));

			// Enrage
			if (notOcean && !BossRushEvent.BossRushActive)
			{
				if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
			}
			else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 0.5f : 0f;
			if (biomeEnraged)
			{
				npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
				enrageScale += 1.5f;
			}

			float lifeRatio = npc.life / (float)npc.lifeMax;

			float bubbleVelocity = death ? 9f : revenge ? 7f : expertMode ? 6f : 5f;
			bubbleVelocity += 4f * enrageScale;
			if (!leviAlive)
				bubbleVelocity += 2f * (1f - lifeRatio);

			npc.damage = npc.defDamage;

			Vector2 vector = npc.Center;

			// Phases
			bool phase2 = lifeRatio < 0.7f;
            bool phase3 = lifeRatio < 0.4f;
			bool phase4 = lifeRatio < 0.2f;

			// Spawn Leviathan and change music
			if (npc.life / (float)npc.lifeMax < 0.4f)
			{
				if (!HasBegunSummoningLeviathan)
				{
					// Use charging frames.
					npc.ai[0] = 3f;

					// Look towards the ocean.
					npc.direction = (npc.Center.X < Main.maxTilesX * 8f).ToDirectionInt();

					// Prevent Anahita from leaving the world when doing her dive.
					// If she leaves the world entity summons will fail and Levi will simply not spawn.
					npc.position.X = MathHelper.Clamp(npc.position.X, 150f, Main.maxTilesX * 16f - 150f);

					if (npc.alpha <= 0)
					{
						float moveDirection = 1f;
						if (Math.Abs(npc.Center.X - Main.maxTilesX * 16f) > Math.Abs(npc.Center.X))
							moveDirection = -1f;
						npc.velocity.X = moveDirection * 6f;
						npc.spriteDirection = (int)-moveDirection;
						npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + 0.2f, -3f, 16f);
					}

					float idealRotation = npc.velocity.ToRotation();
					if (npc.spriteDirection == 1)
						idealRotation += MathHelper.Pi;

					npc.rotation = npc.rotation.AngleTowards(idealRotation, 0.08f);

					if (BossRushEvent.BossRushActive || Collision.WetCollision(npc.position, npc.width, npc.height) || npc.position.Y > (Main.worldSurface - 125f) * 16f)
					{
						int oldAlpha = npc.alpha;
						npc.alpha = Utils.Clamp(npc.alpha + 9, 0, 255);
						if (Main.netMode != NetmodeID.MultiplayerClient && npc.alpha >= 255 && oldAlpha < 255)
						{
							Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<LeviathanSpawner>(), 0, 0f);
							HasBegunSummoningLeviathan = true;
							npc.netUpdate = true;
						}

						npc.velocity *= 0.9f;
					}
					npc.dontTakeDamage = true;
					return;
				}
			}

			// Change music.
			if (leviAlive)
				music = CalamityMod.Instance.GetMusicFromMusicMod("LeviathanAndAnahita") ?? MusicID.Boss3;

			// Ice Shield
			if (npc.localAI[2] < 3f)
			{
				if (npc.ai[3] == 0f && npc.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					int num6 = NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<SirenIce>(), npc.whoAmI);
					npc.ai[3] = num6 + 1;
					npc.localAI[1] = -1f;
					npc.localAI[2] += 1f;
					npc.netUpdate = true;
					Main.npc[num6].ai[0] = npc.whoAmI;
					Main.npc[num6].netUpdate = true;
				}

				int num7 = (int)npc.ai[3] - 1;
				if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<SirenIce>())
					npc.dontTakeDamage = true;
				else
				{
					npc.dontTakeDamage = false;
					npc.ai[3] = 0f;

					if (npc.localAI[1] == -1f)
						npc.localAI[1] = 1f;
					else
					{
						switch ((int)npc.localAI[2])
						{
							case 1:
								if (phase2)
									npc.localAI[1] = 0f;
								break;
							case 2:
								if (phase3)
									npc.localAI[1] = 0f;
								break;
						}
					}
				}
			}
			else
			{
				npc.dontTakeDamage = false;

				int num7 = (int)npc.ai[3] - 1;
				if (num7 != -1 && Main.npc[num7].active && Main.npc[num7].type == ModContent.NPCType<SirenIce>())
					npc.dontTakeDamage = true;
			}

			// Hover near the target, invisible if the Leviathan is present and not sufficiently injured.
			if (npc.life / (float)npc.lifeMax < 0.4f && WaitingForLeviathan)
			{
				ChargeRotation(player, vector);
				ChargeLocation(player, vector, false, true);

				npc.alpha += 3;
				if (npc.alpha >= 255)
					npc.alpha = 255;
				else
				{
					for (int k = 0; k < 3; k++)
					{
						int dust = Dust.NewDust(npc.position, npc.width, npc.height, 172, 0f, 0f, 100, default, 2f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].noLight = true;
					}
				}

				npc.dontTakeDamage = true;
				npc.damage = 0;

				if (npc.ai[0] != -1f)
				{
					npc.ai[0] = -1f;
					npc.ai[1] = 0f;
					npc.ai[2] = 0f;
					npc.localAI[0] = 0f;
					npc.netUpdate = true;
				}

				return;
			}

			// Alpha
			if (npc.damage != 0)
			{
				npc.alpha -= 5;
				if (npc.alpha <= 0)
					npc.alpha = 0;
			}

            // Play sound
            if (Main.rand.NextBool(300))
                Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 35);

            // Time left
            if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

			// Despawn
			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
				{
					npc.rotation = npc.velocity.X * 0.02f;

					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.2f;
					if (npc.velocity.Y > 16f)
						npc.velocity.Y = 16f;

					if (npc.position.Y > Main.worldSurface * 16.0)
					{
						for (int x = 0; x < Main.maxNPCs; x++)
						{
							if (Main.npc[x].type == ModContent.NPCType<Leviathan>())
							{
								Main.npc[x].active = false;
								Main.npc[x].netUpdate = true;
							}
						}
						npc.active = false;
						npc.netUpdate = true;
					}

					if (npc.ai[0] != -1f)
					{
						npc.ai[0] = -1f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.localAI[0] = 0f;
						npc.netUpdate = true;
					}

					return;
				}
			}

			// Rotation when charging
			if (npc.ai[0] > 2f)
                ChargeRotation(player, vector);

            // Phase switch
            if (npc.ai[0] == -1f)
            {
                int random = ((phase2 && expertMode && !leviAlive) || phase4) ? 4 : 3;
				int num618;
				do num618 = Main.rand.Next(random);
				while (num618 == npc.ai[1] || num618 == 1);

				npc.ai[0] = num618;

                if (npc.ai[0] != 3f)
                {
                    forceChargeFrames = false;
                    float playerLocation = vector.X - player.Center.X;
                    npc.direction = playerLocation < 0f ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }
                else
                    ChargeRotation(player, vector);

				npc.TargetClosest();
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }

            // Get in position for bubble spawn
            else if (npc.ai[0] == 0f)
            {
                npc.rotation = npc.velocity.X * 0.02f;
                npc.spriteDirection = npc.direction;

                Vector2 vector118 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num1055 = player.position.X + (player.width / 2) - vector118.X;
                float num1056 = player.position.Y + (player.height / 2) - 200f - vector118.Y;
                float num1057 = (float)Math.Sqrt(num1055 * num1055 + num1056 * num1056);

				npc.Calamity().newAI[0] += 1f;
                if (num1057 < 600f || npc.Calamity().newAI[0] >= 180f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
					npc.Calamity().newAI[0] = 0f;
					npc.netUpdate = true;
                    return;
                }

				float maxVelocityY = death ? 3f : 4f;
				float maxVelocityX = death ? 7f : 8f;
				maxVelocityY -= enrageScale;
				maxVelocityX -= 2f * enrageScale;

				if (npc.position.Y > player.position.Y - 350f)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y -= death ? 0.12f : 0.1f;
                    if (npc.velocity.Y > maxVelocityY)
                        npc.velocity.Y = maxVelocityY;
                }
                else if (npc.position.Y < player.position.Y - 450f)
                {
                    if (npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.98f;
                    npc.velocity.Y += death ? 0.12f : 0.1f;
                    if (npc.velocity.Y < -maxVelocityY)
                        npc.velocity.Y = -maxVelocityY;
                }

                if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 100f)
                {
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X -= death ? 0.12f : 0.1f;
                    if (npc.velocity.X > maxVelocityX)
                        npc.velocity.X = maxVelocityX;
                }

                if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 100f)
                {
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                    npc.velocity.X += death ? 0.12f : 0.1f;
                    if (npc.velocity.X < -maxVelocityX)
                        npc.velocity.X = -maxVelocityX;
                }
            }

            // Bubble spawn
            else if (npc.ai[0] == 1f)
            {
                npc.rotation = npc.velocity.X * 0.02f;
                Vector2 vector119 = new Vector2(npc.position.X + (npc.width / 2) + (15 * npc.direction), npc.position.Y + 30);
                Vector2 vector120 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num1058 = player.position.X + (player.width / 2) - vector120.X;
                float num1059 = player.position.Y + (player.height / 2) - vector120.Y;
                float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                npc.ai[1] += 1f;
				int num638 = 0;
				for (int num639 = 0; num639 < 255; num639++)
				{
					if (Main.player[num639].active && !Main.player[num639].dead && (vector - Main.player[num639].Center).Length() < 1000f)
						num638++;
				}
				npc.ai[1] += num638 / 2;

                bool flag103 = false;
				float num640 = 20f;
				if (!leviAlive || phase4)
					num640 -= death ? 15f * (1f - lifeRatio) : 12f * (1f - lifeRatio);

				if (npc.ai[1] > num640)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    flag103 = true;
                }

                if (Collision.CanHit(vector119, 1, 1, player.position, player.width, player.height) && flag103)
                {
                    Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 85);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int spawn = NPC.NewNPC((int)vector119.X, (int)vector119.Y, NPCID.DetonatingBubble);
						Main.npc[spawn].target = npc.target;
						Main.npc[spawn].velocity = player.Center - vector119;
						Main.npc[spawn].velocity.Normalize();
						Main.npc[spawn].velocity *= bubbleVelocity;
						Main.npc[spawn].netUpdate = true;
						Main.npc[spawn].ai[3] = Main.rand.Next(80, 121) / 100f;
					}
                }

                if (num1060 > 600f)
                {
					float maxVelocityY = death ? 3f : 4f;
					float maxVelocityX = death ? 7f : 8f;
					maxVelocityY -= enrageScale;
					maxVelocityX -= 2f * enrageScale;

					if (npc.position.Y > player.position.Y - 350f)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y *= 0.98f;
						npc.velocity.Y -= death ? 0.12f : 0.1f;
						if (npc.velocity.Y > maxVelocityY)
							npc.velocity.Y = maxVelocityY;
					}
					else if (npc.position.Y < player.position.Y - 450f)
					{
						if (npc.velocity.Y < 0f)
							npc.velocity.Y *= 0.98f;
						npc.velocity.Y += death ? 0.12f : 0.1f;
						if (npc.velocity.Y < -maxVelocityY)
							npc.velocity.Y = -maxVelocityY;
					}

					if (npc.position.X + (npc.width / 2) > player.position.X + (player.width / 2) + 100f)
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X *= 0.98f;
						npc.velocity.X -= death ? 0.12f : 0.1f;
						if (npc.velocity.X > maxVelocityX)
							npc.velocity.X = maxVelocityX;
					}

					if (npc.position.X + (npc.width / 2) < player.position.X + (player.width / 2) - 100f)
					{
						if (npc.velocity.X < 0f)
							npc.velocity.X *= 0.98f;
						npc.velocity.X += death ? 0.12f : 0.1f;
						if (npc.velocity.X < -maxVelocityX)
							npc.velocity.X = -maxVelocityX;
					}
				}
                else
                    npc.velocity *= 0.9f;

                npc.spriteDirection = npc.direction;

				float maxBubbleSpawn = death ? 3f : 4f;
                if (npc.ai[2] > maxBubbleSpawn)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            // Fly near target and summon bullet hells
            else if (npc.ai[0] == 2f)
            {
                npc.rotation = npc.velocity.X * 0.02f;

				Vector2 targetVector = player.Center + new Vector2(0f, -350f);
				float velocity = death ? 13.5f : 12f;
				velocity += 6f * enrageScale;
				Vector2 vector3 = Vector2.Normalize(targetVector - vector - npc.velocity) * velocity;
				float acceleration = death ? 0.28f : 0.25f;
				acceleration += 0.2f * enrageScale;

				if (Math.Abs(npc.Center.Y - targetVector.Y) > 50f || Math.Abs(npc.Center.X - player.Center.X) > 350f)
					npc.SimpleFlyMovement(vector3, acceleration);

				npc.ai[1] += 1f;

				float divisor = 140f;
				divisor -= (int)(30f * enrageScale);
				if (!leviAlive || phase4)
					divisor -= (float)Math.Ceiling(50f * (1f - lifeRatio));

				bool shootProjectiles = npc.ai[1] % divisor == 0f;
				if (Main.netMode != NetmodeID.MultiplayerClient && shootProjectiles)
				{
					float projectileVelocity = expertMode ? 3f : 2f;
					projectileVelocity += enrageScale;
					if (!leviAlive || phase4)
						projectileVelocity += death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);

					int totalProjectiles = 8;
					int projectileDistance = 600;
					int type = ModContent.ProjectileType<WaterSpear>();
					switch ((int)npc.localAI[3])
					{
						case 0:
							Main.PlaySound(SoundID.Item21, player.position);
							break;
						case 1:
							totalProjectiles = 3;
							type = ModContent.ProjectileType<FrostMist>();
							Main.PlaySound(SoundID.Item30, player.position);
							break;
						case 2:
							totalProjectiles = 6;
							type = ModContent.ProjectileType<SirenSong>();
							float soundPitch = (Main.rand.NextFloat() - 0.5f) * 0.5f;
							Main.harpNote = soundPitch;
							Main.PlaySound(SoundID.Item26, player.position);
							break;
					}
					npc.localAI[3] += 1f;
					if (npc.localAI[3] > 2f)
						npc.localAI[3] = 0f;

					if ((phase2 && !leviAlive) || phase4)
						totalProjectiles += totalProjectiles / 2;

					float radians = MathHelper.TwoPi / totalProjectiles;
					int damage = npc.GetProjectileDamage(type);
					for (int i = 0; i < totalProjectiles; i++)
					{
						Vector2 spawnVector = player.Center + Vector2.Normalize(new Vector2(0f, -projectileVelocity).RotatedBy(radians * i)) * projectileDistance;
						Vector2 projVelocity = Vector2.Normalize(player.Center - spawnVector) * projectileVelocity;
						Projectile.NewProjectile(spawnVector, projVelocity, type, damage, 0f, Main.myPlayer);
					}
				}

				if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
				{
					float playerLocation = vector.X - player.Center.X;
					npc.direction = playerLocation < 0f ? 1 : -1;
					npc.spriteDirection = npc.direction;
				}

				float phaseTimer = 300f;
				phaseTimer -= 50f * enrageScale;
				if (!leviAlive || phase4)
					phaseTimer -= 150f * (1f - lifeRatio);

				if (npc.ai[1] >= phaseTimer)
                {
                    npc.ai[0] = -1f;
                    npc.ai[1] = 2f;
                    npc.netUpdate = true;
                }
            }

            // Set up charge
            else if (npc.ai[0] == 3f)
            {
                ChargeLocation(player, vector, leviAlive && !phase4, revenge);

                npc.ai[1] += 1f;

                if (npc.ai[1] >= (revenge ? 45f : 60f))
                {
                    forceChargeFrames = true;
					float aiInterval = (leviAlive && !phase4) ? 2f : 3f;
					npc.ai[0] = (npc.ai[2] >= aiInterval) ? -1f : 4f;
                    npc.ai[1] = (npc.ai[2] >= aiInterval) ? 3f : 0f;
                    npc.localAI[0] = 0f;

                    // Velocity and rotation
                    float chargeVelocity = (leviAlive && !phase4) ? 21f : 26f;
					chargeVelocity += 8f * enrageScale;

					if (revenge)
						chargeVelocity += 2f + (death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio));

                    npc.velocity = Vector2.Normalize(player.Center - vector) * chargeVelocity;
                    npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X);

                    // Direction
                    int num22 = Math.Sign(player.Center.X - vector.X);
                    if (num22 != 0)
                    {
                        npc.direction = num22;

                        if (npc.spriteDirection == 1)
                            npc.rotation += MathHelper.Pi;

                        npc.spriteDirection = -npc.direction;
                    }
                }
            }

            // Charge
            else if (npc.ai[0] == 4f)
            {
				npc.Calamity().canBreakPlayerDefense = true;
				npc.damage = (int)(npc.defDamage * 1.5);

				// Spawn dust
				int num24 = 7;
                for (int j = 0; j < num24; j++)
                {
                    Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f).RotatedBy((j - (num24 / 2 - 1)) * MathHelper.Pi / num24) + vector;
                    Vector2 vector4 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                    int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                    Main.dust[num25].noGravity = true;
                    Main.dust[num25].noLight = true;
                    Main.dust[num25].velocity /= 4f;
                    Main.dust[num25].velocity -= npc.velocity;
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= ((leviAlive && !phase4) ? 50f : 35f))
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.ai[2] += 1f;
                    npc.netUpdate = true;
                }
            }
        }

        // Rotation when charging
        private void ChargeRotation(Player player, Vector2 vector)
        {
            float num17 = (float)Math.Atan2(player.Center.Y - vector.Y, player.Center.X - vector.X);
            if (npc.spriteDirection == 1)
                num17 += MathHelper.Pi;
            if (num17 < 0f)
                num17 += MathHelper.TwoPi;
            if (num17 > MathHelper.TwoPi)
                num17 -= MathHelper.TwoPi;

            float num18 = 0.04f;
            if (npc.ai[0] == 4f)
                num18 = 0f;

			if (num18 != 0f)
				npc.rotation = npc.rotation.AngleTowards(num17, num18);
		}

        // Move to charge location
        private void ChargeLocation(Player player, Vector2 vector, bool leviAlive, bool revenge)
        {
            float distance = leviAlive ? 600f : 500f;

            // Velocity
            if (npc.localAI[0] == 0f)
                npc.localAI[0] = (int)distance * Math.Sign((vector - player.Center).X);

            Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.localAI[0], -distance) - vector - npc.velocity) * 12f;
            float acceleration = revenge ? 0.75f : 0.5f;
			npc.SimpleFlyMovement(vector3, acceleration);

			// Rotation
			int num22 = Math.Sign(player.Center.X - vector.X);
            if (num22 != 0)
            {
                if (npc.ai[1] == 0f && num22 != npc.direction)
                    npc.rotation += MathHelper.Pi;

                npc.direction = num22;

                if (npc.spriteDirection != -npc.direction)
                    npc.rotation += MathHelper.Pi;

                npc.spriteDirection = -npc.direction;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, 187, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, 187, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
			switch (frameUsed)
			{
				case 0:
					texture = Main.npcTexture[npc.type];
					break;
				case 1:
					texture = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/SirenStabbing");
					break;
			}

			bool charging = npc.ai[0] > 2f || forceChargeFrames;
            int height = texture.Height / Main.npcFrameCount[npc.type];
            int width = texture.Width;
			SpriteEffects spriteEffects = charging ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			if (npc.spriteDirection == -1)
				spriteEffects = charging ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0f, npc.gfxOffY), npc.frame, npc.GetAlpha(drawColor), npc.rotation, new Vector2((float)width / 2f, (float)height / 2f), npc.scale, spriteEffects, 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (npc.ai[0] > 2f || forceChargeFrames)
                frameUsed = 1;
            else
                frameUsed = 0;

			int timeBetweenFrames = 8;
            npc.frameCounter++;
			if (npc.frameCounter > timeBetweenFrames * Main.npcFrameCount[npc.type])
				npc.frameCounter = 0;

			npc.frame.Y = frameHeight * (int)(npc.frameCounter / timeBetweenFrames);
			if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
				npc.frame.Y = 0;

			//100x1140
			//200x636
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        // Anahita runs the same loot code as the Leviathan, but only if she dies last.
        public override void NPCLoot()
        {
			//Trophy dropped regardless of Levi, precedent of Twins
            DropHelper.DropItemChance(npc, ModContent.ItemType<AnahitaTrophy>(), 10);

            if (!NPC.AnyNPCs(ModContent.NPCType<Leviathan>()))
                Leviathan.DropSirenLeviLoot(npc);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }
    }
}
