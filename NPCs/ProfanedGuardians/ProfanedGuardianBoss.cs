using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.SummonItems;
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
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Events;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianBoss : ModNPC
    {
        private int spearType = 0;
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Commander");
            Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailingMode[npc.type] = 1;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 20f;
            npc.aiStyle = -1;
			npc.GetNPCDamage();
			npc.width = 100;
            npc.height = 80;
            npc.defense = 40;
			npc.DR_NERD(0.3f);
            npc.LifeMaxNERB(56250, 67500, 165000); // Old HP - 102500, 112500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            aiType = -1;
            npc.boss = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("Guardians") ?? MusicID.Boss1;
            npc.value = Item.buyPrice(1, 0, 0, 0);
            npc.HitSound = SoundID.NPCHit52;
            npc.DeathSound = SoundID.NPCDeath55;
			npc.Calamity().VulnerableToHeat = false;
			npc.Calamity().VulnerableToCold = true;
			npc.Calamity().VulnerableToSickness = false;
			npc.Calamity().VulnerableToWater = true;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(spearType);
            writer.Write(healTimer);
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spearType = reader.ReadInt32();
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
            CalamityGlobalNPC.doughnutBoss = npc.whoAmI;

			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			Vector2 vectorCenter = npc.Center;
			if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[1] == 0f)
            {
                npc.localAI[1] = 1f;
                NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianBoss2>());
                NPC.NewNPC((int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianBoss3>());
            }

            bool defenderAlive = false;
            bool healerAlive = false;
            if (CalamityGlobalNPC.doughnutBossDefender != -1)
            {
                if (Main.npc[CalamityGlobalNPC.doughnutBossDefender].active)
                    defenderAlive = true;
            }
            if (CalamityGlobalNPC.doughnutBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                    healerAlive = true;
            }

            // Defense
            if (defenderAlive)
            {
                npc.Calamity().DR = 0.9f;
                npc.Calamity().unbreakableDR = true;
                npc.Calamity().CurrentlyIncreasingDefenseOrDR = true;
            }
            else
            {
                npc.Calamity().DR = 0.3f;
                npc.Calamity().unbreakableDR = false;
                npc.Calamity().CurrentlyIncreasingDefenseOrDR = false;
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
                        int healAmt = npc.lifeMax / 20;
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

			Player player = Main.player[npc.target];

			if (!Main.dayTime || !player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!Main.dayTime || !player.active || player.dead)
                {
					if (npc.velocity.Y > 3f)
						npc.velocity.Y = 3f;
					npc.velocity.Y -= 0.1f;
					if (npc.velocity.Y < -12f)
						npc.velocity.Y = -12f;

					if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

					if (npc.ai[0] != 0f)
					{
						npc.ai[0] = 0f;
						npc.ai[1] = 0f;
						npc.ai[2] = 0f;
						npc.ai[3] = 0f;
						npc.netUpdate = true;
					}
					return;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

			// Become immune over time if target isn't in hell or hallow
			bool isHoly = player.ZoneHoly;
			bool isHell = player.ZoneUnderworldHeight;
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

			bool phase1 = false;
            for (int num569 = 0; num569 < Main.maxNPCs; num569++)
            {
                if ((Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss2>()) || (Main.npc[num569].active && Main.npc[num569].type == ModContent.NPCType<ProfanedGuardianBoss3>()))
                    phase1 = true;
            }

            float inertia = (malice || biomeEnraged) ? 45f : death ? 50f : revenge ? 52f : expertMode ? 55f : 60f;
            if (lifeRatio < 0.5f)
                inertia *= 0.8f;
            if (!phase1)
                inertia *= 0.8f;

            float num1006 = 0.111111117f * inertia;
            
            if (npc.ai[0] == 0f)
            {
                if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;
                    npc.direction = playerLocation < 0f ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }

                float velocity = (malice || biomeEnraged) ? 20f : death ? 18f : revenge ? 17f : expertMode ? 16f : 14f;
                Vector2 targetVector = player.Center - vectorCenter;
                targetVector = Vector2.Normalize(targetVector) * velocity;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                if (defenderAlive)
                    phaseGateValue *= 1.25f;

                if (npc.ai[3] < phaseGateValue || healerAlive)
                {
                    npc.velocity = (npc.velocity * (inertia - 1f) + targetVector) / inertia;
                    npc.ai[3] += 1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float divisor = (malice || biomeEnraged) ? 15f : death ? 20f : revenge ? 22f : expertMode ? 25f : 30f;
                        if (phase1)
                            divisor *= 2f;

                        if (npc.ai[3] % divisor == 0f)
                        {
                            Main.PlaySound(SoundID.Item20, npc.position);
                            int type = ModContent.ProjectileType<FlareDust>();
                            int damage = npc.GetProjectileDamage(type);
                            Vector2 projectileVelocity = Vector2.Normalize(player.Center - vectorCenter);

                            int numProj = death ? 3 : 2;
                            int spread = death ? 30 : 20;
                            if (!phase1)
                            {
                                numProj *= 2;
                                spread *= 2;
                            }

                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);
                                Projectile.NewProjectile(vectorCenter, normalizedPerturbedSpeed * npc.velocity.Length() * 1.25f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                    npc.ai[2] = targetVector.X;
                    npc.ai[3] = targetVector.Y;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (Math.Abs(npc.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;
                    npc.direction = playerLocation < 0f ? 1 : -1;
                    npc.spriteDirection = npc.direction;
                }

                npc.velocity *= 0.8f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 12f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]);
                    velocity.Normalize();
                    velocity *= (malice || biomeEnraged) ? 39f : death ? 33f : revenge ? 30f : expertMode ? 27f : 21f;
                    if (defenderAlive)
                        velocity *= 0.8f;

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
                    float projectileGateValue = (malice || biomeEnraged) ? 30f : death ? 35f : revenge ? 37f : expertMode ? 40f : 45f;
                    if (npc.localAI[0] >= projectileGateValue && Vector2.Distance(vectorCenter, player.Center) > 160f)
                    {
                        npc.localAI[0] = 0f;

                        Main.PlaySound(SoundID.Item20, npc.position);

                        int totalProjectiles = phase1 ? 6 : 10;
                        float velocity = phase1 ? 5f : 6f;
                        int type = ModContent.ProjectileType<ProfanedSpear>();
                        int damage = npc.GetProjectileDamage(type);

                        switch (spearType)
                        {
                            case 0:
                                break;
                            case 1:
                                totalProjectiles = phase1 ? 10 : 15;
                                velocity = phase1 ? 4f : 5f;
                                break;
                            case 2:
                                totalProjectiles = phase1 ? 5 : 7;
                                velocity = phase1 ? 6f : 7f;
                                break;
                            default:
                                break;
                        }

                        if (malice || biomeEnraged)
                            totalProjectiles *= 2;

                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                            Projectile.NewProjectile(npc.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }

                        spearType++;
                        if (spearType > 2)
                            spearType = 0;
                    }
                }

                npc.ai[1] += 1f;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
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
                    Vector2 targetVector = player.Center - vectorCenter;
                    targetVector.Normalize();
                    if (targetVector.HasNaNs())
                        targetVector = new Vector2(npc.direction, 0f);

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

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBossGlow");
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

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.SuperHealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			// Profaned Guardians have no actual drops and no treasure bag
			DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedGuardianMask>(), 7);
            DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedGuardianTrophy>(), 10);
            DropHelper.DropItemChance(npc, ModContent.ItemType<RelicOfDeliverance>(), 4);
			DropHelper.DropItemChance(npc, ModContent.ItemType<SamuraiBadge>(), 10);
			DropHelper.DropItem(npc, ModContent.ItemType<ProfanedCoreUnlimited>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeProfanedGuardians>(), true, !CalamityWorld.downedGuardians);

			// Mark the Profaned Guardians as dead
			CalamityWorld.downedGuardians = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<HolyFlames>(), 300, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
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
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA4"), 1f);
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
