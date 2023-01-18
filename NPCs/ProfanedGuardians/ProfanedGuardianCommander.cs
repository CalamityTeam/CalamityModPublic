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
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Events;
using Terraria.Audio;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianCommander : ModNPC
    {
        private int spearType = 0;
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian Commander");
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionXOverride = 0,
                PortraitScale = 0.75f,
                Scale = 0.75f
            };
            value.Position.X += 25;
            value.Position.Y += 15;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 20f;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 228;
            NPC.height = 186;
            NPC.defense = 40;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(56250, 67500, 165000); // Old HP - 102500, 112500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            AIType = -1;
            NPC.boss = true;
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("When it turns its burning spear towards anything, its simple mind has a clear goal. To entirely eradicate the enemy.")
            });
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
            NPC.frameCounter += 0.12f + NPC.velocity.Length() / 120f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.doughnutBoss = NPC.whoAmI;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.005f;

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            Vector2 vectorCenter = NPC.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[1] == 0f)
            {
                NPC.localAI[1] = 1f;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianDefender>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)vectorCenter.X, (int)vectorCenter.Y, ModContent.NPCType<ProfanedGuardianHealer>());
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
                NPC.Calamity().DR = 0.9f;
                NPC.Calamity().unbreakableDR = true;
                NPC.Calamity().CurrentlyIncreasingDefenseOrDR = true;
            }
            else
            {
                NPC.Calamity().DR = 0.3f;
                NPC.Calamity().unbreakableDR = false;
                NPC.Calamity().CurrentlyIncreasingDefenseOrDR = false;
            }

            // Healing
            if (healerAlive)
            {
                float distanceFromHealer = Vector2.Distance(Main.npc[CalamityGlobalNPC.doughnutBossHealer].Center, NPC.Center);
                bool dontHeal = distanceFromHealer > 2000f || Main.npc[CalamityGlobalNPC.doughnutBossHealer].justHit || NPC.life == NPC.lifeMax;
                if (dontHeal)
                {
                    healTimer = 0;
                }
                else
                {
                    float healGateValue = 60f;
                    healTimer++;
                    if (healTimer >= healGateValue)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                        int maxHealDustIterations = (int)distanceFromHealer;
                        int maxDust = 100;
                        int dustDivisor = maxHealDustIterations / maxDust;
                        if (dustDivisor < 2)
                            dustDivisor = 2;

                        Vector2 dustLineStart = Main.npc[CalamityGlobalNPC.doughnutBossHealer].Center;
                        Vector2 dustLineEnd = NPC.Center;
                        Vector2 currentDustPos = default;
                        Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                        Vector2 value5 = new Vector2(2.1f, 2f);
                        int dustSpawned = 0;
                        for (int i = 0; i < maxHealDustIterations; i++)
                        {
                            if (i % dustDivisor == 0)
                            {
                                currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxHealDustIterations);
                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(new Color(255, 200, Math.Abs(Main.DiscoB - (int)(dustSpawned * 2.55f)))).X, 1f, 0.5f);
                                dustColor.A = 255;
                                int dust = Dust.NewDust(currentDustPos, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = currentDustPos;
                                Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxHealDustIterations) * value5 * (0.8f + Main.rand.NextFloat() * 0.4f) + NPC.velocity;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1f;
                                Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                Dust dust2 = Dust.CloneDust(dust);
                                Dust dust3 = dust2;
                                dust3.scale /= 2f;
                                dust3 = dust2;
                                dust3.fadeIn /= 2f;
                                dust2.color = new Color(255, 255, 255, 255);
                                dustSpawned++;
                            }
                        }

                        healTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int healAmt = NPC.lifeMax / 20;
                            if (healAmt > NPC.lifeMax - NPC.life)
                                healAmt = NPC.lifeMax - NPC.life;

                            if (healAmt > 0)
                            {
                                NPC.life += healAmt;
                                NPC.HealEffect(healAmt, true);
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].ai[0] == 599 && Main.getGoodWorld && Main.netMode != NetmodeID.MultiplayerClient) // move to zenith seed later
                {
                    // gain more health once the healer's channel heal is done
                    NPC.lifeMax += 7500;
                    NPC.life += NPC.lifeMax - NPC.life;
                    NPC.HealEffect(NPC.lifeMax - NPC.life, true);
                    NPC.netUpdate = true;
                }
            }

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if ((!Main.dayTime && !Main.getGoodWorld) || !player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if ((!Main.dayTime && !Main.getGoodWorld) || !player.active || player.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Become immune over time if target isn't in hell or hallow
            bool isHoly = player.ZoneHallow;
            bool isHell = player.ZoneUnderworldHeight;
            if (!isHoly && !isHell && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
                else
                    NPC.Calamity().CurrentlyEnraged = true;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0;

            bool phase1 = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if ((Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ProfanedGuardianDefender>()) || (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ProfanedGuardianHealer>()))
                    phase1 = true;
            }

            float inertia = (bossRush || biomeEnraged) ? 45f : death ? 50f : revenge ? 52f : expertMode ? 55f : 60f;
            if (lifeRatio < 0.5f)
                inertia *= 0.8f;
            if (!phase1)
                inertia *= 0.8f;
            if (Main.getGoodWorld)
                inertia *= 0.8f;

            float num1006 = 0.111111117f * inertia;

            if (NPC.ai[0] == 0f)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                float velocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;

                Vector2 targetVector = player.Center - vectorCenter;
                targetVector = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;
                float phaseGateValue = (bossRush || biomeEnraged) ? 50f : death ? 66f : revenge ? 75f : expertMode ? 83f : 100f;
                if (defenderAlive)
                    phaseGateValue *= 1.5f;

                if (NPC.ai[3] < phaseGateValue || healerAlive)
                {
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector) / inertia;
                    NPC.ai[3] += 1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float divisor = (bossRush || biomeEnraged) ? 30f : death ? 40f : revenge ? 44f : expertMode ? 50f : 60f;
                        if (Main.getGoodWorld && healerAlive) // move to zenith seed later
                            divisor += 30;
                        if (!phase1)
                            divisor = (float)Math.Round(divisor * 0.8f);

                        if (NPC.ai[3] % divisor == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                            /*int type = ModContent.ProjectileType<FlareDust>();
                            if (Main.getGoodWorld && Main.rand.NextBool(4)) // move to zenith seed later
                                type = ModContent.ProjectileType<HolyFlare>();

                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 projectileVelocity = Vector2.Normalize(player.Center - vectorCenter);
                            int numProj = death ? 3 : 2;
                            int spread = !phase1 ? 15 : death ? 30 : 20;

                            if (Main.getGoodWorld)
                            {
                                numProj *= 2;
                                spread *= 3;
                            }

                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Vector2 normalizedPerturbedSpeed = Vector2.Normalize(perturbedSpeed);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), vectorCenter, normalizedPerturbedSpeed * NPC.velocity.Length() * 1.25f, type, damage, 0f, Main.myPlayer, 3f, 0f);
                            }*/
                        }
                    }
                }
                else
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[2] = targetVector.X;
                    NPC.ai[3] = targetVector.Y;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                NPC.velocity *= 0.8f;
                if (Main.getGoodWorld)
                    NPC.velocity *= 0.5f;

                float chargeGateValue = 12f;
                if (Main.getGoodWorld)
                    chargeGateValue *= 0.25f;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= chargeGateValue)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                    velocity.SafeNormalize(new Vector2(NPC.direction, 0f));
                    velocity *= (bossRush || biomeEnraged) ? 32f : death ? 28f : revenge ? 26f : expertMode ? 24f : 20f;
                    if (defenderAlive)
                        velocity *= 0.8f;
                    if (Main.getGoodWorld)
                        velocity *= 1.15f;

                    NPC.velocity = velocity;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] += 1f;
                    float projectileGateValue = (bossRush || biomeEnraged) ? 30f : death ? 35f : revenge ? 37f : expertMode ? 40f : 45f;
                    if (NPC.localAI[0] >= projectileGateValue && Vector2.Distance(vectorCenter, player.Center) > 160f)
                    {
                        NPC.localAI[0] = 0f;

                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                        /*int totalProjectiles = phase1 ? 6 : 10;
                        float velocity = phase1 ? 5f : 6f;
                        int type = ModContent.ProjectileType<ProfanedSpear>();
                        int damage = NPC.GetProjectileDamage(type);

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

                        if (bossRush || biomeEnraged)
                            totalProjectiles *= 2;

                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }

                        spearType++;
                        if (spearType > 2)
                            spearType = 0;*/
                    }
                }

                NPC.ai[1] += 1f;
                float phaseGateValue = (bossRush || biomeEnraged) ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
                if (NPC.ai[1] >= phaseGateValue)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = phase1 ? 24f : 6f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 targetVector = player.Center - vectorCenter;
                    targetVector.SafeNormalize(new Vector2(NPC.direction, 0f));
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector * (NPC.velocity.Length() + num1006)) / inertia;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] -= 1f;
                if (NPC.ai[1] <= 0f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }

                NPC.velocity *= phase1 ? 0.9f : 0.66f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;
            if (NPC.ai[0] == 2f)
                num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommanderGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            if (Main.getGoodWorld)
            {
                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommanderGlowNight").Value;
                color37 = Color.Cyan;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "A Profaned Guardian";
            potionType = ItemID.SuperHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<RelicOfDeliverance>(), 4);
            npcLoot.Add(ModContent.ItemType<ProfanedGuardianMask>(), 7);
            npcLoot.Add(ModContent.ItemType<WarbanneroftheSun>(), 10);
            npcLoot.Add(ModContent.ItemType<ProfanedGuardianTrophy>(), 10);
            npcLoot.Add(ModContent.ItemType<ProfanedCore>());

			// Furniture
            npcLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<ProfanedGuardiansRelic>());

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedGuardians, ModContent.ItemType<KnowledgeProfanedGuardians>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark the Profaned Guardians as dead
            DownedBossSystem.downedGuardians = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (damage > 0)
                player.AddBuff(ModContent.BuffType<HolyFlames>(), 300, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossA").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossA2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossA3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossA4").Type, 1f);
                }
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
