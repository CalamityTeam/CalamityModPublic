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
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Events;
using Terraria.Audio;

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
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 20f;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 100;
            NPC.height = 80;
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
            Music = CalamityMod.Instance.GetMusicFromMusicMod("Guardians") ?? MusicID.Boss1;
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
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
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC.doughnutBoss = NPC.whoAmI;

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            Vector2 vectorCenter = NPC.Center;
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[1] == 0f)
            {
                NPC.localAI[1] = 1f;
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
                float healGateValue = 60f;
                healTimer++;
                if (healTimer >= healGateValue)
                {
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

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!Main.dayTime || !player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!Main.dayTime || !player.active || player.dead)
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
            if (!isHoly && !isHell && !BossRushEvent.BossRushActive)
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

            if (NPC.ai[0] == 0f)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = vectorCenter.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                float velocity = (malice || biomeEnraged) ? 20f : death ? 18f : revenge ? 17f : expertMode ? 16f : 14f;
                Vector2 targetVector = player.Center - vectorCenter;
                targetVector = Vector2.Normalize(targetVector) * velocity;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                if (defenderAlive)
                    phaseGateValue *= 1.25f;

                if (NPC.ai[3] < phaseGateValue || healerAlive)
                {
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector) / inertia;
                    NPC.ai[3] += 1f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float divisor = (malice || biomeEnraged) ? 15f : death ? 20f : revenge ? 22f : expertMode ? 25f : 30f;
                        if (phase1)
                            divisor *= 2f;

                        if (NPC.ai[3] % divisor == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.position);
                            int type = ModContent.ProjectileType<FlareDust>();
                            int damage = NPC.GetProjectileDamage(type);
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
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), vectorCenter, normalizedPerturbedSpeed * NPC.velocity.Length() * 1.25f, type, damage, 0f, Main.myPlayer, 2f, 0f);
                            }
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
                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 12f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    Vector2 velocity = new Vector2(NPC.ai[2], NPC.ai[3]);
                    velocity.Normalize();
                    velocity *= (malice || biomeEnraged) ? 39f : death ? 33f : revenge ? 30f : expertMode ? 27f : 21f;
                    if (defenderAlive)
                        velocity *= 0.8f;

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
                    float projectileGateValue = (malice || biomeEnraged) ? 30f : death ? 35f : revenge ? 37f : expertMode ? 40f : 45f;
                    if (NPC.localAI[0] >= projectileGateValue && Vector2.Distance(vectorCenter, player.Center) > 160f)
                    {
                        NPC.localAI[0] = 0f;

                        SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                        int totalProjectiles = phase1 ? 6 : 10;
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

                        if (malice || biomeEnraged)
                            totalProjectiles *= 2;

                        float radians = MathHelper.TwoPi / totalProjectiles;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            Vector2 vector255 = new Vector2(0f, -velocity).RotatedBy(radians * i);
                            Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, vector255, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }

                        spearType++;
                        if (spearType > 2)
                            spearType = 0;
                    }
                }

                NPC.ai[1] += 1f;
                float phaseGateValue = (malice || biomeEnraged) ? 60f : death ? 70f : revenge ? 75f : expertMode ? 80f : 90f;
                if (NPC.ai[1] >= phaseGateValue)
                {
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 24f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 targetVector = player.Center - vectorCenter;
                    targetVector.Normalize();
                    if (targetVector.HasNaNs())
                        targetVector = new Vector2(NPC.direction, 0f);

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

                NPC.velocity *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
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
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianBossGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 = NPC.GetAlpha(color41);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

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
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Profaned Guardians have no actual drops and no treasure bag
            DropHelper.DropItemChance(NPC, ModContent.ItemType<ProfanedGuardianMask>(), 7);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<ProfanedGuardianTrophy>(), 10);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<RelicOfDeliverance>(), 4);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<SamuraiBadge>(), 10);
            DropHelper.DropItem(NPC, ModContent.ItemType<ProfanedCoreUnlimited>());
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<KnowledgeProfanedGuardians>(), true, !DownedBossSystem.downedGuardians);

            // Mark the Profaned Guardians as dead
            DownedBossSystem.downedGuardians = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
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
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/ProfanedGuardianBossGores/ProfanedGuardianBossA4").Type, 1f);
                }
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
