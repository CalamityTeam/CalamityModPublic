using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss3 : ModNPC
    {
		private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Guardian");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 3f;
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 100;
            npc.height = 80;
            npc.defense = 30;
			npc.DR_NERD(0.2f);
            npc.LifeMaxNERB(21875, 26250, 20000); // Old HP - 25000, 35000
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            aiType = -1;
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToWater = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(npc.dontTakeDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			biomeEnrageTimer = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
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
			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool isHoly = player.ZoneHoly;
			bool isHell = player.ZoneUnderworldHeight;

            // Become immune over time if target isn't in hell or hallow
            if (!isHoly && !isHell && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
					biomeEnrageTimer--;
                else
                    npc.Calamity().CurrentlyEnraged = true;
            }
            else
				biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            // Take damage or not
            npc.dontTakeDamage = biomeEnrageTimer <= 0 && !malice;

			Vector2 vectorCenter = npc.Center;

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
                        dustType = 107;
                    }
                    int num1012 = Dust.NewDust(npc.Center - new Vector2(num1010), num1010 * 2, num1010 * 2, dustType, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                    Main.dust[num1012].noGravity = true;
                    Main.dust[num1012].velocity *= 0.2f;
                    Main.dust[num1012].fadeIn = 1f;
                }
            }
            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            if (npc.ai[0] == 0f)
            {
                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num784 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector96.X;
                float num785 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector96.Y;
                float num786 = (float)Math.Sqrt(num784 * num784 + num785 * num785);
                if (num786 > 90f)
                {
                    num786 = 21f / num786; //8f
                    num784 *= num786;
                    num785 *= num786;
                    npc.velocity.X = (npc.velocity.X * 15f + num784) / 16f;
                    npc.velocity.Y = (npc.velocity.Y * 15f + num785) / 16f;
                    return;
                }
                if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < 21f) //8f
                {
                    npc.velocity *= 1.12f; //1.05f
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && ((expertMode && Main.rand.NextBool(50)) || Main.rand.NextBool(100)))
                {
                    npc.TargetClosest();
                    vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                    num784 = player.Center.X - vector96.X;
                    num785 = player.Center.Y - vector96.Y;
                    num786 = (float)Math.Sqrt(num784 * num784 + num785 * num785);
                    num786 = 21f / num786; //8f
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
                value4 *= 22f; //9f
                npc.velocity = (npc.velocity * 99f + value4) / 100f;
                Vector2 vector97 = new Vector2(npc.Center.X, npc.Center.Y);
                float num787 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector97.X;
                float num788 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector97.Y;
                float num789 = (float)Math.Sqrt(num787 * num787 + num788 * num788);
                if (num789 > 700f)
                {
                    npc.ai[0] = 0f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += expertMode ? 2f : 1f;
                if (npc.localAI[0] >= 600f)
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);

						float velocity = 5f;
						int totalProjectiles = 6;
						float radians = MathHelper.TwoPi / totalProjectiles;
						for (int i = 0; i < totalProjectiles; i++)
						{
							Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
							Projectile.NewProjectile(npc.Center, vector255, ModContent.ProjectileType<HealOrbProv>(), 0, 0f, Main.myPlayer, npc.target, -32);
						}
                    }
                }
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Texture2D texture2D16 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss3Glow2");
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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss3Glow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
			Color color42 = Color.Lerp(Color.White, Color.Violet, 0.5f);

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

					Color color43 = color42;
					color43 = Color.Lerp(color43, color36, amount9);
					color43 = npc.GetAlpha(color43);
					color43 *= (num153 - num163) / 15f;
					spriteBatch.Draw(texture2D16, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
				}
			}

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			spriteBatch.Draw(texture2D16, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<RelicOfConvergence>(), 4);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<HolyFlames>(), 180, true);
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH4"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossH5"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
