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
    public class PlagueHomingMissile : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Homing Missile");
            Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.GetNPCDamage();
			npc.width = 22;
            npc.height = 22;
            npc.defense = 20;
            npc.lifeMax = BossRushEvent.BossRushActive ? 5000 : 500;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.noGravity = true;
            npc.canGhostHeal = false;
            npc.noTileCollide = true;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToElectricity = true;
		}

        public override void AI()
        {
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

			Lighting.AddLight(npc.Center, 0.015f, 0.1f, 0f);

            if (Math.Abs(npc.velocity.X) >= 3f || Math.Abs(npc.velocity.Y) >= 3f)
            {
                float num247 = 0f;
                float num248 = 0f;
                if (Main.rand.Next(2) == 1)
                {
                    num247 = npc.velocity.X * 0.5f;
                    num248 = npc.velocity.Y * 0.5f;
                }
                int num249 = Dust.NewDust(new Vector2(npc.position.X + 3f + num247, npc.position.Y + 3f + num248) - npc.velocity * 0.5f, npc.width - 8, npc.height - 8, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[num249].velocity *= 0.2f;
                Main.dust[num249].noGravity = true;
                num249 = Dust.NewDust(new Vector2(npc.position.X + 3f + num247, npc.position.Y + 3f + num248) - npc.velocity * 0.5f, npc.width - 8, npc.height - 8, 31, 0f, 0f, 100, default, 0.25f);
                Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num249].velocity *= 0.05f;
            }
            else if (Main.rand.NextBool(4))
            {
                int num252 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default, 0.5f);
                Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = npc.Center + new Vector2(0f, (float)(-(float)npc.height / 2)).RotatedBy((double)npc.rotation, default) * 1.1f;
                Main.rand.Next(2);
                num252 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = npc.Center + new Vector2(0f, (float)(-(float)npc.height / 2 - 6)).RotatedBy((double)npc.rotation, default) * 1.1f;
            }

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;

			float timeBeforeHoming = 120f - npc.ai[3] * 0.5f;
            if (npc.ai[2] < timeBeforeHoming)
            {
                npc.ai[2] += 1f;
                return;
            }

            if (npc.ai[0] == 0f && npc.ai[1] == 0f)
            {
                npc.ai[0] = 1f;
                npc.ai[1] = (float)Player.FindClosest(npc.position, npc.width, npc.height);
                npc.netUpdate = true;
                float num754 = npc.velocity.Length();
                npc.velocity = Vector2.Normalize(npc.velocity) * (num754 + 1f);
            }
            else if (npc.ai[0] == 1f)
            {
                npc.localAI[1] += 1f;
                float timeBeforeExploding = 480f + npc.ai[3] * 2f;
                float homingDuration = (malice ? 430f : death ? 340f : revenge ? 290f : expertMode ? 240f : 150f) + npc.ai[3] * 2f;
                if (npc.localAI[1] == timeBeforeExploding)
                {
                    CheckDead();
                    npc.life = 0;
                    return;
                }

                if (npc.localAI[1] < homingDuration)
                {
                    npc.noTileCollide = true;
                    Vector2 v3 = Main.player[(int)npc.ai[1]].Center - npc.Center;
                    float num760 = npc.velocity.ToRotation();
                    float num761 = v3.ToRotation();
					float angle = num761 - num760;
					angle = MathHelper.WrapAngle(angle);
                    npc.velocity = npc.velocity.RotatedBy(angle * 0.2);
                }
                else
                {
                    npc.noTileCollide = false;

					if (npc.velocity.Length() < (malice ? 25f : 20f))
						npc.velocity *= 1.01f;

					if (Collision.SolidCollision(npc.position, npc.width, npc.height))
					{
						CheckDead();
						npc.life = 0;
						return;
					}
                }
            }

			float distanceBeforeExploding = 42f;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead && Vector2.Distance(player.Center, npc.Center) <= distanceBeforeExploding)
                {
                    CheckDead();
                    npc.life = 0;
                    return;
                }
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
					vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[npc.type])) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/PlaguebringerGoliath/PlagueHomingMissileGlow");
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
					vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
					spriteBatch.Draw(texture2D15, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool CheckDead()
        {
            Main.PlaySound(SoundID.Item14, npc.position);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
            npc.width = npc.height = 216;
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }

		public override bool PreNPCLoot() => false;

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 180, true);
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
