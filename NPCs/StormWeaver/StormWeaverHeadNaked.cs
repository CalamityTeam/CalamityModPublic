using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.StormWeaver
{
    [AutoloadBossHead]
    public class StormWeaverHeadNaked : ModNPC
    {
        private const float BoltAngleSpread = 280;
        private const float speed = 13f;
        private const float turnSpeed = 0.35f;
        private bool tail = false;
        private int invinceTime = 180;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 74;
            npc.height = 74;
			bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel2;
			npc.LifeMaxNERB(notDoGFight ? 585000 : 97500, notDoGFight ? 585000 : 97500, 3500000);
			Mod calamityModMusic = CalamityMod.Instance.musicMod;
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss3;
            if (notDoGFight)
            {
                npc.value = Item.buyPrice(0, 35, 0, 0);
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Weaver");
                else
                    music = MusicID.Boss3;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit13;
            npc.DeathSound = SoundID.NPCDeath13;
            npc.netAlways = true;
            bossBag = ModContent.ItemType<StormWeaverBag>();

			if (CalamityWorld.death || BossRushEvent.BossRushActive)
				npc.scale = 1.2f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
				npc.scale = 1.1f;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			bool malice = CalamityWorld.malice;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive || malice;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || malice;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive || malice;

            if (invinceTime > 0)
            {
                invinceTime--;
                npc.damage = 0;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }

            if (!Main.raining && !BossRushEvent.BossRushActive && CalamityWorld.DoGSecondStageCountdown <= 0)
            {
				CalamityUtils.StartRain();
            }

            double lifeRatio = npc.life / (double)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			int lifePercentage = (int)(100.0 * lifeRatio);

			int BoltProjectiles = 2;
            if (lifePercentage < 33 || death)
            {
                BoltProjectiles = 4;
            }
            else if (lifePercentage < 66)
            {
                BoltProjectiles = 3;
            }

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			if (npc.alpha != 0)
            {
                for (int num934 = 0; num934 < 2; num934++)
                {
                    int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }

            npc.alpha -= 12;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
					int totalLength = death ? 60 : revenge ? 50 : expertMode ? 40 : 30;
					for (int num36 = 0; num36 < totalLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < totalLength - 1)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverBodyNaked>(), npc.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<StormWeaverTailNaked>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        npc.netUpdate = true;
                        Previous = lol;
                    }
                    tail = true;
                }

				if (expertMode)
				{
					npc.localAI[0] += malice ? 1.5f : 1f;
					if (npc.localAI[0] >= 450f)
					{
						npc.localAI[0] = 0f;
						npc.netUpdate = true;

						int random = Main.rand.Next(250, 501);
						float xPos = Main.rand.NextBool(2) ? Main.player[npc.target].position.X + random : Main.player[npc.target].position.X - random;
						random = Main.rand.Next(250, 501);
						float yPos = Main.rand.NextBool(2) ? Main.player[npc.target].position.Y + random : Main.player[npc.target].position.Y - random;
						Vector2 spawnPos = new Vector2(xPos, yPos);

						int type = ProjectileID.CultistBossLightningOrb;
						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(spawnPos, Vector2.Zero, type, damage, 0f, Main.myPlayer);
					}
				}
            }

            if (Main.player[npc.target].dead && npc.life > 0)
            {
				npc.localAI[1] = 0f;
				calamityGlobalNPC.newAI[0] = 0f;
				npc.TargetClosest(false);
                npc.velocity.Y = npc.velocity.Y - 10f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y = npc.velocity.Y - 10f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    CalamityWorld.DoGSecondStageCountdown = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                        netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                        netMessage.Send();
                    }
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].active && (Main.npc[num957].type == ModContent.NPCType<StormWeaverBodyNaked>() 
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverHeadNaked>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverTailNaked>()))
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }

            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 10000f && npc.life > 0)
            {
                CalamityWorld.DoGSecondStageCountdown = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
                for (int num957 = 0; num957 < 200; num957++)
                {
                    if (Main.npc[num957].type == ModContent.NPCType<StormWeaverBodyNaked>()
                       || Main.npc[num957].type == ModContent.NPCType<StormWeaverHeadNaked>()
                       || Main.npc[num957].type == ModContent.NPCType<StormWeaverTailNaked>())
                    {
                        Main.npc[num957].active = false;
                    }
                }
            }

            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }

            Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
            float num188 = malice ? 15f : revenge ? 13f : 12f;
            float num189 = malice ? 0.36f : revenge ? 0.31f : 0.28f;

			calamityGlobalNPC.newAI[0] += 1f;
			if (calamityGlobalNPC.newAI[0] >= 400f)
			{
				if (npc.localAI[1] == 0f)
					npc.localAI[1] = 1f;

				if (calamityGlobalNPC.newAI[0] >= 500f)
				{
					npc.TargetClosest();
					npc.localAI[1] = 0f;
					calamityGlobalNPC.newAI[0] = 0f;
				}

				if (revenge)
				{
					if (npc.localAI[1] == 2f)
					{
						num188 += Vector2.Distance(Main.player[npc.target].Center, npc.Center) * 0.01f * (1f - (float)lifeRatio);
						num189 += Vector2.Distance(Main.player[npc.target].Center, npc.Center) * 0.0001f * (1f - (float)lifeRatio);
						num188 *= 2f;
						num189 *= 0.85f;

						float stopChargeDistance = 800f * npc.localAI[2];
						if (stopChargeDistance < 0)
						{
							if (npc.Center.X < Main.player[npc.target].Center.X + stopChargeDistance)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = 0f;
							}
						}
						else
						{
							if (npc.Center.X > Main.player[npc.target].Center.X + stopChargeDistance)
							{
								npc.localAI[1] = 0f;
								calamityGlobalNPC.newAI[0] = 0f;
							}
						}
					}

					int dustAmt = 5;
					for (int num1474 = 0; num1474 < dustAmt; num1474++)
					{
						Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((npc.width + 50) / 2f, npc.height) * 0.75f;
						vector171 = vector171.RotatedBy((num1474 - (dustAmt / 2 - 1)) * (double)MathHelper.Pi / (float)dustAmt) + npc.Center;
						Vector2 value18 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
						int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 206, value18.X, value18.Y, 100, default, 3f);
						Main.dust[num1475].noGravity = true;
						Main.dust[num1475].noLight = true;
						Main.dust[num1475].velocity /= 4f;
						Main.dust[num1475].velocity -= npc.velocity;
					}
				}
			}
			else if (revenge)
				calamityGlobalNPC.newAI[0] += malice ? 4f : death ? 2f : 2f * (float)(1D - lifeRatio);

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

			if (npc.localAI[1] == 1f)
			{
				npc.localAI[1] = 2f;
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/LightningStrike"), (int)Main.player[npc.target].Center.X, (int)Main.player[npc.target].Center.Y);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int speed2 = revenge ? 8 : 7;
					if (npc.Calamity().enraged > 0)
					{
						speed2 += 1;
					}
					float spawnX2 = npc.Center.X > Main.player[npc.target].Center.X ? 1000f : -1000f;
					float spawnY2 = -1000f + Main.player[npc.target].Center.Y;
					Vector2 baseSpawn = new Vector2(spawnX2 + Main.player[npc.target].Center.X, spawnY2);
					Vector2 baseVelocity = Main.player[npc.target].Center - baseSpawn;
					baseVelocity.Normalize();
					baseVelocity *= speed2;
					for (int i = 0; i < BoltProjectiles; i++)
					{
						Vector2 source = baseSpawn;
						source.X += i * 30 - (BoltProjectiles * 15);
						Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / BoltProjectiles)));
						velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
						Vector2 vector94 = Main.player[npc.target].Center - source;
						float ai = Main.rand.Next(100);
						int type = ProjectileID.CultistBossLightningOrbArc;
						int damage = npc.GetProjectileDamage(type);
						Projectile.NewProjectile(source, velocity, type, damage, 0f, Main.myPlayer, vector94.ToRotation(), ai);
					}
				}

				if (revenge)
					npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * (num188 + Vector2.Distance(Main.player[npc.target].Center, npc.Center) * 0.01f * (1f - (float)lifeRatio)) * 2f;

				float chargeDirection = 0;
				if (npc.velocity.X < 0f)
					chargeDirection = -1f;
				else if (npc.velocity.X > 0f)
					chargeDirection = 1f;

				npc.localAI[2] = chargeDirection;
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
                {
                    npc.velocity.X = npc.velocity.X + num189;
                }
                else
                {
                    if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                if (npc.velocity.Y < num192)
                {
                    npc.velocity.Y = npc.velocity.Y + num189;
                }
                else
                {
                    if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189;
                    }
                }
                if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                    }
                }
                if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
                    }
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                    }
                    else if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                    }
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                }
            }
            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + 1.57f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;

					if (npc.Calamity().newAI[0] > 280f && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
						color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp((npc.Calamity().newAI[0] - 280f) / 120f, 0f, 1f));

					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			Color color = npc.GetAlpha(lightColor);

			if (npc.Calamity().newAI[0] > 280f && (CalamityWorld.revenge || BossRushEvent.BossRushActive))
				color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp((npc.Calamity().newAI[0] - 280f) / 120f, 0f, 1f));

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			int buffDuration = npc.Calamity().newAI[0] >= 400f ? 300 : 90;
			player.AddBuff(BuffID.Electrified, buffDuration, true);
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNudeHead1"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/SWNudeHead2"), 1f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 30;
                npc.height = 30;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int num569 = 0; num569 < 200; num569++)
            {
                if (Main.npc[num569].active && (Main.npc[num569].type == ModContent.NPCType<StormWeaverBodyNaked>() || Main.npc[num569].type == ModContent.NPCType<StormWeaverTailNaked>()))
                {
                    Main.npc[num569].life = 0;
                }
            }
            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<StormWeaverHeadNaked>(),
                ModContent.NPCType<StormWeaverBodyNaked>(),
                ModContent.NPCType<StormWeaverTailNaked>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            // Only drop items if fought at full strength
			bool fullStrength = !CalamityWorld.downedSentinel2 || CalamityWorld.DoGSecondStageCountdown <= 0;
            if (fullStrength)
            {
				DropHelper.DropBags(npc);

				// Legendary drop for Storm Weaver
				DropHelper.DropItemCondition(npc, ModContent.ItemType<Thunderstorm>(), true, CalamityWorld.malice);

				DropHelper.DropItemChance(npc, ModContent.ItemType<WeaverTrophy>(), 10);
				bool lastSentinelKilled = CalamityWorld.downedSentinel1 && !CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3;
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);
				DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSentinel2, 5, 2, 1);
				if (!Main.expertMode)
				{
					// Materials
					DropHelper.DropItem(npc, ModContent.ItemType<ArmoredShell>(), true, 5, 8);

					// Weapons
					DropHelper.DropItemChance(npc, ModContent.ItemType<TheStorm>(), 4);
					DropHelper.DropItemChance(npc, ModContent.ItemType<StormDragoon>(), 4);

					// Vanity
					DropHelper.DropItemChance(npc, ModContent.ItemType<StormWeaverMask>(), 7);
					if (Main.rand.NextBool(20))
					{
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerHelm>());
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerChestplate>());
						DropHelper.DropItem(npc, ModContent.ItemType<AncientGodSlayerLeggings>());
						DropHelper.DropItem(npc, ModContent.ItemType<GodSlayerHornedHelm>());
						DropHelper.DropItem(npc, ModContent.ItemType<GodSlayerVisage>());
					}
				}
            }

            // If DoG's fight is active, set the timer for Signus' phase
            if (CalamityWorld.DoGSecondStageCountdown > 7260)
            {
                CalamityWorld.DoGSecondStageCountdown = 7260;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

			// Mark Storm Weaver as dead
			if (fullStrength)
			{
				CalamityWorld.downedSentinel2 = true;
				CalamityNetcode.SyncWorld();
			}
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
