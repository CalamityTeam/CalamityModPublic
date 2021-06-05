using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.PlaguebringerGoliath
{
    public class PlagueBeeG : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Charger");
            Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.width = 36;
            npc.height = 30;
            npc.defense = 15;
            npc.scale = 0.75f;
            npc.lifeMax = 300;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 5000;
            }
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = BossRushEvent.BossRushActive ? 0f : 0.9f;
            animationType = NPCID.Bee;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || CalamityWorld.malice;
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.015f, 0.1f, 0f);
            if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest();
            }
            float num = revenge ? 8f : 7f;
            float num2 = revenge ? 0.2f : 0.15f;
            if (BossRushEvent.BossRushActive || CalamityWorld.malice)
            {
                num *= 1.5f;
                num2 *= 1.5f;
            }

            npc.localAI[0] += 1f;
            float num3 = (npc.localAI[0] - 60f) / 60f;
            if (num3 > 1f)
            {
                num3 = 1f;
            }
            else
            {
                if (npc.velocity.X > 6f)
                {
                    npc.velocity.X = 6f;
                }
                if (npc.velocity.X < -6f)
                {
                    npc.velocity.X = -6f;
                }
                if (npc.velocity.Y > 6f)
                {
                    npc.velocity.Y = 6f;
                }
                if (npc.velocity.Y < -6f)
                {
                    npc.velocity.Y = -6f;
                }
            }
            num2 *= num3;
            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            npc.ai[0] += 1f;
            if (npc.ai[0] > 0f)
            {
                npc.velocity.Y = npc.velocity.Y + 0.023f;
            }
            else
            {
                npc.velocity.Y = npc.velocity.Y - 0.023f;
            }
            if (npc.ai[0] < -100f || npc.ai[0] > 100f)
            {
                npc.velocity.X = npc.velocity.X + 0.023f;
            }
            else
            {
                npc.velocity.X = npc.velocity.X - 0.023f;
            }
            if (npc.ai[0] > 200f)
            {
                npc.ai[0] = -200f;
            }
            if (Main.player[npc.target].dead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X = npc.velocity.X + num2;
                if (npc.velocity.X < 0f && num4 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num2;
                }
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X = npc.velocity.X - num2;
                if (npc.velocity.X > 0f && num4 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num2;
                }
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y = npc.velocity.Y + num2;
                if (npc.velocity.Y < 0f && num5 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num2;
                }
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y = npc.velocity.Y - num2;
                if (npc.velocity.Y > 0f && num5 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num2;
                }
            }
            npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
            float num12 = 0.7f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -num12;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -num12;
                if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                {
                    npc.velocity.Y = -2f;
                }
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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/NormalNPCs/PlagueBeeGlow");
			Color color37 = Color.Lerp(Color.White, Color.Red, 0.5f);

			if (CalamityConfig.Instance.Afterimages)
			{
				for (int num163 = 1; num163 < num153; num163++)
				{
					Color color41 = color37;
					color41 = Color.Lerp(color41, color36, amount9);
					color41 *= (float)(num153 - num163) / 15f;
					Vector2 vector44 = npc.oldPos[num163] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
					vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
					vector44 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool PreNPCLoot()
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Plague, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
