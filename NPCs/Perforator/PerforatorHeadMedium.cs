using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Perforator
{
	[AutoloadBossHead]
    public class PerforatorHeadMedium : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 58;
            npc.height = 68;
            npc.defense = 2;
			npc.LifeMaxNERB(150, 180, 7000);
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

			npc.Calamity().SplittingWorm = true;

			npc.Calamity().VulnerableToHeat = true;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = true;
		}

		public override void AI()
		{
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
			if ((npc.position.Y / 16f) < Main.worldSurface || malice)
				enrageScale += 1f;
			if (!Main.player[npc.target].ZoneCrimson || malice)
				enrageScale += 1f;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			float speed = 8f;
			float turnSpeed = 0.06f;

			if (expertMode)
			{
				float velocityScale = (death ? 8f : 7f) * enrageScale;
				speed += velocityScale * (1f - lifeRatio);
				float accelerationScale = (death ? 0.08f : 0.07f) * enrageScale;
				turnSpeed += accelerationScale * (1f - lifeRatio);
			}

			npc.realLife = -1;

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
				if (npc.ai[0] == 0f)
				{
					int totalSegments = death ? 14 : revenge ? 13 : expertMode ? 12 : 10;
					npc.ai[2] = totalSegments;
					npc.ai[0] = NPC.NewNPC((int)(npc.position.X + (npc.width / 2)), (int)(npc.position.Y + npc.height), ModContent.NPCType<PerforatorBodyMedium>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
					Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
					Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
					npc.netUpdate = true;
				}

				// Splitting effect
				if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
				if (!Main.npc[(int)npc.ai[0]].active)
				{
					npc.life = 0;
					npc.HitEffect(0, 10.0);
					npc.checkDead();
					npc.active = false;
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}

				if (!npc.active && Main.netMode == NetmodeID.Server)
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
			}

			// Movement
			int num29 = (int)(npc.position.X / 16f) - 1;
			int num30 = (int)((npc.position.X + npc.width) / 16f) + 2;
			int num31 = (int)(npc.position.Y / 16f) - 1;
			int num32 = (int)((npc.position.Y + npc.height) / 16f) + 2;
			if (num29 < 0)
				num29 = 0;
			if (num30 > Main.maxTilesX)
				num30 = Main.maxTilesX;
			if (num31 < 0)
				num31 = 0;
			if (num32 > Main.maxTilesY)
				num32 = Main.maxTilesY;

			// Fly or not
			bool flag2 = false;
			if (!flag2)
			{
				for (int num33 = num29; num33 < num30; num33++)
				{
					for (int num34 = num31; num34 < num32; num34++)
					{
						if (Main.tile[num33, num34] != null && ((Main.tile[num33, num34].nactive() && (Main.tileSolid[Main.tile[num33, num34].type] || (Main.tileSolidTop[Main.tile[num33, num34].type] && Main.tile[num33, num34].frameY == 0))) || Main.tile[num33, num34].liquid > 64))
						{
							Vector2 vector;
							vector.X = num33 * 16;
							vector.Y = num34 * 16;
							if (npc.position.X + npc.width > vector.X && npc.position.X < vector.X + 16f && npc.position.Y + npc.height > vector.Y && npc.position.Y < vector.Y + 16f)
							{
								flag2 = true;
								if (Main.rand.NextBool(100) && Main.tile[num33, num34].nactive())
								{
									WorldGen.KillTile(num33, num34, true, true, false);
								}
							}
						}
					}
				}
			}
			if (!flag2)
			{
				Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
				int num35 = death ? 160 : revenge ? 200 : expertMode ? 240 : 300;
				bool flag3 = true;
				for (int num36 = 0; num36 < Main.maxPlayers; num36++)
				{
					if (Main.player[num36].active)
					{
						Rectangle rectangle2 = new Rectangle((int)Main.player[num36].position.X - num35, (int)Main.player[num36].position.Y - num35, num35 * 2, num35 * 2);
						if (rectangle.Intersects(rectangle2))
						{
							flag3 = false;
							break;
						}
					}
				}
				if (flag3)
					flag2 = true;
			}

			if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
			{
				npc.TargetClosest(false);
				flag2 = false;
				npc.velocity.Y += 1f;
				if (npc.position.Y > Main.worldSurface * 16.0)
				{
					npc.velocity.Y += 1f;
				}
				if (npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int num957 = 0; num957 < Main.maxNPCs; num957++)
					{
						if (Main.npc[num957].type == npc.type || Main.npc[num957].type == ModContent.NPCType<PerforatorBodyMedium>() || Main.npc[num957].type == ModContent.NPCType<PerforatorTailMedium>())
						{
							Main.npc[num957].active = false;
						}
					}
				}
			}

			// Velocity and acceleration
			float num37 = speed;
			float num38 = turnSpeed;

			Vector2 vector2 = npc.Center;
			float num39 = player.Center.X;
			float num40 = player.Center.Y;

			num39 = (int)(num39 / 16f) * 16;
			num40 = (int)(num40 / 16f) * 16;
			vector2.X = (int)(vector2.X / 16f) * 16;
			vector2.Y = (int)(vector2.Y / 16f) * 16;
			num39 -= vector2.X;
			num40 -= vector2.Y;
			float num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

			// Prevent new heads from being slowed when they spawn
			if (npc.Calamity().newAI[1] < 3f)
			{
				npc.Calamity().newAI[1] += 1f;

				// Set velocity for when a new head spawns
				npc.velocity = Vector2.Normalize(player.Center - npc.Center) * (num37 * (death ? 0.3f : 0.2f));
			}

			if (!flag2)
			{
				npc.velocity.Y += 0.11f;
				if (npc.velocity.Y > num37)
					npc.velocity.Y = num37;

				if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.4)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X -= num38 * 1.1f;
					else
						npc.velocity.X += num38 * 1.1f;
				}
				else if (npc.velocity.Y == num37)
				{
					if (npc.velocity.X < num39)
						npc.velocity.X += num38;
					else if (npc.velocity.X > num39)
						npc.velocity.X -= num38;
				}
				else if (npc.velocity.Y > 4f)
				{
					if (npc.velocity.X < 0f)
						npc.velocity.X += num38 * 0.9f;
					else
						npc.velocity.X -= num38 * 0.9f;
				}
			}
			else
			{
				// Sound
				if (npc.soundDelay == 0)
				{
					float num54 = num52 / 40f;
					if (num54 < 10f)
						num54 = 10f;
					if (num54 > 20f)
						num54 = 20f;

					npc.soundDelay = (int)num54;
					Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1, 1f, 0f);
				}

				num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
				float num55 = Math.Abs(num39);
				float num56 = Math.Abs(num40);
				float num57 = num37 / num52;
				num39 *= num57;
				num40 *= num57;

				if ((npc.velocity.X > 0f && num39 > 0f) || (npc.velocity.X < 0f && num39 < 0f) || (npc.velocity.Y > 0f && num40 > 0f) || (npc.velocity.Y < 0f && num40 < 0f))
				{
					if (npc.velocity.X < num39)
						npc.velocity.X += num38;
					else if (npc.velocity.X > num39)
						npc.velocity.X -= num38;
					if (npc.velocity.Y < num40)
						npc.velocity.Y += num38;
					else if (npc.velocity.Y > num40)
						npc.velocity.Y -= num38;

					if (Math.Abs(num40) < num37 * 0.2 && ((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f)))
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y += num38 * 2f;
						else
							npc.velocity.Y -= num38 * 2f;
					}

					if (Math.Abs(num39) < num37 * 0.2 && ((npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)))
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X += num38 * 2f;
						else
							npc.velocity.X -= num38 * 2f;
					}
				}
				else if (num55 > num56)
				{
					if (npc.velocity.X < num39)
						npc.velocity.X += num38 * 1.1f;
					else if (npc.velocity.X > num39)
						npc.velocity.X -= num38 * 1.1f;

					if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.5)
					{
						if (npc.velocity.Y > 0f)
							npc.velocity.Y += num38;
						else
							npc.velocity.Y -= num38;
					}
				}
				else
				{
					if (npc.velocity.Y < num40)
						npc.velocity.Y += num38 * 1.1f;
					else if (npc.velocity.Y > num40)
						npc.velocity.Y -= num38 * 1.1f;

					if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.5)
					{
						if (npc.velocity.X > 0f)
							npc.velocity.X += num38;
						else
							npc.velocity.X -= num38;
					}
				}
			}

			npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

			if (flag2)
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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorHeadMediumGlow");
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf2"), npc.scale);
            }
        }

		public override bool PreNPCLoot()
		{
			if (!CalamityWorld.revenge)
			{
				int closestPlayer = Player.FindClosest(npc.Center, 1, 1);
				if (Main.rand.Next(4) == 0 && Main.player[closestPlayer].statLife < Main.player[closestPlayer].statLifeMax2)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Heart);
			}

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == npc.type || Main.npc[i].type == ModContent.NPCType<PerforatorBodyMedium>() || Main.npc[i].type == ModContent.NPCType<PerforatorTailMedium>()))
					return false;
			}

			return true;
		}

		public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The Medium Perforator";
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                npc.type,
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
			DropHelper.DropItem(npc, ModContent.ItemType<BloodSample>(), 3, 7);
			DropHelper.DropItem(npc, ItemID.CrimtaneBar, 2, 4);
			DropHelper.DropItem(npc, ItemID.Vertebrae, 2, 3);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 240, true);
        }
    }
}
