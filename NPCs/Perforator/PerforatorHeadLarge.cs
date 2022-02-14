using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;

namespace CalamityMod.NPCs.Perforator
{
	[AutoloadBossHead]
    public class PerforatorHeadLarge : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
		private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 70;
            npc.height = 84;
            npc.defense = 4;
			npc.LifeMaxNERB(2250, 2700, 80000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;

			if (CalamityWorld.malice || BossRushEvent.BossRushActive)
				npc.scale = 1.25f;
			else if (CalamityWorld.death)
				npc.scale = 1.2f;
			else if (CalamityWorld.revenge)
				npc.scale = 1.15f;
			else if (Main.expertMode)
				npc.scale = 1.1f;

			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(biomeEnrageTimer);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			biomeEnrageTimer = reader.ReadInt32();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

		public override void AI()
        {
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Enrage
			if ((!Main.player[npc.target].ZoneCrimson || (npc.position.Y / 16f) < Main.worldSurface) && !BossRushEvent.BossRushActive)
			{
				if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
			}
			else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

			bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if (biomeEnraged && (!Main.player[npc.target].ZoneCrimson || malice))
				enrageScale += 1f;
			if (biomeEnraged && ((npc.position.Y / 16f) < Main.worldSurface || malice))
				enrageScale += 1f;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			if (revenge || lifeRatio < (expertMode ? 0.75f : 0.5f))
				npc.Calamity().newAI[0] += 1f;

			float burrowTimeGateValue = death ? 480f : 600f;
			bool burrow = npc.Calamity().newAI[0] >= burrowTimeGateValue;
			bool resetTime = npc.Calamity().newAI[0] >= burrowTimeGateValue + 600f;
			bool lungeUpward = burrow && npc.Calamity().newAI[1] == 1f;
			bool quickFall = npc.Calamity().newAI[1] == 2f;

			float speed = 0.09f;
			float turnSpeed = 0.06f;

			if (expertMode)
			{
				float velocityScale = (death ? 0.12f : 0.1f) * enrageScale;
				speed += velocityScale * (1f - lifeRatio);
				float accelerationScale = (death ? 0.12f : 0.1f) * enrageScale;
				turnSpeed += accelerationScale * (1f - lifeRatio);
			}

			if (lungeUpward)
			{
				speed *= 1.25f;
				turnSpeed *= 1.5f;
			}

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

            npc.alpha -= 42;
            if (npc.alpha < 0)
                npc.alpha = 0;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!TailSpawned)
				{
					int Previous = npc.whoAmI;
					int maxLength = death ? 27 : revenge ? 24 : expertMode ? 21 : 15;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol;
						if (num36 >= 0 && num36 < maxLength - 1)
						{
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<PerforatorBodyLarge>(), npc.whoAmI);
						}
						else
						{
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<PerforatorTailLarge>(), npc.whoAmI);
						}
						if (num36 % 2 == 0)
						{
							Main.npc[lol].localAI[3] = 1f;
						}
						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = npc.whoAmI;
						Main.npc[lol].ai[1] = Previous;
						Main.npc[Previous].ai[0] = lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					TailSpawned = true;
				}
			}

			int num12 = (int)(npc.position.X / 16f) - 1;
			int num13 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num14 = (int)(npc.position.Y / 16f) - 1;
			int num15 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num12 < 0)
			{
				num12 = 0;
			}
			if (num13 > Main.maxTilesX)
			{
				num13 = Main.maxTilesX;
			}
			if (num14 < 0)
			{
				num14 = 0;
			}
			if (num15 > Main.maxTilesY)
			{
				num15 = Main.maxTilesY;
			}
			bool flag2 = lungeUpward;
			if (!flag2)
			{
				for (int k = num12; k < num13; k++)
				{
					for (int l = num14; l < num15; l++)
					{
						if (Main.tile[k, l] != null && ((Main.tile[k, l].nactive() && (Main.tileSolid[(int)Main.tile[k, l].type] || (Main.tileSolidTop[(int)Main.tile[k, l].type] && Main.tile[k, l].frameY == 0))) || Main.tile[k, l].liquid > 64))
						{
							Vector2 vector2;
							vector2.X = (float)(k * 16);
							vector2.Y = (float)(l * 16);
							if (npc.position.X + (float)npc.width > vector2.X && npc.position.X < vector2.X + 16f && npc.position.Y + (float)npc.height > vector2.Y && npc.position.Y < vector2.Y + 16f)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
			}
			if (!flag2)
			{
				npc.localAI[1] = 1f;
				Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num16 = death ? 160 : revenge ? 200 : expertMode ? 240 : 300;
				bool flag3 = true;
				if (npc.position.Y > player.position.Y)
				{
					for (int m = 0; m < 255; m++)
					{
						if (Main.player[m].active)
						{
							Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, num16 * 2);
							if (rectangle.Intersects(rectangle2))
							{
								flag3 = false;
								break;
							}
						}
					}
					if (flag3)
					{
						flag2 = true;
					}
				}
			}
			else
			{
				npc.localAI[1] = 0f;
			}

			float num17 = 16f;
			if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
			{
				flag2 = false;
				npc.velocity.Y = npc.velocity.Y + 1f;
				if ((double)npc.position.Y > Main.worldSurface * 16.0)
				{
					npc.velocity.Y = npc.velocity.Y + 1f;
					num17 = 32f;
				}
				if ((double)npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int a = 0; a < 200; a++)
					{
						if (Main.npc[a].type == ModContent.NPCType<PerforatorHeadLarge>() || Main.npc[a].type == ModContent.NPCType<PerforatorBodyLarge>() ||
							Main.npc[a].type == ModContent.NPCType<PerforatorTailLarge>())
						{
							Main.npc[a].active = false;
						}
					}
				}
			}

			float num18 = speed;
			float num19 = turnSpeed;
			float burrowDistance = malice ? 500f : 800f;
			float burrowTarget = player.Center.Y + burrowDistance;
			float lungeTarget = player.Center.Y - 600f;
			Vector2 vector3 = npc.Center;
			float num20 = player.Center.X;
			float num21 = lungeUpward ? lungeTarget : burrow ? burrowTarget : player.Center.Y;
			num20 = (float)((int)(num20 / 16f) * 16);
			num21 = (float)((int)(num21 / 16f) * 16);
			vector3.X = (float)((int)(vector3.X / 16f) * 16);
			vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
			num20 -= vector3.X;
			num21 -= vector3.Y;
			float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));

			// Lunge up towards target
			if (burrow && npc.Center.Y >= burrowTarget - 16f)
				npc.Calamity().newAI[1] = 1f;

			// Quickly fall back down once above target
			if (lungeUpward && npc.Center.Y <= player.Center.Y - 420f)
			{
				npc.TargetClosest();
				npc.Calamity().newAI[1] = 2f;
			}

			// Quickly fall and reset variables once at target's Y position
			if (quickFall)
			{
				npc.velocity.Y += 0.5f;
				if (npc.Center.Y >= player.Center.Y)
				{
					npc.Calamity().newAI[0] = 0f;
					npc.Calamity().newAI[1] = 0f;
				}
			}

			// Reset variables if the burrow and lunge attack is taking too long
			if (resetTime)
			{
				npc.Calamity().newAI[0] = 0f;
				npc.Calamity().newAI[1] = 0f;
			}

			if (!flag2)
			{
				npc.TargetClosest(true);
				npc.velocity.Y = npc.velocity.Y + 0.15f;
				if (npc.velocity.Y > num17)
				{
					npc.velocity.Y = num17;
				}
				if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.4)
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X - num18 * 1.1f;
					}
					else
					{
						npc.velocity.X = npc.velocity.X + num18 * 1.1f;
					}
				}
				else if (npc.velocity.Y == num17)
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18;
					}
				}
				else if (npc.velocity.Y > 4f)
				{
					if (npc.velocity.X < 0f)
					{
						npc.velocity.X = npc.velocity.X + num18 * 0.9f;
					}
					else
					{
						npc.velocity.X = npc.velocity.X - num18 * 0.9f;
					}
				}
			}
			else
			{
				if (npc.soundDelay == 0)
				{
					float num24 = num22 / 40f;
					if (num24 < 10f)
					{
						num24 = 10f;
					}
					if (num24 > 20f)
					{
						num24 = 20f;
					}
					npc.soundDelay = (int)num24;
					Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
				}
				num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
				float num25 = Math.Abs(num20);
				float num26 = Math.Abs(num21);
				float num27 = num17 / num22;
				num20 *= num27;
				num21 *= num27;
				if (((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f)) && ((npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f)))
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num19;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num19;
					}
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num19;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num19;
					}
				}
				if ((npc.velocity.X > 0f && num20 > 0f) || (npc.velocity.X < 0f && num20 < 0f) || (npc.velocity.Y > 0f && num21 > 0f) || (npc.velocity.Y < 0f && num21 < 0f))
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18;
					}
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num18;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num18;
					}
					if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((npc.velocity.X > 0f && num20 < 0f) || (npc.velocity.X < 0f && num20 > 0f)))
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num18 * 2f;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num18 * 2f;
						}
					}
					if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((npc.velocity.Y > 0f && num21 < 0f) || (npc.velocity.Y < 0f && num21 > 0f)))
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num18 * 2f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num18 * 2f;
						}
					}
				}
				else if (num25 > num26)
				{
					if (npc.velocity.X < num20)
					{
						npc.velocity.X = npc.velocity.X + num18 * 1.1f;
					}
					else if (npc.velocity.X > num20)
					{
						npc.velocity.X = npc.velocity.X - num18 * 1.1f;
					}
					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + num18;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - num18;
						}
					}
				}
				else
				{
					if (npc.velocity.Y < num21)
					{
						npc.velocity.Y = npc.velocity.Y + num18 * 1.1f;
					}
					else if (npc.velocity.Y > num21)
					{
						npc.velocity.Y = npc.velocity.Y - num18 * 1.1f;
					}
					if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num17 * 0.5)
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + num18;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - num18;
						}
					}
				}
			}
			npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
			if (flag2)
			{
				if (npc.localAI[0] != 1f)
				{
					npc.netUpdate = true;
				}
				npc.localAI[0] = 1f;
			}
			else
			{
				if (npc.localAI[0] != 0f)
				{
					npc.netUpdate = true;
				}
				npc.localAI[0] = 0f;
			}
			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorHeadLargeGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LargePerf"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/LargePerf2"), npc.scale);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The Large Perforator";
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorTailLarge>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
			if (!CalamityWorld.revenge)
			{
				int heartAmt = Main.rand.Next(3) + 3;
				for (int i = 0; i < heartAmt; i++)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			DropHelper.DropItem(npc, ModContent.ItemType<BloodSample>(), 4, 8);
			DropHelper.DropItem(npc, ItemID.CrimtaneBar, 3, 5);
			DropHelper.DropItem(npc, ItemID.Vertebrae, 2, 4);
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 300, true);
        }
    }
}
