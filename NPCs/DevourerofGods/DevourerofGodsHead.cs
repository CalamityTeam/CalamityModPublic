using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Potions;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [AutoloadBossHead]
    public class DevourerofGodsHead : ModNPC
    {
		private enum LaserWallType
		{
			DiagonalRight = 0,
			DiagonalLeft = 1,
			DiagonalHorizontal = 2,
			DiagonalCross = 3
		}

		private const int shotSpacingMax = 750;
		private int shotSpacing = shotSpacingMax;
		private const int spacingVar = 250;
		private const int totalShots = 6;
		private int laserWallType = 0;
		private const float laserWallSpacingOffset = 16f;

		private bool tail = false;
        private const int minLength = 80;
        private const int maxLength = 81;
        private bool halfLife = false;
        private bool halfLife2 = false;
        private int spawnDoGCountdown = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 104;
            npc.height = 104;
            npc.defense = 50;
			npc.LifeMaxNERB(371250, 445500);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.takenDamageMultiplier = 1.1f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(0, 40, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
			npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
            Mod calamityModMusic = CalamityMod.Instance.musicMod;
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(halfLife);
            writer.Write(halfLife2);
            writer.Write(spawnDoGCountdown);
			writer.Write(shotSpacing);
			writer.Write(laserWallType);
            for (int i = 0; i < 3; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            halfLife = reader.ReadBoolean();
            halfLife2 = reader.ReadBoolean();
            spawnDoGCountdown = reader.ReadInt32();
			shotSpacing = reader.ReadInt32();
			laserWallType = reader.ReadInt32();
            for (int i = 0; i < 3; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable
            CalamityGlobalNPC.DoGHead = npc.whoAmI;

            // Stop rain
            CalamityMod.StopRain();

            // Variables
            Vector2 vector = npc.Center;
            bool flies = npc.ai[2] == 0f;
			bool malice = CalamityWorld.malice;
			bool expertMode = Main.expertMode || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool death = CalamityWorld.death || malice;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			bool phase2 = lifeRatio < 0.75f;
			bool phase3 = lifeRatio < 0.2f;

            // Light
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            // Worm variable
            if (npc.ai[3] > 0f)
                npc.realLife = (int)npc.ai[3];

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			float distanceFromTarget = Vector2.Distance(player.Center, vector);
			bool increaseSpeed = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance200Tiles;
			bool increaseSpeedMore = distanceFromTarget > CalamityGlobalNPC.CatchUpDistance350Tiles;

			float takeLessDamageDistance = 1600f;
			if (distanceFromTarget > takeLessDamageDistance)
			{
				float damageTakenScalar = MathHelper.Clamp(1f - ((distanceFromTarget - takeLessDamageDistance) / takeLessDamageDistance), 0f, 1f);
				npc.takenDamageMultiplier = MathHelper.Lerp(1f, 1.1f, damageTakenScalar);
			}
			else
				npc.takenDamageMultiplier = 1.1f;

			// Spawn Guardians
			if (phase3)
            {
                if (!halfLife)
                {
                    if (revenge)
                        spawnDoGCountdown = 10;

                    string key = "Mods.CalamityMod.EdgyBossText";
                    Color messageColor = Color.Cyan;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);

					npc.TargetClosest();
                    halfLife = true;
                }
                if (spawnDoGCountdown > 0)
                {
                    spawnDoGCountdown--;
                    if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

						for (int i = 0; i < 2; i++)
                            NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
                    }
                }
            }
            else if (phase2)
            {
                if (!halfLife2)
                {
                    if (revenge)
                        spawnDoGCountdown = 10;

                    halfLife2 = true;
                }
                if (spawnDoGCountdown > 0)
                {
                    spawnDoGCountdown--;

					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/DevourerAttack"), (int)player.position.X, (int)player.position.Y);

					if (spawnDoGCountdown == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.SpawnOnPlayer(npc.FindClosestPlayer(), ModContent.NPCType<DevourerofGodsHead2>());
                }
            }

            // Spawn dust
            if (npc.alpha != 0)
            {
                for (int spawnDust = 0; spawnDust < 2; spawnDust++)
                {
                    int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }

            // Alpha
            npc.alpha -= 12;
            if (npc.alpha < 0)
                npc.alpha = 0;

            // Spawn segments
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsBody>(), npc.whoAmI);
                        else
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsTail>(), npc.whoAmI);

                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[2] = npc.whoAmI;
                        Main.npc[segment].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = segment;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segment, 0f, 0f, 0f, 0);
                        Previous = segment;
                    }
                    tail = true;
                }

				if (phase2)
				{
					float speed = 12f;
					float spawnOffset = 1500f;
					float divisor = malice ? 240f : death ? 360f : 480f;

					if (calamityGlobalNPC.newAI[1] % divisor == 0f)
					{
						Main.PlaySound(SoundID.Item12, player.position);

						// Side walls
						int type = ModContent.ProjectileType<DoGDeath>();
						int damage = npc.GetProjectileDamage(type);
						Vector2 start = default;
						Vector2 velocity = default;
						Vector2 aim = expertMode ? player.Center + player.velocity * 20f : Vector2.Zero;
						Vector2 aimClone = aim;

						switch (laserWallType)
						{
							case (int)LaserWallType.DiagonalRight:

								for (int x = 0; x < totalShots + 1; x++)
								{
									start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
									aim.Y += laserWallSpacingOffset * (x - 3);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									shotSpacing -= spacingVar;
								}

								if (expertMode)
									Projectile.NewProjectile(player.position.X + spawnOffset, player.position.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);

								laserWallType = (int)LaserWallType.DiagonalLeft;
								break;

							case (int)LaserWallType.DiagonalLeft:

								for (int x = 0; x < totalShots + 1; x++)
								{
									start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
									aim.Y += laserWallSpacingOffset * (x - 3);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									shotSpacing -= spacingVar;
								}

								if (expertMode)
									Projectile.NewProjectile(player.position.X - spawnOffset, player.position.Y, speed, 0f, type, damage, 0f, Main.myPlayer);

								laserWallType = expertMode ? (int)LaserWallType.DiagonalHorizontal : (int)LaserWallType.DiagonalRight;
								break;

							case (int)LaserWallType.DiagonalHorizontal:

								for (int x = 0; x < totalShots + 1; x++)
								{
									start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
									aim.Y += laserWallSpacingOffset * (x - 3);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									shotSpacing -= spacingVar;
								}

								if (expertMode)
								{
									Projectile.NewProjectile(player.position.X + spawnOffset, player.position.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
									Projectile.NewProjectile(player.position.X - spawnOffset, player.position.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
								}

								laserWallType = revenge ? (int)LaserWallType.DiagonalCross : (int)LaserWallType.DiagonalRight;
								break;

							case (int)LaserWallType.DiagonalCross:

								for (int x = 0; x < totalShots + 1; x++)
								{
									start = new Vector2(player.position.X + spawnOffset, player.position.Y + shotSpacing);
									aim.Y += laserWallSpacingOffset * (x - 3);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									start = new Vector2(player.position.X - spawnOffset, player.position.Y + shotSpacing);
									velocity = Vector2.Normalize(aim - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									start = new Vector2(player.position.X + shotSpacing, player.position.Y + spawnOffset);
									aimClone.X += laserWallSpacingOffset * (x - 3);
									velocity = Vector2.Normalize(aimClone - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									start = new Vector2(player.position.X + shotSpacing, player.position.Y - spawnOffset);
									velocity = Vector2.Normalize(aimClone - start) * speed;
									Projectile.NewProjectile(start, velocity, type, damage, 0f, Main.myPlayer);

									shotSpacing -= spacingVar;
								}

								if (expertMode)
								{
									Projectile.NewProjectile(player.position.X + spawnOffset, player.position.Y, -speed, 0f, type, damage, 0f, Main.myPlayer);
									Projectile.NewProjectile(player.position.X - spawnOffset, player.position.Y, speed, 0f, type, damage, 0f, Main.myPlayer);
								}

								laserWallType = (int)LaserWallType.DiagonalRight;
								break;
						}
						shotSpacing = shotSpacingMax;
					}

					calamityGlobalNPC.newAI[1] += 1f;
				}
				else
					calamityGlobalNPC.newAI[1] = 0f;
			}

            // Despawn
            if (player.dead)
            {
				npc.TargetClosest(false);
				flies = true;

                npc.velocity.Y -= 3f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                    npc.velocity.Y -= 3f;

                int bodyType = ModContent.NPCType<DevourerofGodsBody>();
                int tailType = ModContent.NPCType<DevourerofGodsTail>();
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type != npc.type && Main.npc[a].type != bodyType && Main.npc[a].type != tailType)
                            continue;

                        Main.npc[a].active = false;
                        Main.npc[a].netUpdate = true;
                    }
                }
            }

            // Movement
            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + npc.height) / 16f) + 2;

            if (num180 < 0)
                num180 = 0;
            if (num181 > Main.maxTilesX)
                num181 = Main.maxTilesX;
            if (num182 < 0)
                num182 = 0;
            if (num183 > Main.maxTilesY)
                num183 = Main.maxTilesY;

            if (npc.velocity.X < 0f)
                npc.spriteDirection = -1;
            else if (npc.velocity.X > 0f)
                npc.spriteDirection = 1;

            // Flight
            if (npc.ai[2] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
                }

                // Flying movement
                npc.localAI[1] = 0f;

				float phaseChangeRate = 1f + (expertMode ? 9f * (1f - lifeRatio) : 0f);
				calamityGlobalNPC.newAI[2] += phaseChangeRate;

				float speed = malice ? 18f : death ? 16.5f : 15f;
				float turnSpeed = malice ? 0.36f : death ? 0.33f : 0.3f;
				float homingSpeed = malice ? 27f : death ? 22.5f : 18f;
				float homingTurnSpeed = malice ? 0.48f : death ? 0.405f : 0.33f;

				if (expertMode)
				{
					speed += 3f * (1f - lifeRatio);
					turnSpeed += 0.06f * (1f - lifeRatio);
					homingSpeed += 9f * (1f - lifeRatio);
					homingTurnSpeed += 0.15f * (1f - lifeRatio);
				}

				// Go to ground phase sooner
				if (increaseSpeedMore)
					calamityGlobalNPC.newAI[2] += 10f;
				else if (increaseSpeed)
					calamityGlobalNPC.newAI[2] += 2f;

				float num188 = speed;
                float num189 = turnSpeed;
                Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num191 = player.position.X + (player.width / 2);
                float num192 = player.position.Y + (player.height / 2);
                int num42 = -1;
                int num43 = (int)(player.Center.X / 16f);
                int num44 = (int)(player.Center.Y / 16f);

                for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
                {
                    for (int num46 = num44; num46 <= num44 + 15; num46++)
                    {
                        if (WorldGen.SolidTile2(num45, num46))
                        {
                            num42 = num46;
                            break;
                        }
                    }
                    if (num42 > 0)
                        break;
                }

                if (num42 > 0)
                {
                    num42 *= 16;
                    float num47 = num42 - 800;
                    if (player.position.Y > num47)
                    {
                        num192 = num47;
                        if (Math.Abs(npc.Center.X - player.Center.X) < 500f)
                        {
                            if (npc.velocity.X > 0f)
                                num191 = player.Center.X + 600f;
                            else
                                num191 = player.Center.X - 600f;
                        }
                    }
                }
                else
                {
                    num188 = homingSpeed;
                    num189 = homingTurnSpeed;
                }

				if (expertMode)
				{
					num188 += distanceFromTarget * 0.005f * (1f - lifeRatio);
					num189 += distanceFromTarget * 0.0001f * (1f - lifeRatio);
				}

                float num48 = num188 * 1.3f;
                float num49 = num188 * 0.7f;
                float num50 = npc.velocity.Length();
                if (num50 > 0f)
                {
                    if (num50 > num48)
                    {
                        npc.velocity.Normalize();
                        npc.velocity *= num48;
                    }
                    else if (num50 < num49)
                    {
                        npc.velocity.Normalize();
                        npc.velocity *= num49;
                    }
                }

                num191 = (int)(num191 / 16f) * 16;
                num192 = (int)(num192 / 16f) * 16;
                vector18.X = (int)(vector18.X / 16f) * 16;
                vector18.Y = (int)(vector18.Y / 16f) * 16;
                num191 -= vector18.X;
                num192 -= vector18.Y;
                float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
                float num196 = Math.Abs(num191);
                float num197 = Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;

                if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                {
                    if (npc.velocity.X < num191)
                        npc.velocity.X += num189;
                    else
                    {
                        if (npc.velocity.X > num191)
                            npc.velocity.X -= num189;
                    }

                    if (npc.velocity.Y < num192)
                        npc.velocity.Y += num189;
                    else
                    {
                        if (npc.velocity.Y > num192)
                            npc.velocity.Y -= num189;
                    }

                    if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y += num189 * 2f;
                        else
                            npc.velocity.Y -= num189 * 2f;
                    }

                    if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X += num189 * 2f;
                        else
                            npc.velocity.X -= num189 * 2f;
                    }
                }
                else
                {
                    if (num196 > num197)
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X += num189 * 1.1f;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X -= num189 * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num189;
                            else
                                npc.velocity.Y -= num189;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                            npc.velocity.Y += num189 * 1.1f;
                        else if (npc.velocity.Y > num192)
                            npc.velocity.Y -= num189 * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num189;
                            else
                                npc.velocity.X -= num189;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (calamityGlobalNPC.newAI[2] > 900f)
                {
                    npc.ai[2] = 1f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }

            // Ground
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
                }

				calamityGlobalNPC.newAI[2] += 1f;

                float fallSpeed = malice ? 19.5f : death ? 17.75f : 16f;
                float speed = malice ? 0.26f : death ? 0.22f : 0.18f;
                float turnSpeed = malice ? 0.24f : death ? 0.18f : 0.12f;

				if (expertMode)
				{
					fallSpeed += 3.5f * (1f - lifeRatio);
					speed += 0.08f * (1f - lifeRatio);
					turnSpeed += 0.12f * (1f - lifeRatio);
					speed += distanceFromTarget * 0.00005f * (1f - lifeRatio);
					turnSpeed += distanceFromTarget * 0.00005f * (1f - lifeRatio);
				}

                // Enrage
                if (increaseSpeedMore)
                {
					fallSpeed *= 3f;
                    speed *= 4f;
                    turnSpeed *= 6f;
                }
                else if (increaseSpeed)
                {
					fallSpeed *= 1.5f;
					speed *= 2f;
                    turnSpeed *= 3f;
                }

                if (!flies)
                {
                    for (int num952 = num180; num952 < num181; num952++)
                    {
                        for (int num953 = num182; num953 < num183; num953++)
                        {
                            if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[Main.tile[num952, num953].type] || (Main.tileSolidTop[Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
                            {
                                Vector2 vector105;
                                vector105.X = num952 * 16;
                                vector105.Y = num953 * 16;
                                if (npc.position.X + npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
                                {
                                    flies = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (!flies)
                {
                    npc.localAI[1] = 1f;
                    Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);

					int num954 = death ? 1100 : 1175;
					if (expertMode)
						num954 -= (int)(150f * (1f - lifeRatio));
					if (num954 < 1025)
						num954 = 1025;

					bool flag95 = true;
                    if (npc.position.Y > player.position.Y)
                    {
                        for (int num955 = 0; num955 < 255; num955++)
                        {
                            if (Main.player[num955].active)
                            {
                                Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
                                if (rectangle12.Intersects(rectangle13))
                                {
                                    flag95 = false;
                                    break;
                                }
                            }
                        }
                        if (flag95)
                            flies = true;
                    }
                }
                else
                    npc.localAI[1] = 0f;

                Vector2 vector3 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num20 = player.position.X + (player.width / 2);
                float num21 = player.position.Y + (player.height / 2);
                num20 = (int)(num20 / 16f) * 16;
                num21 = (int)(num21 / 16f) * 16;
                vector3.X = (int)(vector3.X / 16f) * 16;
                vector3.Y = (int)(vector3.Y / 16f) * 16;
                num20 -= vector3.X;
                num21 -= vector3.Y;
                float num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);

                if (!flies)
                {
                    npc.velocity.Y += turnSpeed;
                    if (npc.velocity.Y > fallSpeed)
                        npc.velocity.Y = fallSpeed;

                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * 1.8)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= speed * 1.1f;
                        else
                            npc.velocity.X += speed * 1.1f;
                    }
                    else if (npc.velocity.Y == fallSpeed)
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += speed;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= speed;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X += speed * 0.9f;
                        else
                            npc.velocity.X -= speed * 0.9f;
                    }
                }
                else
                {

					double maximumSpeed1 = malice ? 0.48 : death ? 0.44 : 0.4;
					double maximumSpeed2 = malice ? 1.16 : death ? 1.08 : 1D;

					if (increaseSpeedMore)
					{
						maximumSpeed1 *= 4;
						maximumSpeed2 *= 4;
					}
					if (increaseSpeed)
					{
						maximumSpeed1 *= 2;
						maximumSpeed2 *= 2;
					}

					if (expertMode)
					{
						maximumSpeed1 += 0.08f * (1f - lifeRatio);
						maximumSpeed2 += 0.16f * (1f - lifeRatio);
					}

                    num22 = (float)Math.Sqrt(num20 * num20 + num21 * num21);
                    float num25 = Math.Abs(num20);
                    float num26 = Math.Abs(num21);
                    float num27 = fallSpeed / num22;
                    num20 *= num27;
                    num21 *= num27;

                    if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += turnSpeed * 1.3f;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= turnSpeed * 1.3f;

                        if (npc.velocity.Y < num21)
                            npc.velocity.Y += turnSpeed * 1.3f;
                        else if (npc.velocity.Y > num21)
                            npc.velocity.Y -= turnSpeed * 1.3f;
                    }

                    if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += speed;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= speed;
                        if (npc.velocity.Y < num21)
                            npc.velocity.Y += speed;
                        else if (npc.velocity.Y > num21)
                            npc.velocity.Y -= speed;

                        if (Math.Abs(num21) < fallSpeed * maximumSpeed1 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += speed * 2f;
                            else
                                npc.velocity.Y -= speed * 2f;
                        }
                        if (Math.Abs(num20) < fallSpeed * maximumSpeed1 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += speed * 2f;
                            else
                                npc.velocity.X -= speed * 2f;
                        }
                    }
                    else if (num25 > num26)
                    {
                        if (npc.velocity.X < num20)
                            npc.velocity.X += speed * 1.1f;
                        else if (npc.velocity.X > num20)
                            npc.velocity.X -= speed * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += speed;
                            else
                                npc.velocity.Y -= speed;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num21)
                            npc.velocity.Y += speed * 1.1f;
                        else if (npc.velocity.Y > num21)
                            npc.velocity.Y -= speed * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < fallSpeed * maximumSpeed2)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += speed;
                            else
                                npc.velocity.X -= speed;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (flies)
                {
                    if (npc.localAI[0] != 1f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 0f;
                }

                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                    npc.netUpdate = true;

                if (calamityGlobalNPC.newAI[2] > 900f)
                {
                    npc.ai[2] = 0f;
					calamityGlobalNPC.newAI[2] = 0f;
					npc.TargetClosest();
                    npc.netUpdate = true;
                }
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow");
			Color color37 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/DevourerofGods/DevourerofGodsHeadGlow2");
			color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        // DoG phase 1 does not drop loot, but starts the sentinel phase of the fight.
        public override void NPCLoot()
        {
            // Skip the sentinel phase entirely if DoG has already been killed
            CalamityWorld.DoGSecondStageCountdown = (CalamityWorld.downedDoG || CalamityWorld.downedSecondSentinels) ? 600 : 21600;

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                netMessage.Send();
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (CalamityUtils.AntiButcher(npc, ref damage, 0.5f))
            {
                string key = "Mods.CalamityMod.EdgyBossText2";
                Color messageColor = Color.Cyan;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                return false;
            }
            return true;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGHead"), 1f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 50;
                npc.height = 50;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 15; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
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
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300, true);
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
            player.AddBuff(BuffID.Frostburn, 300, true);
            /*if (CalamityWorld.death && !player.Calamity().lol)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s essence was consumed by the devourer."), 1000.0, 0, false);
            }*/

			if (player.Calamity().dogTextCooldown <= 0)
			{
				string text = Utils.SelectRandom(Main.rand, new string[]
				{
					"Mods.CalamityMod.EdgyBossText3",
					"Mods.CalamityMod.EdgyBossText4",
					"Mods.CalamityMod.EdgyBossText5",
					"Mods.CalamityMod.EdgyBossText6",
					"Mods.CalamityMod.EdgyBossText7"
				});
				Color messageColor = Color.Cyan;
				Rectangle location = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				CombatText.NewText(location, messageColor, Language.GetTextValue(text), true);
				player.Calamity().dogTextCooldown = 60;
			}
        }
    }
}
