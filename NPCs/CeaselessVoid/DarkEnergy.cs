using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.CeaselessVoid
{
	public class DarkEnergy : ModNPC
    {
        public int invinceTime = 120;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Energy");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.dontTakeDamage = true;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 50;
            npc.lifeMax = 6000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0 || !CalamityWorld.downedSentinel1)
            {
                npc.lifeMax = 24000;
            }
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 44000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0.25f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.HitSound = SoundID.NPCHit53;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(invinceTime);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            invinceTime = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow2");
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
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
					color38 *= (float)(num153 - num155) / 15f;
					Vector2 vector41 = npc.oldPos[num155] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector41 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/CeaselessVoid/DarkEnergyGlow");
			Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);
			Color color42 = Color.Lerp(Color.White, Color.Fuchsia, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 = npc.GetAlpha(color41);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (float)(num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            if (invinceTime > 0)
            {
				npc.damage = 0;
                invinceTime--;
            }
            else
            {
                npc.damage = npc.defDamage;
                npc.dontTakeDamage = false;
            }

			if (CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
			{
				npc.active = false;
				npc.netUpdate = true;
				return;
			}

			Vector2 vectorCenter = npc.Center;
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            double mult = 0.5 +
                (CalamityWorld.revenge ? 0.2 : 0.0) +
                (CalamityWorld.death ? 0.2 : 0.0);

            if (npc.life < npc.lifeMax * mult || BossRushEvent.BossRushActive)
                npc.knockBackResist = 0f;

			float tileEnrageMult = Main.npc[CalamityGlobalNPC.voidBoss].ai[1];

			float num1247 = 0.1f;
			float maxDistance = 24f * MathHelper.Lerp(0.3333333334f, 1.6f, MathHelper.Clamp((tileEnrageMult - 1f) * 1.6666666667f, 0f, 1f));
			for (int num1248 = 0; num1248 < Main.maxNPCs; num1248++)
			{
				if (Main.npc[num1248].active)
				{
					if (num1248 != npc.whoAmI && Main.npc[num1248].type == npc.type)
					{
						if (Vector2.Distance(npc.Center, Main.npc[num1248].Center) < maxDistance)
						{
							if (npc.position.X < Main.npc[num1248].position.X)
								npc.velocity.X = npc.velocity.X - num1247;
							else
								npc.velocity.X = npc.velocity.X + num1247;

							if (npc.position.Y < Main.npc[num1248].position.Y)
								npc.velocity.Y = npc.velocity.Y - num1247;
							else
								npc.velocity.Y = npc.velocity.Y + num1247;
						}
					}
				}
			}

			if (npc.ai[1] == 0f)
            {
                npc.scale -= 0.01f;
                npc.alpha += 15;
                if (npc.alpha >= 125)
                {
                    npc.alpha = 130;
                    npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.scale += 0.01f;
                npc.alpha -= 15;
                if (npc.alpha <= 0)
                {
                    npc.alpha = 0;
                    npc.ai[1] = 0f;
                }
            }

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);

                    if (npc.timeLeft > 150)
                        npc.timeLeft = 150;

                    return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            if (npc.ai[0] == 0f)
            {
                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num784 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector96.X;
                float num785 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector96.Y;
                float num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
                if (num786 > 90f)
                {
                    num786 = (BossRushEvent.BossRushActive ? 24f : 16f) * tileEnrageMult / num786;
                    num784 *= num786;
                    num785 *= num786;
                    npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
                    npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
                    return;
                }
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 16f)
                {
                    npc.velocity.Y = npc.velocity.Y * 1.1f;
                    npc.velocity.X = npc.velocity.X * 1.1f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && ((expertMode && Main.rand.NextBool(50)) || Main.rand.NextBool(100)))
                {
                    npc.TargetClosest(true);
                    vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                    num784 = player.Center.X - vector96.X;
                    num785 = player.Center.Y - vector96.Y;
                    num786 = (float)Math.Sqrt((double)(num784 * num784 + num785 * num785));
                    num786 = (BossRushEvent.BossRushActive ? 16f : 12f) * tileEnrageMult / num786;
                    npc.velocity.X = num784 * num786;
                    npc.velocity.Y = num785 * num786;
                    npc.ai[0] = 1f;
                    npc.netUpdate = true;
                }
            }
            else
            {
                Vector2 value4 = player.Center - npc.Center;
                value4.Normalize();
                value4 *= (BossRushEvent.BossRushActive ? 16f : 11f) * tileEnrageMult;
                npc.velocity = (npc.velocity * 99f + value4) / 100f;
                Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
                float num787 = Main.npc[CalamityGlobalNPC.voidBoss].Center.X - vector97.X;
                float num788 = Main.npc[CalamityGlobalNPC.voidBoss].Center.Y - vector97.Y;
                float num789 = (float)Math.Sqrt((double)(num787 * num787 + num788 * num788));
                npc.ai[2] += 1f;
                if (num789 > 700f || npc.ai[2] >= 150f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[2] = 0f;
                }
            }
        }

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.5f * bossLifeScale);
		}

		public override bool CheckActive()
		{
			return CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active;
		}

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(BuffID.VortexDebuff, 20, true);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.PurpleCosmolite, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
