using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Polterghast
{
	public class PolterghastHook : ModNPC
	{
		private int despawnTimer = 300;
		private bool phase2 = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Polterghast Hook");
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 0;
			npc.width = 40;
			npc.height = 40;
			npc.lifeMax = 50000;
			npc.dontTakeDamage = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit34;
			npc.DeathSound = SoundID.NPCDeath39;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(phase2);
			writer.Write(despawnTimer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			phase2 = reader.ReadBoolean();
			despawnTimer = reader.ReadInt32();
		}

		public override void AI()
		{
			// Emit light
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 1f, 1f);

			// Bools
			bool speedBoost1 = false;
			bool despawnBoost = false;

			// Despawn if Polter is gone
			if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			// Percent life remaining, Polter
			float lifeRatio = (float)Main.npc[CalamityGlobalNPC.ghostBoss].life / (float)Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

			// Despawn
			if (CalamityGlobalNPC.ghostBoss != -1 && !Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].ZoneDungeon &&
				(double)Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].position.Y < Main.worldSurface * 16.0 && !CalamityWorld.bossRushActive)
			{
				despawnTimer--;
				if (despawnTimer <= 0)
					despawnBoost = true;

				npc.localAI[0] -= 6f;
				speedBoost1 = true;
			}
			else
				despawnTimer++;

			// Phase 2
			if (lifeRatio < 0.75f && lifeRatio >= ((CalamityWorld.revenge || CalamityWorld.bossRushActive) ? 0.5 : 0.33))
			{
				phase2 = true;

				npc.TargetClosest(true);

				Movement(phase2, (Main.expertMode || CalamityWorld.bossRushActive), (CalamityWorld.revenge || CalamityWorld.bossRushActive), speedBoost1, despawnBoost, lifeRatio);

				// Fire projectiles
				Vector2 vector17 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num147 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector17.X;
				float num148 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector17.Y;
				float num149 = (float)Math.Sqrt((double)(num147 * num147 + num148 * num148));

				num149 = 4f / num149;
				num147 *= num149;
				num148 *= num149;

				if (num149 > 1200f)
				{
					npc.ai[2] = 0f;
					npc.ai[3] = 0f;
					return;
				}

				npc.ai[2] += 1f;
				if (npc.ai[3] == 0f)
				{
					if (npc.ai[2] > 120f)
					{
						npc.ai[2] = 0f;
						npc.ai[3] = 1f;
						npc.netUpdate = true;
					}
				}
				else
				{
					if (npc.ai[2] > 40f)
						npc.ai[3] = 0f;

					if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 20f)
					{
						float num151 = CalamityWorld.bossRushActive ? 7.5f : 5f;
						int num152 = Main.expertMode ? 48 : 60;
						int num153 = mod.ProjectileType("PhantomHookShot");
						num149 = num151 / num149;
						num147 *= num149;
						num148 *= num149;
						Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, num153, num152, 0f, Main.myPlayer, 0f, 0f);
					}
				}
				return;
			}

			// Phase 1 or 3
			phase2 = false;
			Movement(phase2, (Main.expertMode || CalamityWorld.bossRushActive), (CalamityWorld.revenge || CalamityWorld.bossRushActive), speedBoost1, despawnBoost, lifeRatio);
		}

		private void Movement(bool phase2, bool expertMode, bool revenge, bool speedBoost1, bool despawnBoost, float lifeRatio)
		{
			if (phase2)
			{
				Vector2 vector92 = new Vector2(npc.Center.X, npc.Center.Y);
				float num740 = Main.player[npc.target].Center.X - vector92.X;
				float num741 = Main.player[npc.target].Center.Y - vector92.Y;
				npc.rotation = (float)Math.Atan2((double)num741, (double)num740) + 1.57f;
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (npc.ai[0] == 0f)
					npc.ai[0] = (float)((int)(npc.Center.X / 16f));
				if (npc.ai[1] == 0f)
					npc.ai[1] = (float)((int)(npc.Center.X / 16f));
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.ai[0] == 0f || npc.ai[1] == 0f)
					npc.localAI[0] = 0f;

				npc.localAI[0] -= 1f + (2f * (1f - lifeRatio));
				if (speedBoost1)
					npc.localAI[0] -= 6f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					npc.localAI[0] -= 0.5f;

				if (!despawnBoost && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
				{
					for (int num763 = 0; num763 < 200; num763++)
					{
						if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
							npc.localAI[0] = (float)Main.rand.Next(30, 121);
					}
				}

				if (npc.localAI[0] <= 0f)
				{
					npc.localAI[0] = (float)Main.rand.Next(150, 301);
					bool flag50 = false;
					int num764 = 0;
					while (!flag50 && num764 <= 1000)
					{
						num764++;
						int num765 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X / 16f);
						int num766 = (int)(Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y / 16f);
						if (npc.ai[0] == 0f)
						{
							num765 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
							num766 = (int)((Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target].Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
						}
						if (despawnBoost)
						{
							num765 = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
							num766 = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
						}
						int num767 = 20;
						num767 += (int)(100f * ((float)num764 / 1000f));
						int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
						int num769 = num766 + Main.rand.Next(-num767, num767 + 1);
						try
						{
							if (WorldGen.SolidTile(num768, num769) || Main.tile[num768, num769].wall > 0)
							{
								flag50 = true;
								npc.ai[0] = (float)num768;
								npc.ai[1] = (float)num769;
								npc.netUpdate = true;
							}
						}
						catch
						{
						}
					}
				}
			}

			if (npc.ai[0] > 0f && npc.ai[1] > 0f)
			{
				float velocity = 8f + (2f * (1f - lifeRatio));
				if (expertMode)
					velocity += 1f;
				if (revenge)
					velocity += 1f;
				if (CalamityWorld.death || CalamityWorld.bossRushActive)
					velocity += 1f;
				if (speedBoost1)
					velocity *= 2f;
				if (despawnBoost)
					velocity *= 2f;

				Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
				float num773 = npc.ai[0] * 16f - 8f - vector95.X;
				float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
				float num775 = (float)Math.Sqrt((double)(num773 * num773 + num774 * num774));
				if (num775 < 12f + velocity)
				{
					npc.velocity.X = num773;
					npc.velocity.Y = num774;
				}
				else
				{
					num775 = velocity / num775;
					npc.velocity.X = num773 * num775;
					npc.velocity.Y = num774 * num775;
				}

				if (!phase2)
				{
					Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
					float num776 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - vector96.X;
					float num777 = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - vector96.Y;
					npc.rotation = (float)Math.Atan2((double)num777, (double)num776) - 1.57f;
				}
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (phase2)
			{
				if (npc.ai[3] == 0f)
				{
					if (npc.frame.Y < 1)
					{
						npc.frameCounter += 1.0;
						if (npc.frameCounter > 4.0)
						{
							npc.frameCounter = 0.0;
							npc.frame.Y = npc.frame.Y + frameHeight;
						}
					}
				}
				else if (npc.frame.Y > 0)
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 4.0)
					{
						npc.frameCounter = 0.0;
						npc.frame.Y = npc.frame.Y - frameHeight;
					}
				}
				return;
			}
			if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
			{
				if (npc.frame.Y < 1)
				{
					npc.frameCounter += 1.0;
					if (npc.frameCounter > 4.0)
					{
						npc.frameCounter = 0.0;
						npc.frame.Y = npc.frame.Y + frameHeight;
					}
				}
			}
			else if (npc.frame.Y > 0)
			{
				npc.frameCounter += 1.0;
				if (npc.frameCounter > 4.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y - frameHeight;
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (Main.npc[CalamityGlobalNPC.ghostBoss].active && !phase2)
			{
				Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
				float bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
				float bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
				float rotation2 = (float)Math.Atan2((double)bossCenterY, (double)bossCenterX) - 1.57f;
				bool draw = true;
				while (draw)
				{
					int chainWidth = 20; //16 24
					int chainHeight = 52; //32 16
					float num10 = (float)Math.Sqrt((double)(bossCenterX * bossCenterX + bossCenterY * bossCenterY));
					if (num10 < (float)chainHeight)
					{
						chainWidth = (int)num10 - chainHeight + chainWidth;
						draw = false;
					}
					num10 = (float)chainWidth / num10;
					bossCenterX *= num10;
					bossCenterY *= num10;
					center.X += bossCenterX;
					center.Y += bossCenterY;
					bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
					bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
					Microsoft.Xna.Framework.Color color2 = new Color(100, 100, 100, 0);
					Main.spriteBatch.Draw(mod.GetTexture("NPCs/Polterghast/PolterghastChain"), new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y),
						new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/Polterghast/PolterghastChain").Width, chainWidth)), color2, rotation2,
						new Vector2((float)mod.GetTexture("NPCs/Polterghast/PolterghastChain").Width * 0.5f, (float)mod.GetTexture("NPCs/Polterghast/PolterghastChain").Height * 0.5f), 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
				player.AddBuff(mod.BuffType("Horror"), 180, true);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
