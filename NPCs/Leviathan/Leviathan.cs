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
    public class Leviathan : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private int counter = 0;
        private bool initialised = false;
		private int soundDelay = 0;
        public static Texture2D AttackTexture = null;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Leviathan");
            Main.npcFrameCount[npc.type] = 3;
            if (!Main.dedServ)
                AttackTexture = ModContent.GetTexture("CalamityMod/NPCs/Leviathan/LeviathanAttack");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 20f;
			npc.GetNPCDamage();
			npc.width = 900;
            npc.height = 450;
            npc.defense = 40;
			npc.DR_NERD(0.35f);
            npc.LifeMaxNERB(60500, 72560, 600000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
			npc.value = Item.buyPrice(0, 15, 0, 0);
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.boss = true;
            npc.netAlways = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("LeviathanAndAnahita") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<LeviathanBag>();
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToSickness = true;
			npc.Calamity().VulnerableToElectricity = true;
			npc.Calamity().VulnerableToWater = false;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(biomeEnrageTimer);
			writer.Write(npc.dontTakeDamage);
            writer.Write(soundDelay);
            writer.Write(npc.Calamity().newAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			biomeEnrageTimer = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
            soundDelay = reader.ReadInt32();
            npc.Calamity().newAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            CalamityGlobalNPC.leviathan = npc.whoAmI;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            Vector2 vector = npc.Center;

			// Is in spawning animation
			float spawnAnimationTime = 180f;
			bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

			// Don't do damage during spawn animation
			npc.damage = npc.defDamage;
			if (spawnAnimation)
				npc.damage = 0;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Phases
			bool phase2 = (lifeRatio < 0.7f || death) && expertMode;
			bool phase3 = lifeRatio < (death ? 0.7f : 0.4f);
			bool phase4 = lifeRatio < (death ? 0.4f : 0.2f);

            bool sirenAlive = false;
            if (CalamityGlobalNPC.siren != -1)
                sirenAlive = Main.npc[CalamityGlobalNPC.siren].active;

			if (CalamityGlobalNPC.siren != -1)
			{
				if (Main.npc[CalamityGlobalNPC.siren].active)
				{
					if (Main.npc[CalamityGlobalNPC.siren].damage == 0)
						sirenAlive = false;
				}
			}

			int soundChoiceRage = 92;
			int soundChoice = Utils.SelectRandom(Main.rand, new int[]
			{
				38,
				39,
				40
			});

			if (soundDelay > 0)
				soundDelay--;

            if (Main.rand.NextBool(600) && !spawnAnimation)
                Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, (sirenAlive && !death) ? soundChoice : soundChoiceRage);

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

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

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
            if (biomeEnraged)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 2f;
            }

			npc.dontTakeDamage = spawnAnimation;

			// Adjust slowing debuff immunity
			bool immuneToSlowingDebuffs = npc.ai[0] == 2f;
			npc.buffImmune[ModContent.BuffType<ExoFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<GlacialState>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<KamiDebuff>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Eutrophication>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TimeSlow>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<TeslaFreeze>()] = immuneToSlowingDebuffs;
			npc.buffImmune[ModContent.BuffType<Vaporfied>()] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Slow] = immuneToSlowingDebuffs;
			npc.buffImmune[BuffID.Webbed] = immuneToSlowingDebuffs;

			if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
					if (npc.velocity.Y < -3f)
						npc.velocity.Y = -3f;
					npc.velocity.Y += 0.2f;
					if (npc.velocity.Y > 16f)
						npc.velocity.Y = 16f;

					if (npc.position.Y > Main.worldSurface * 16.0)
                    {
                        for (int x = 0; x < Main.maxNPCs; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<Siren>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        npc.active = false;
                        npc.netUpdate = true;
                    }

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.netUpdate = true;
					}

					return;
                }
            }
            else
            {
				// Slowly drift up when spawning
				if (spawnAnimation)
				{
					float minSpawnVelocity = 0.4f;
					float maxSpawnVelocity = 4f;
					float velocityY = maxSpawnVelocity - MathHelper.Lerp(minSpawnVelocity, maxSpawnVelocity, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
					npc.velocity = new Vector2(0f, -velocityY);

					if (calamityGlobalNPC.newAI[3] == 10f)
						Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, soundChoiceRage);

					npc.Opacity = MathHelper.Clamp(calamityGlobalNPC.newAI[3] / spawnAnimationTime, 0f, 1f);

					calamityGlobalNPC.newAI[3] += 1f;

					return;
				}

				if (npc.ai[0] == 0f)
                {
                    float num412 = (sirenAlive && !phase4) ? 3.5f : 7f;
                    float num413 = (sirenAlive && !phase4) ? 0.1f : 0.2f;
					num412 += 2f * enrageScale;
					num413 += 0.05f * enrageScale;

					if (expertMode && (!sirenAlive || phase4))
					{
						num412 += death ? 6f * (1f - lifeRatio) : 3.5f * (1f - lifeRatio);
						num413 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
					}

                    int num414 = 1;
                    if (vector.X < player.position.X + player.width)
                        num414 = -1;

                    Vector2 vector40 = vector;
                    float num415 = player.Center.X + (num414 * ((sirenAlive && !phase4) ? 1000f : 800f)) - vector40.X;
                    float num416 = player.Center.Y - vector40.Y;
                    float num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
                    num417 = num412 / num417;
                    num415 *= num417;
                    num416 *= num417;

                    if (npc.velocity.X < num415)
                    {
                        npc.velocity.X += num413;
                        if (npc.velocity.X < 0f && num415 > 0f)
                            npc.velocity.X += num413;
                    }
                    else if (npc.velocity.X > num415)
                    {
                        npc.velocity.X -= num413;
                        if (npc.velocity.X > 0f && num415 < 0f)
                            npc.velocity.X -= num413;
                    }

                    if (npc.velocity.Y < num416)
                    {
                        npc.velocity.Y += num413;
                        if (npc.velocity.Y < 0f && num416 > 0f)
                            npc.velocity.Y += num413;
                    }
                    else if (npc.velocity.Y > num416)
                    {
                        npc.velocity.Y -= num413;
                        if (npc.velocity.Y > 0f && num416 < 0f)
                            npc.velocity.Y -= num413;
                    }

					float playerLocation = vector.X - player.Center.X;
					npc.direction = playerLocation < 0 ? 1 : -1;
					npc.spriteDirection = npc.direction;

					npc.ai[1] += 1f;
					float phaseTimer = 240f;
					if (!sirenAlive || phase4)
						phaseTimer -= 120f * (1f - lifeRatio);

					if (npc.ai[1] >= phaseTimer)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
						npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!player.dead)
                        {
                            npc.ai[2] += 1f;
                            if (!sirenAlive || phase4)
                                npc.ai[2] += 2f;
                        }

                        if (npc.ai[2] >= 75f)
                        {
                            npc.ai[2] = 0f;
                            vector40 = new Vector2(vector.X, vector.Y + 20f);
                            num415 = vector40.X + 1000f * npc.direction - vector40.X;
                            num416 = player.Center.Y - vector40.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float speed = (sirenAlive && !phase4 && !death) ? 13.5f : 16f;
                                int type = ModContent.ProjectileType<LeviathanBomb>();
								int damage = npc.GetProjectileDamage(type);

								if (expertMode)
                                    speed = (sirenAlive && !phase4 && !death) ? 14f : 17f;

								speed += 2f * enrageScale;

								if (!sirenAlive || phase4)
									speed += 3f * (1f - lifeRatio);

                                num417 = (float)Math.Sqrt(num415 * num415 + num416 * num416);
                                num417 = speed / num417;
                                num415 *= num417;
                                num416 *= num417;
                                vector40.X += num415 * 4f;
                                vector40.Y += num416 * 4f;
                                Projectile.NewProjectile(vector40.X, vector40.Y, num415, num416, type, damage, 0f, Main.myPlayer);
								if (soundDelay <= 0)
								{
									soundDelay = 120;
									Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LeviathanRoarMeteor"), vector);
								}
                            }
                        }
                    }
                }
                else if (npc.ai[0] == 1f)
                {
                    Vector2 vector119 = new Vector2(vector.X, npc.position.Y + npc.height * 0.8f);
                    Vector2 vector120 = vector;
                    float num1058 = player.Center.X - vector120.X;
                    float num1059 = player.Center.Y - vector120.Y;
                    float num1060 = (float)Math.Sqrt(num1058 * num1058 + num1059 * num1059);

                    npc.ai[1] += 1f;
					int num638 = 0;
					for (int num639 = 0; num639 < Main.maxPlayers; num639++)
					{
						if (Main.player[num639].active && !Main.player[num639].dead && (vector - Main.player[num639].Center).Length() < 1000f)
							num638++;
					}
					npc.ai[1] += num638 / 2;

                    bool flag103 = false;
					float num640 = (!sirenAlive || phase4) ? 30f : 60f;
					if (npc.ai[1] > num640)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] += 1f;
                        flag103 = true;
                    }

					int spawnLimit = (sirenAlive && !phase4) ? 2 : 4;
					if (flag103 && NPC.CountNPCS(ModContent.NPCType<AquaticAberration>()) < spawnLimit)
                    {
                        Main.PlaySound(SoundID.Zombie, (int)vector.X, (int)vector.Y, soundChoice);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
							NPC.NewNPC((int)vector119.X, (int)vector119.Y, ModContent.NPCType<AquaticAberration>());
                    }

                    if (num1060 > ((sirenAlive && !phase4) ? 1000f : 800f))
                    {
                        float num1064 = (sirenAlive && !phase4) ? 0.05f : 0.065f;
						num1064 += 0.04f * enrageScale;

						if (expertMode && (!sirenAlive || phase4))
							num1064 += death ? 0.05f * (1f - lifeRatio) : 0.03f * (1f - lifeRatio);

                        vector120 = vector119;
                        num1058 = player.Center.X - vector120.X;
                        num1059 = player.Center.Y - vector120.Y;

                        if (npc.velocity.X < num1058)
                        {
                            npc.velocity.X += num1064;
                            if (npc.velocity.X < 0f && num1058 > 0f)
                                npc.velocity.X += num1064;
                        }
                        else if (npc.velocity.X > num1058)
                        {
                            npc.velocity.X -= num1064;
                            if (npc.velocity.X > 0f && num1058 < 0f)
                                npc.velocity.X -= num1064;
                        }
                        if (npc.velocity.Y < num1059)
                        {
                            npc.velocity.Y += num1064;
                            if (npc.velocity.Y < 0f && num1059 > 0f)
                                npc.velocity.Y += num1064;
                        }
                        else if (npc.velocity.Y > num1059)
                        {
                            npc.velocity.Y -= num1064;
                            if (npc.velocity.Y > 0f && num1059 < 0f)
                                npc.velocity.Y -= num1064;
                        }
                    }
                    else
                        npc.velocity *= 0.9f;

					float playerLocation = vector.X - player.Center.X;
					npc.direction = playerLocation < 0 ? 1 : -1;
					npc.spriteDirection = npc.direction;

					if (npc.ai[2] > ((sirenAlive && !phase4) ? 2f : 4f))
                    {
                        npc.ai[0] = (((phase2 || phase3) && !sirenAlive) || phase4 || death) ? 2f : 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
						npc.TargetClosest();
                        npc.netUpdate = true;
                    }
                }
                else if (npc.ai[0] == 2f)
                {
                    Vector2 distFromPlayer = player.Center - vector;
					float chargeAmt = death ? (phase4 ? 3f : phase3 ? 2f : 1f) : 1f;
                    if (npc.ai[1] >= chargeAmt * 2f || distFromPlayer.Length() > 2400f)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
						npc.TargetClosest();
                        npc.netUpdate = true;
                        return;
                    }

					float chargeDistance = (sirenAlive && !phase4) ? 1100f : 900f;
					chargeDistance -= 50f * enrageScale;
					if (!sirenAlive || phase4)
						chargeDistance -= 250f * (1f - lifeRatio);

					if (npc.ai[1] % 2f == 0f)
                    {
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

                        if (Math.Abs(npc.position.Y + (npc.height / 2) - (player.position.Y + (player.height / 2))) < 20f)
                        {
                            npc.ai[1] += 1f;
                            npc.ai[2] = 0f;

                            float num1044 = revenge ? 20f : 18f;
							num1044 += 2f * enrageScale;

							if (revenge && (!sirenAlive || phase4))
								num1044 += death ? 9f * (1f - lifeRatio) : 6f * (1f - lifeRatio);

                            Vector2 vector117 = vector;
                            float num1045 = player.Center.X - vector117.X;
                            float num1046 = player.Center.Y - vector117.Y;
                            float num1047 = (float)Math.Sqrt(num1045 * num1045 + num1046 * num1046);
                            num1047 = num1044 / num1047;
                            npc.velocity.X = num1045 * num1047;
                            npc.velocity.Y = num1046 * num1047;

							float playerLocation = vector.X - player.Center.X;
							npc.direction = playerLocation < 0 ? 1 : -1;
							npc.spriteDirection = npc.direction;

							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LeviathanRoarCharge"), vector);

                            return;
                        }

                        float num1048 = revenge ? 7.5f : 6.5f;
                        float num1049 = revenge ? 0.12f : 0.11f;
						num1048 += 2f * enrageScale;
						num1049 += 0.04f * enrageScale;

						if (revenge && (!sirenAlive || phase4))
						{
							num1048 += death ? 9f * (1f - lifeRatio) : 6f * (1f - lifeRatio);
							num1049 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
						}

                        if (vector.Y < player.Center.Y)
                            npc.velocity.Y += num1049;
                        else
                            npc.velocity.Y -= num1049;

                        if (npc.velocity.Y < -num1048)
                            npc.velocity.Y = -num1048;
                        if (npc.velocity.Y > num1048)
                            npc.velocity.Y = num1048;

                        if (Math.Abs(vector.X - player.Center.X) > chargeDistance + 200f)
                            npc.velocity.X += num1049 * npc.direction;
                        else if (Math.Abs(vector.X - player.Center.X) < chargeDistance)
                            npc.velocity.X -= num1049 * npc.direction;
                        else
                            npc.velocity.X *= 0.8f;

                        if (npc.velocity.X < -num1048)
                            npc.velocity.X = -num1048;
                        if (npc.velocity.X > num1048)
                            npc.velocity.X = num1048;

						float playerLocation2 = vector.X - player.Center.X;
						npc.direction = playerLocation2 < 0 ? 1 : -1;
						npc.spriteDirection = npc.direction;
					}
                    else
                    {
                        if (npc.velocity.X < 0f)
                            npc.direction = -1;
                        else
                            npc.direction = 1;

                        npc.spriteDirection = npc.direction;

                        int num1051 = 1;
                        if (vector.X < player.Center.X)
                            num1051 = -1;
                        if (npc.direction == num1051 && Math.Abs(vector.X - player.Center.X) > chargeDistance)
                            npc.ai[2] = 1f;

                        if (npc.ai[2] != 1f)
                            return;

						float playerLocation = vector.X - player.Center.X;
						npc.direction = playerLocation < 0 ? 1 : -1;
						npc.spriteDirection = npc.direction;

						npc.velocity *= 0.9f;
                        float num1052 = revenge ? 0.11f : 0.1f;
						num1052 += 0.02f * enrageScale;

						if (revenge && (!sirenAlive || phase4))
						{
							npc.velocity *= death ? MathHelper.Lerp(0.75f, 1f, lifeRatio) : MathHelper.Lerp(0.81f, 1f, lifeRatio);
							num1052 += death ? 0.15f * (1f - lifeRatio) : 0.1f * (1f - lifeRatio);
						}

                        if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num1052)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] += 1f;
							npc.TargetClosest();
                        }
                    }
                }
            }
        }

		// Can only hit the target if within certain distance.
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			Vector2 npcCenter = npc.Center;

			// NOTE: Tail and mouth hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the body hitbox.
			// Width = 225, Height = 225
			Rectangle mouthHitbox = new Rectangle((int)(npcCenter.X - (npc.width / 2f)), (int)(npcCenter.Y - (npc.height / 4f)), npc.width / 4, npc.height / 2);
			// Width = 450, Height = 450
			Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (npc.width / 4f)), (int)(npcCenter.Y - (npc.height / 2f)), npc.width / 2, npc.height);
			// Width = 225, Height = 225
			Rectangle tailHitbox = new Rectangle((int)(npcCenter.X + (npc.width / 4f)), (int)(npcCenter.Y - (npc.height / 4f)), npc.width / 4, npc.height / 2);

			Vector2 mouthHitboxCenter = new Vector2(mouthHitbox.X + (mouthHitbox.Width / 2), mouthHitbox.Y + (mouthHitbox.Height / 2));
			Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
			Vector2 tailHitboxCenter = new Vector2(tailHitbox.X + (tailHitbox.Width / 2), tailHitbox.Y + (tailHitbox.Height / 2));

			Rectangle targetHitbox = target.Hitbox;

			float mouthDist1 = Vector2.Distance(mouthHitboxCenter, targetHitbox.TopLeft());
			float mouthDist2 = Vector2.Distance(mouthHitboxCenter, targetHitbox.TopRight());
			float mouthDist3 = Vector2.Distance(mouthHitboxCenter, targetHitbox.BottomLeft());
			float mouthDist4 = Vector2.Distance(mouthHitboxCenter, targetHitbox.BottomRight());

			float minMouthDist = mouthDist1;
			if (mouthDist2 < minMouthDist)
				minMouthDist = mouthDist2;
			if (mouthDist3 < minMouthDist)
				minMouthDist = mouthDist3;
			if (mouthDist4 < minMouthDist)
				minMouthDist = mouthDist4;

			bool insideMouthHitbox = minMouthDist <= 115f;

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

			bool insideBodyHitbox = minBodyDist <= 230f;

			float tailDist1 = Vector2.Distance(tailHitboxCenter, targetHitbox.TopLeft());
			float tailDist2 = Vector2.Distance(tailHitboxCenter, targetHitbox.TopRight());
			float tailDist3 = Vector2.Distance(tailHitboxCenter, targetHitbox.BottomLeft());
			float tailDist4 = Vector2.Distance(tailHitboxCenter, targetHitbox.BottomRight());

			float minTailDist = tailDist1;
			if (tailDist2 < minTailDist)
				minTailDist = tailDist2;
			if (tailDist3 < minTailDist)
				minTailDist = tailDist3;
			if (tailDist4 < minTailDist)
				minTailDist = tailDist4;

			bool insideTailHitbox = minTailDist <= 115f;

			return insideMouthHitbox || insideBodyHitbox || insideTailHitbox;
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                float randomSpread = Main.rand.Next(-200, 200) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/LeviathanGores/LeviGore"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/LeviathanGores/LeviGore2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/LeviathanGores/LeviGore3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread, mod.GetGoreSlot("Gores/LeviathanGores/LeviGore4"), 1f);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        // The Leviathan runs the same loot code as Anahita, but only if she dies last.
        public override void NPCLoot()
        {
			//Trophy dropped regardless of Anahita, precedent of Twins
            DropHelper.DropItemChance(npc, ModContent.ItemType<LeviathanTrophy>(), 10);

            if (!NPC.AnyNPCs(ModContent.NPCType<Siren>()))
                DropSirenLeviLoot(npc);
        }

        // This loot code is shared with Anahita.
        public static void DropSirenLeviLoot(NPC npc)
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeOcean>(), true, !CalamityWorld.downedLeviathan);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeLeviathanandSiren>(), true, !CalamityWorld.downedLeviathan);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<Greentide>(w),
                    DropHelper.WeightStack<Leviatitan>(w),
                    DropHelper.WeightStack<SirensSong>(w),
                    DropHelper.WeightStack<Atlantis>(w),
                    DropHelper.WeightStack<GastricBelcherStaff>(w),
                    DropHelper.WeightStack<BrackishFlask>(w),
                    DropHelper.WeightStack<LeviathanTeeth>(w),
					DropHelper.WeightStack<LureofEnthrallment>(w),
					DropHelper.WeightStack<TheCommunity>(w)
				);

				// Equipment
				DropHelper.DropItem(npc, ModContent.ItemType<LeviathanAmbergris>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<LeviathanMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AnahitaMask>(), 7);

                // Fishing
                DropHelper.DropItemChance(npc, ItemID.HotlineFishingHook, 10);
                DropHelper.DropItemChance(npc, ItemID.BottomlessBucket, 10);
                DropHelper.DropItemChance(npc, ItemID.SuperAbsorbantSponge, 10);
                DropHelper.DropItemChance(npc, ItemID.FishingPotion, 5, 5, 8);
                DropHelper.DropItemChance(npc, ItemID.SonarPotion, 5, 5, 8);
                DropHelper.DropItemChance(npc, ItemID.CratePotion, 5, 5, 8);
            }

            // Mark Siren & Levi as dead
            CalamityWorld.downedLeviathan = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Bleeding, 240, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = AttackTexture;
			if (npc.ai[0] == 1f || npc.Calamity().newAI[3] < 180f)
            {
				texture = Main.npcTexture[npc.type];
            }
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			float xOffset = -50f;
			if (npc.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.None;
				xOffset *= -1f;
			}
			Rectangle rectangle = new Rectangle(npc.frame.X, npc.frame.Y, texture.Width / 2, texture.Height / 3);
			Vector2 origin = rectangle.Size() / 2f;
			spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(xOffset, npc.gfxOffY), rectangle, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, spriteEffects, 0f);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
			int width = 1011;
			int height = 486;

            if (!initialised)
            {
                counter = 3;
                npc.frameCounter = 8D;
                initialised = true;
            }

			npc.frameCounter += 1D;
			if (npc.frameCounter >= 8D)
			{
				npc.frameCounter = 0D;
				counter++;
				npc.frame.X = counter >= 3 ? width + 3 : 0;

				if (counter == 3)
					npc.frame.Y = 0;
				else
					npc.frame.Y += height;
			}

            if (counter == 6)
            {
                counter = 1;
                npc.frame.Y = 0;
                npc.frame.X = 0;
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }
    }
}
