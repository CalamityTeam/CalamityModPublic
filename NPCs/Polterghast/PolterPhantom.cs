using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
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
	[AutoloadBossHead]
	public class PolterPhantom : ModNPC
    {
        private int despawnTimer = 600;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polterghast");
            Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.damage = 210;
            npc.width = 90;
            npc.height = 120;
			npc.LifeMaxNERB(130000, 150000, 900000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.alpha = 255;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.Daybreak] = false;
			npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.BetsysCurse] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            npc.canGhostHeal = false;
			npc.dontTakeDamage = true;
            npc.HitSound = SoundID.NPCHit36;
            npc.DeathSound = SoundID.NPCDeath39;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(despawnTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            despawnTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            CalamityGlobalNPC.ghostBossClone = npc.whoAmI;

            npc.alpha -= 5;
            if (npc.alpha < 50)
                npc.alpha = 50;

            if (CalamityGlobalNPC.ghostBoss < 0 || !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            Lighting.AddLight((int)((npc.position.X + (npc.width / 2)) / 16f), (int)((npc.position.Y + (npc.height / 2)) / 16f), 0.5f, 0.25f, 0.75f);

            npc.TargetClosest(true);

            Vector2 vector = npc.Center;

            if (Vector2.Distance(Main.player[npc.target].Center, vector) > 6000f)
                npc.active = false;

			// Scale multiplier based on nearby active tiles
			float tileEnrageMult = Main.npc[CalamityGlobalNPC.ghostBoss].ai[3];

			bool speedBoost1 = false;
            bool despawnBoost = false;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            if (npc.timeLeft < 1500)
                npc.timeLeft = 1500;

            int[] array2 = new int[4];
            float num730 = 0f;
            float num731 = 0f;
            int num732 = 0;
            int num;
            for (int num733 = 0; num733 < 200; num733 = num + 1)
            {
                if (Main.npc[num733].active && Main.npc[num733].type == ModContent.NPCType<PolterghastHook>())
                {
                    num730 += Main.npc[num733].Center.X;
                    num731 += Main.npc[num733].Center.Y;
                    array2[num732] = num733;
                    num732++;
                    if (num732 > 3)
                        break;
                }
                num = num733;
            }
            num730 /= num732;
            num731 /= num732;

            float num734 = 3f;
            float num735 = 0.03f;
            if (!Main.player[npc.target].ZoneDungeon && !BossRushEvent.BossRushActive && Main.player[npc.target].position.Y < Main.worldSurface * 16.0)
            {
                despawnTimer--;
                if (despawnTimer <= 0)
                    despawnBoost = true;

                speedBoost1 = true;
                num734 += 8f;
                num735 = 0.15f;
            }
            else
                despawnTimer++;

            if (Main.npc[CalamityGlobalNPC.ghostBoss].ai[2] < 300f)
            {
                num734 = 21f;
                num735 = 0.13f;
            }

			if (expertMode)
			{
				num734 += revenge ? 5f : 3.5f;
				num735 += revenge ? 0.035f : 0.025f;
			}

			Vector2 vector91 = new Vector2(num730, num731);
            float num736 = Main.player[npc.target].Center.X - vector91.X;
            float num737 = Main.player[npc.target].Center.Y - vector91.Y;

            if (despawnBoost)
            {
                num737 *= -1f;
                num736 *= -1f;
                num734 += 8f;
            }

            float num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);
            int num739 = 500;
            if (speedBoost1)
                num739 += 500;
            if (expertMode)
                num739 += 150;

			// Increase speed based on nearby active tiles
			num734 *= tileEnrageMult;
			num735 *= tileEnrageMult;

			if (num738 >= num739)
            {
                num738 = num739 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            num730 += num736;
            num731 += num737;
            vector91 = new Vector2(vector.X, vector.Y);
            num736 = num730 - vector91.X;
            num737 = num731 - vector91.Y;
            num738 = (float)Math.Sqrt(num736 * num736 + num737 * num737);

            if (num738 < num734)
            {
                num736 = npc.velocity.X;
                num737 = npc.velocity.Y;
            }
            else
            {
                num738 = num734 / num738;
                num736 *= num738;
                num737 *= num738;
            }

            if (npc.velocity.X < num736)
            {
                npc.velocity.X += num735;
                if (npc.velocity.X < 0f && num736 > 0f)
                    npc.velocity.X += num735 * 2f;
            }
            else if (npc.velocity.X > num736)
            {
                npc.velocity.X -= num735;
                if (npc.velocity.X > 0f && num736 < 0f)
                    npc.velocity.X -= num735 * 2f;
            }
            if (npc.velocity.Y < num737)
            {
                npc.velocity.Y += num735;
                if (npc.velocity.Y < 0f && num737 > 0f)
                    npc.velocity.Y += num735 * 2f;
            }
            else if (npc.velocity.Y > num737)
            {
                npc.velocity.Y -= num735;
                if (npc.velocity.Y > 0f && num737 < 0f)
                    npc.velocity.Y -= num735 * 2f;
            }

            Vector2 vector92 = new Vector2(vector.X, vector.Y);
            float num740 = Main.player[npc.target].Center.X - vector92.X;
            float num741 = Main.player[npc.target].Center.Y - vector92.Y;
            npc.rotation = (float)Math.Atan2(num741, num740) + 1.57f;

            npc.damage = npc.defDamage;

            if (speedBoost1)
            {
                npc.damage *= 2;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return new Color(200, 150, 255, npc.alpha);
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
			int num153 = 7;

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

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 1.0;
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (npc.frame.Y > frameHeight * 3)
            {
                npc.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);

			player.AddBuff(BuffID.MoonLeech, 360, true);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            Dust.NewDust(npc.position, npc.width, npc.height, 180, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 90;
                npc.height = 90;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, (int)CalamityDusts.Phantoplasm, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 60; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 180, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
