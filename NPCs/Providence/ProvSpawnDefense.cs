using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Providence
{
	public class ProvSpawnDefense : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("A Profaned Guardian");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
            npc.npcSlots = 1f;
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 100;
            npc.height = 80;
            npc.defense = 50;
			npc.DR_NERD(0.4f);
            npc.lifeMax = 25000;
            if (BossRushEvent.BossRushActive)
            {
                npc.lifeMax = 300000;
            }
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            aiType = -1;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[BuffID.StardustMinionBleed] = false;
			npc.buffImmune[BuffID.Oiled] = false;
            npc.buffImmune[BuffID.BetsysCurse] = false;
            npc.buffImmune[ModContent.BuffType<AstralInfectionDebuff>()] = false;
            npc.buffImmune[ModContent.BuffType<AbyssalFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<ArmorCrunch>()] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = false;
            npc.buffImmune[ModContent.BuffType<Nightwither>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;
            npc.buffImmune[ModContent.BuffType<WarCleave>()] = false;
            npc.buffImmune[ModContent.BuffType<WhisperingDeath>()] = false;
            npc.buffImmune[ModContent.BuffType<SilvaStun>()] = false;
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.holyBossDefender = npc.whoAmI;
            bool expertMode = Main.expertMode;
            Vector2 vectorCenter = npc.Center;
            Player player = Main.player[npc.target];
            npc.TargetClosest(false);
            if (CalamityGlobalNPC.holyBoss < 0 || !Main.npc[CalamityGlobalNPC.holyBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            npc.dontTakeDamage = Main.npc[CalamityGlobalNPC.holyBoss].dontTakeDamage;

            if (Math.Sign(npc.velocity.X) != 0)
            {
                npc.spriteDirection = -Math.Sign(npc.velocity.X);
            }
            npc.spriteDirection = Math.Sign(npc.velocity.X);
            int num1009 = (npc.ai[0] == 0f) ? 1 : 2;
            int num1010 = (npc.ai[0] == 0f) ? 60 : 80;
            for (int num1011 = 0; num1011 < 2; num1011++)
            {
                if (Main.rand.Next(3) < num1009)
                {
                    int dustType = Main.rand.Next(2);
                    if (dustType == 0)
                    {
                        dustType = (int)CalamityDusts.ProfanedFire;
                    }
                    else
                    {
                        dustType = 160;
                    }
                    int num1012 = Dust.NewDust(npc.Center - new Vector2((float)num1010), num1010 * 2, num1010 * 2, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 0.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
            npc.TargetClosest(true);
            float num1372 = 9f;
            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[1] -= 1f;
            if (num1375 < 200f || npc.ai[1] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[1] = 20f;
                }
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss2Glow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

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
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
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
			player.AddBuff(ModContent.BuffType<HolyFlames>(), 300, true);
		}

		public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT3"), 1f);
                for (int k = 0; k < 30; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
