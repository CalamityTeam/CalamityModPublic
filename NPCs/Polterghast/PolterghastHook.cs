using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
			NPCID.Sets.TrailingMode[npc.type] = 1;
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
            npc.HitSound = SoundID.NPCHit34;
            npc.DeathSound = SoundID.NPCDeath39;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2);
            writer.Write(despawnTimer);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2 = reader.ReadBoolean();
            despawnTimer = reader.ReadInt32();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
		}

        public override void AI()
        {
            // Emit light
            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0f, 0.3f, 0.3f);

            // Bools
            bool speedBoost = false;
            bool despawnBoost = false;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			// Despawn if Polter is gone
			if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

			Player player = Main.player[Main.npc[CalamityGlobalNPC.ghostBoss].target];

			if (!player.active || player.dead)
			{
				speedBoost = true;
				despawnBoost = true;
			}

			bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f;

			// Percent life remaining, Polter
			float lifeRatio = Main.npc[CalamityGlobalNPC.ghostBoss].life / (float)Main.npc[CalamityGlobalNPC.ghostBoss].lifeMax;

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

			// Despawn
			if (CalamityGlobalNPC.ghostBoss != -1 && !player.ZoneDungeon &&
                player.position.Y < Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                npc.localAI[0] -= 6f;
                speedBoost = true;
            }
            else
                despawnTimer++;

			// Phase 2
			bool phase3 = lifeRatio < (death ? 0.6f : revenge ? 0.5f : expertMode ? 0.35f : 0.2f);
			phase2 = lifeRatio < (death ? 0.9f : revenge ? 0.8f : expertMode ? 0.65f : 0.5f) && !phase3;
			if (phase2)
            {
				// Get a target
				if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
					npc.TargetClosest();

				// Despawn safety, make sure to target another player if the current player target is too far away
				if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
					npc.TargetClosest();

				Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);

                // Fire projectiles
                Vector2 vector17 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                float num147 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector17.X;
                float num148 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector17.Y;
                float num149 = (float)Math.Sqrt(num147 * num147 + num148 * num148);

                if (chargePhase)
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
                        float num151 = 10f * tileEnrageMult;
						int type = ModContent.ProjectileType<PhantomHookShot>();
						int damage = npc.GetProjectileDamage(type);
						num149 = num151 / num149;
                        num147 *= num149;
                        num148 *= num149;
                        int proj = Projectile.NewProjectile(vector17.X, vector17.Y, num147, num148, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                return;
            }

            // Phase 1 or 3
            Movement(phase2, expertMode, revenge, death, speedBoost, despawnBoost, lifeRatio, tileEnrageMult, player);
        }

        private void Movement(bool phase2, bool expertMode, bool revenge, bool death, bool speedBoost, bool despawnBoost, float lifeRatio, float tileEnrageMult, Player player)
        {
			bool chargePhase = Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] >= 420f;

			if (phase2)
            {
                float num740 = Main.player[npc.target].Center.X - npc.Center.X;
                float num741 = Main.player[npc.target].Center.Y - npc.Center.Y;
                npc.rotation = (float)Math.Atan2(num741, num740) + MathHelper.PiOver2;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f)
                    npc.ai[0] = (int)(npc.Center.X / 16f);
                if (npc.ai[1] == 0f)
                    npc.ai[1] = (int)(npc.Center.X / 16f);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.ai[0] == 0f || npc.ai[1] == 0f)
                    npc.localAI[0] = 0f;

				if (chargePhase)
				{
					npc.localAI[0] -= 10f;
				}
				else
				{
					float shootBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
					npc.localAI[0] -= 1f + shootBoost * tileEnrageMult;
					if (expertMode)
						npc.localAI[0] -= Vector2.Distance(npc.Center, player.Center) * 0.002f;
					if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] >= 300f)
						npc.localAI[0] -= 3f;
					if (speedBoost)
						npc.localAI[0] -= 6f;
				}

                if (!despawnBoost && npc.localAI[0] <= 0f && npc.ai[0] != 0f)
                {
                    for (int num763 = 0; num763 < Main.maxNPCs; num763++)
                    {
                        if (num763 != npc.whoAmI && Main.npc[num763].active && Main.npc[num763].type == npc.type && (Main.npc[num763].velocity.X != 0f || Main.npc[num763].velocity.Y != 0f))
                            npc.localAI[0] = 180f;
                    }
                }

                if (npc.localAI[0] <= 0f)
                {
                    npc.localAI[0] = 450f;
                    bool flag50 = false;
                    int num764 = 0;
                    while (!flag50 && num764 <= 1000)
                    {
                        num764++;
                        int num765 = (int)(player.Center.X / 16f);
                        int num766 = (int)(player.Center.Y / 16f);
                        if (npc.ai[0] == 0f)
                        {
                            num765 = (int)((player.Center.X + Main.npc[CalamityGlobalNPC.ghostBoss].Center.X) / 32f);
                            num766 = (int)((player.Center.Y + Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y) / 32f);
                        }
                        if (despawnBoost)
                        {
                            num765 = (int)Main.npc[CalamityGlobalNPC.ghostBoss].position.X / 16;
                            num766 = (int)(Main.npc[CalamityGlobalNPC.ghostBoss].position.Y + 400f) / 16;
                        }
                        int num767 = 20;
                        num767 += (int)(100f * (num764 / 1000f));
                        int num768 = num765 + Main.rand.Next(-num767, num767 + 1);
                        int num769 = num766 + Main.rand.Next(-num767, num767 + 1);
                        try
                        {
                            if (WorldGen.SolidTile(num768, num769) || Main.tile[num768, num769].wall > 0 || chargePhase)
                            {
                                flag50 = true;
                                npc.ai[0] = num768;
                                npc.ai[1] = num769;
								npc.localAI[1] = Vector2.Distance(npc.Center, player.Center) * 0.01f;
								npc.netUpdate = true;
                            }
                        } catch
                        {
                        }
                    }
                }
            }

            if (npc.ai[0] > 0f && npc.ai[1] > 0f)
            {
				float velocityBoost = death ? 4f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                float velocity = (8f + velocityBoost) * tileEnrageMult;
                if (expertMode)
                    velocity += npc.localAI[1];
                if (revenge)
                    velocity += 1f;
                if (speedBoost)
                    velocity *= 2f;
                if (despawnBoost)
                    velocity *= 2f;

                Vector2 vector95 = new Vector2(npc.Center.X, npc.Center.Y);
                float num773 = npc.ai[0] * 16f - 8f - vector95.X;
                float num774 = npc.ai[1] * 16f - 8f - vector95.Y;
                float num775 = (float)Math.Sqrt(num773 * num773 + num774 * num774);
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
                    npc.rotation = (float)Math.Atan2(num777, num776) - 1.57f;
                }
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Color lightRed = new Color(255, 100, 100, 255);
			if (Main.npc[CalamityGlobalNPC.ghostBoss].active && !phase2)
			{
				Vector2 center = npc.Center;
				float bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
				float bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;
				float rotation2 = (float)Math.Atan2(bossCenterY, bossCenterX) - 1.57f;
				bool draw = true;
				while (draw)
				{
					int chainWidth = 20; //16 24
					int chainHeight = 52; //32 16
					float num10 = (float)Math.Sqrt(bossCenterX * bossCenterX + bossCenterY * bossCenterY);
					if (num10 < chainHeight)
					{
						chainWidth = (int)num10 - chainHeight + chainWidth;
						draw = false;
					}
					num10 = chainWidth / num10;
					bossCenterX *= num10;
					bossCenterY *= num10;
					center.X += bossCenterX;
					center.Y += bossCenterY;
					bossCenterX = Main.npc[CalamityGlobalNPC.ghostBoss].Center.X - center.X;
					bossCenterY = Main.npc[CalamityGlobalNPC.ghostBoss].Center.Y - center.Y;

					Color color2 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
					if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
						color2 = Color.Lerp(color2, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

					Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastChain"), new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y),
						new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastChain").Width, chainWidth)), color2, rotation2,
						new Vector2(ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastChain").Width * 0.5f, ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastChain").Height * 0.5f), 1f, SpriteEffects.None, 0f);
				}
			}

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num155 = 1; num155 < num153; num155 += 2)
				{
					Color color38 = lightColor;
					color38 = Color.Lerp(color38, color36, amount9);
					color38 = npc.GetAlpha(color38);
					color38 *= (num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Polterghast/PolterghastHookGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

			if (Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] > 300f)
				color37 = Color.Lerp(color37, lightRed, MathHelper.Clamp((Main.npc[CalamityGlobalNPC.ghostBoss].Calamity().newAI[0] - 300f) / 120f, 0f, 1f));

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
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

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
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
