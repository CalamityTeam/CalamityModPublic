using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Weapons.Typeless;
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
    public class ProfanedGuardianBoss2 : ModNPC
    {
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Defender");
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
            npc.defense = 50;
			npc.DR_NERD(0.4f);
            npc.LifeMaxNERB(43750, 52500, 30000); // Old HP - 40000, 50000
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
            writer.Write(healTimer);
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            healTimer = reader.ReadInt32();
            biomeEnrageTimer = reader.ReadInt32();
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
            CalamityGlobalNPC.doughnutBossDefender = npc.whoAmI;

            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }

            bool healerAlive = false;
            if (CalamityGlobalNPC.doughnutBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                    healerAlive = true;
            }

            // Healing
            if (healerAlive)
            {
                float healGateValue = 60f;
                healTimer++;
                if (healTimer >= healGateValue)
                {
                    healTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = npc.lifeMax / 25;
                        if (healAmt > npc.lifeMax - npc.life)
                            healAmt = npc.lifeMax - npc.life;

                        if (healAmt > 0)
                        {
                            npc.life += healAmt;
                            npc.HealEffect(healAmt, true);
                            npc.netUpdate = true;
                        }
                    }
                }
            }

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[Main.npc[CalamityGlobalNPC.doughnutBoss].target];

			if (npc.timeLeft < 1800)
				npc.timeLeft = 1800;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
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

            bool biomeEnraged = biomeEnrageTimer <= 0;

            if (npc.ai[0] == 0f)
            {
                if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0f ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }

                npc.ai[3] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    float divisor = (malice || biomeEnraged) ? 30f : death ? 40f : revenge ? 45f : expertMode ? 50f : 60f;
                    if (npc.ai[3] % divisor == 0f)
                    {
                        Main.PlaySound(SoundID.Item20, npc.position);
                        int type = ModContent.ProjectileType<FlareDust>();
                        int damage = npc.GetProjectileDamage(type);
                        Projectile.NewProjectile(npc.Center, Vector2.Normalize(player.Center - npc.Center) * npc.velocity.Length() * 1.25f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                    }
                }

                Vector2 targetVector = player.Center - npc.Center;
                float phaseGateValue = (malice || biomeEnraged) ? 120f : death ? 160f : revenge ? 180f : expertMode ? 200f : 240f;
                if (npc.ai[3] >= phaseGateValue)
                {
                    npc.ai[0] = 1f;
                    npc.ai[2] = targetVector.X;
                    npc.ai[3] = targetVector.Y;
                    npc.netUpdate = true;
                }

                Vector2 vector96 = new Vector2(npc.Center.X, npc.Center.Y);
                float num784 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.X - vector96.X;
                float num785 = Main.npc[CalamityGlobalNPC.doughnutBoss].Center.Y - vector96.Y;
                float num786 = (float)Math.Sqrt(num784 * num784 + num785 * num785);

                if (num786 > 320f)
                {
                    num786 = (Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 3f) / num786;
                    num784 *= num786;
                    num785 *= num786;
                    npc.velocity.X = (npc.velocity.X * 25f + num784) / 26f;
                    npc.velocity.Y = (npc.velocity.Y * 25f + num785) / 26f;
                    return;
                }

                if (npc.velocity.Length() < Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 3f)
                    npc.velocity *= 1.1f;
            }
            else if (npc.ai[0] == 1f)
            {
                if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = npc.Center.X - player.Center.X;
                    npc.direction = playerLocation < 0f ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }

                npc.velocity *= 0.8f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 18f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]);
                    velocity.Normalize();
                    velocity *= (malice || biomeEnraged) ? 32f : death ? 27f : revenge ? 24.5f : expertMode ? 22f : 17f;
                    npc.velocity = velocity;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (Math.Sign(npc.velocity.X) != 0)
                    npc.spriteDirection = -Math.Sign(npc.velocity.X);
                npc.spriteDirection = Math.Sign(npc.velocity.X);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[0] += 1f;
                    float projectileGateValue = (malice || biomeEnraged) ? 30f : death ? 40f : revenge ? 45f : expertMode ? 50f : 60f;
                    if (npc.localAI[0] >= projectileGateValue && Vector2.Distance(npc.Center, player.Center) > 160f)
                    {
                        npc.localAI[0] = 0f;

                        Main.PlaySound(SoundID.Item20, npc.position);

                        float velocity = 5f;
                        int totalProjectiles = 6;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = ModContent.ProjectileType<ProfanedSpear>();
                        int damage = npc.GetProjectileDamage(type);
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                            Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }

                npc.ai[1] += 1f;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                if (npc.ai[1] >= phaseGateValue)
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 24f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.velocity /= 2f;
                    npc.netUpdate = true;
                }
                else
                {
                    Vector2 targetVector = player.Center - npc.Center;
                    targetVector.Normalize();
                    if (targetVector.HasNaNs())
                        targetVector = new Vector2(npc.direction, 0f);

                    float inertia = (malice || biomeEnraged) ? 35f : death ? 40f : revenge ? 42f : expertMode ? 45f : 50f;
                    float num1006 = 0.111111117f * inertia;
                    npc.velocity = (npc.velocity * (inertia - 1f) + targetVector * (npc.velocity.Length() + num1006)) / inertia;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                if (Math.Sign(npc.velocity.X) != 0)
                    npc.spriteDirection = -Math.Sign(npc.velocity.X);
                npc.spriteDirection = Math.Sign(npc.velocity.X);

                npc.ai[1] -= 1f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                }

                npc.velocity *= 0.9f;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);
			Color color36 = Color.White;
			float amount9 = 0.5f;
			int num153 = 5;
            if (npc.ai[0] == 2f)
                num153 = 10;

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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBoss2Glow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

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

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<RelicOfResilience>(), 4);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.GreaterHealingPotion;
        }

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(ModContent.BuffType<HolyFlames>(), 240, true);
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossT3"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
