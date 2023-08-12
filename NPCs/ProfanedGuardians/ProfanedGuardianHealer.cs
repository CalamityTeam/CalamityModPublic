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
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianHealer : ModNPC
    {
        private enum Phase
        {
            CrystalShards = 0,
            Stars = 1
        }

        private float AIState
        {
            get => NPC.localAI[0];
            set => NPC.localAI[0] = value;
        }

        private float AITimer
        {
            get => NPC.ai[3];
            set => NPC.ai[3] = value;
        }

        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public override void SetStaticDefaults()
        {
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
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 3f;
            NPC.aiStyle = -1;
            NPC.GetNPCDamage();
            NPC.width = 228;
            NPC.height = 164;
            NPC.defense = 30;
            NPC.DR_NERD(0.2f);
            NPC.LifeMaxNERB(60000, 72000, 50000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            AIType = -1;
            NPC.HitSound = SoundID.NPCHit52;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<ProfanedGuardianCommander>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ProfanedGuardianHealer")
            });
        }

        public float GetStarShootSlowDownGateValue() => BossRushEvent.BossRushActive ? 180f : CalamityWorld.death ? 210f : CalamityWorld.revenge ? 225f : Main.expertMode ? 240f : 270f;

        public float GetStarShootGateValue() => GetStarShootSlowDownGateValue() + 60f;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            NPC.localAI[0] = reader.ReadSingle();
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
            CalamityGlobalNPC.doughnutBossHealer = NPC.whoAmI;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1.1f, 0.9f, 0f);

            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            Vector2 dustAndProjectileOffset = new Vector2(40f * NPC.direction, 20f);
            Vector2 shootFrom = NPC.Center + dustAndProjectileOffset;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.005f;

            // Despawn
            if (Main.npc[CalamityGlobalNPC.doughnutBoss].ai[3] == -1f)
            {
                NPC.velocity = Main.npc[CalamityGlobalNPC.doughnutBoss].velocity;
                return;
            }

            // Get the Guardian Commander's target
            Player player = Main.player[Main.npc[CalamityGlobalNPC.doughnutBoss].target];

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;
            bool isHoly = player.ZoneHallow;
            bool isHell = player.ZoneUnderworldHeight;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (Main.zenithWorld)
                NPC.ai[0]++;
            if (NPC.ai[0] >= 300f)
                NPC.ai[1] = 1f;

            // Become immune over time if target isn't in hell or hallow
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

            // Direction
            if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
            {
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            if (NPC.ai[1] == 1f && Main.zenithWorld)
            {
                NPC.ai[2]++;
                NPC.velocity = Vector2.Zero;
                // Spray out stars of healing stars in gfb
                if (NPC.ai[2] >= 45f)
                {
                    int type = ModContent.ProjectileType<HolyBurnOrb>();
                    int damage = NPC.GetProjectileDamage(type);
                    int totalProjectiles = 10;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    float projectileVelocity = 8f;
                    Vector2 spinningPoint = new Vector2(0f, -projectileVelocity);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, velocity2, type, 0, 0f, Main.myPlayer, 0f, damage);
                    }
                    NPC.ai[2] = 0f;
                }
                if (NPC.ai[0] >= 600f)
                {
                    SoundEngine.PlaySound(SoundID.Item10, player.Center);
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[0] = 0f;
                }
                return;
            }

            bool useCrystalShards = AIState == (float)Phase.CrystalShards;
            float velocity = useCrystalShards ? ((bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f) : (Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 5f);
            if (Main.getGoodWorld)
                velocity *= 1.25f;

            float idealDistanceFromDestination = useCrystalShards ? 80f : 160f;
            Vector2 destination = player.Center + (useCrystalShards ? Vector2.Zero : Vector2.UnitX * player.velocity.SafeNormalize(new Vector2(NPC.direction, 0f)).X * 400f) + Vector2.UnitY * -400f;
            Vector2 distanceFromDestination = destination - NPC.Center;
            Vector2 desiredVelocity = distanceFromDestination.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;

            // Whether the commander is calling all guardians together for the laser attack
            bool commanderUsingLaser = Main.npc[CalamityGlobalNPC.doughnutBoss].ai[0] == 5f;
            if (commanderUsingLaser)
            {
                NPC.Calamity().DR = 0.9f;
                NPC.Calamity().unbreakableDR = true;
                NPC.Calamity().CurrentlyIncreasingDefenseOrDR = true;
            }
            else
            {
                NPC.Calamity().DR = 0.2f;
                NPC.Calamity().unbreakableDR = false;
                NPC.Calamity().CurrentlyIncreasingDefenseOrDR = false;
            }

            // Laser attack
            if (commanderUsingLaser)
            {
                AITimer = 0f;

                // Move towards the commander
                distanceFromDestination = Main.npc[CalamityGlobalNPC.doughnutBoss].Center - NPC.Center;
                desiredVelocity = distanceFromDestination.SafeNormalize(new Vector2(NPC.direction, 0f)) * (Main.npc[CalamityGlobalNPC.doughnutBoss].velocity.Length() + 5f);
                if (distanceFromDestination.Length() > 40f)
                {
                    float inertia = 10f;
                    if (Main.getGoodWorld)
                        inertia *= 0.8f;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + desiredVelocity) / inertia;
                }
                else
                    NPC.velocity *= 0.9f;

                return;
            }

            // Fire crystal rain similar to Providence's Crystal attack
            if (useCrystalShards)
            {
                // Increment timer
                AITimer += 1f;
                float crystalShootGateValue = bossRush ? 140f : death ? 180f : revenge ? 200f : expertMode ? 220f : 260f;
                float crystalShootPhaseDuration = crystalShootGateValue + crystalShootGateValue * 0.25f;

                // Generate dust that scales with how close the crystals are to firing
                float dustGateValue = crystalShootGateValue * 0.5f;
                if (AITimer >= dustGateValue && AITimer < crystalShootGateValue)
                {
                    int dustChance = (int)((crystalShootGateValue - AITimer) * 0.25f);
                    if (dustChance < 2)
                        dustChance = 2;

                    int maxDust = 10;
                    for (int i = 0; i < maxDust; i++)
                    {
                        if (Main.rand.NextBool(dustChance))
                        {
                            Color baseColor = new Color(250, 150, 0);
                            float brightness = 0.8f;
                            Color dustColor = Color.Lerp(baseColor, Color.White, brightness);
                            Dust dust = Main.dust[Dust.NewDust(NPC.Top, 0, 0, 267, 0f, 0f, 100, dustColor, 1f)];
                            dust.velocity.X = 0f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.position = shootFrom + Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * (4f * Main.rand.NextFloat() + 26f);
                            dust.scale = 0.8f;
                        }
                    }
                }

                // Fire crystals
                if (AITimer == crystalShootGateValue)
                {
                    SoundEngine.PlaySound(SoundID.Item109, shootFrom);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<ProvidenceCrystalShard>();
                        int damage = NPC.GetProjectileDamage(type);
                        int totalProjectiles = biomeEnraged ? 18 : death ? 16 : revenge ? 14 : expertMode ? 12 : 10;
                        float speedX = -12f;
                        float speedAdjustment = Math.Abs(speedX * 2f / (totalProjectiles - 1));
                        float speedY = -4f;
                        float randomVelocityMult = death ? 2f : 1f;
                        for (int i = 0; i < totalProjectiles; i++)
                        {
                            float x4 = Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X;
                            Vector2 randomizedVelocity = Vector2.Zero;
                            if (revenge)
                            {
                                float randomFloatX = Main.rand.NextFloat() - 0.5f;
                                float randomFloatY = Main.rand.NextFloat() - 0.5f;
                                randomizedVelocity = revenge ? (new Vector2(randomFloatX, randomFloatY) * randomVelocityMult) : Vector2.Zero;
                            }
                            Vector2 projectileVelocity = new Vector2(speedX + speedAdjustment * i + distanceFromDestination.SafeNormalize(Vector2.Zero).X * Math.Abs(player.velocity.X), speedY) + randomizedVelocity;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, projectileVelocity, type, damage, 0f, Main.myPlayer, x4);
                        }
                    }
                }

                // Reset timer and use stars next
                if (AITimer >= crystalShootPhaseDuration)
                {
                    AITimer = 0f;
                    AIState = (float)Phase.Stars;
                }
            }

            // Fire spread of damage and healing stars
            else
            {
                // Increment timer
                AITimer += 1f;
                float starShootPhaseDuration = GetStarShootGateValue() + 60f;
                if (AITimer == GetStarShootGateValue())
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, shootFrom);

                    int totalFlameProjectiles = biomeEnraged ? 20 : 16;
                    int totalRings = revenge ? 3 : 2;
                    int healingStarChance = revenge ? 8 : expertMode ? 6 : 4;
                    double radians = MathHelper.TwoPi / totalFlameProjectiles;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    for (int i = 0; i < totalRings; i++)
                    {
                        bool firstRing = i % 2 == 0;
                        float starVelocity = i + 2f;
                        float velocityX = (float)(starVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = firstRing ? new Vector2(-velocityX, -starVelocity) : new Vector2(0f, -starVelocity);
                        for (int j = 0; j < totalFlameProjectiles; j++)
                        {
                            Vector2 vector2 = spinningPoint.RotatedBy(radians * j);

                            int type = ModContent.ProjectileType<HolyBurnOrb>();
                            int dmgAmt = NPC.GetProjectileDamage(type);
                            if (Main.rand.NextBool(healingStarChance) && !death)
                            {
                                type = ModContent.ProjectileType<HolyLight>();
                                dmgAmt = NPC.GetProjectileDamageNoScaling(type);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, vector2, type, 0, 0f, Main.myPlayer, 0f, dmgAmt);
                            }
                            else if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, vector2, type, dmgAmt, 0f, Main.myPlayer);

                            Color dustColor = Main.hslToRgb(Main.rgbToHsl(type == ModContent.ProjectileType<HolyBurnOrb>() ? Color.Orange : Color.Green).X, 1f, 0.5f);
                            dustColor.A = 255;
                            int maxDust = 3;
                            for (int k = 0; k < maxDust; k++)
                            {
                                int dust = Dust.NewDust(shootFrom, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = shootFrom;
                                Main.dust[dust].velocity = vector2 * starVelocity * (k * 0.5f + 1f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1f + k;
                                Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                Dust dust2 = Dust.CloneDust(dust);
                                Dust dust3 = dust2;
                                dust3.scale /= 2f;
                                dust3 = dust2;
                                dust3.fadeIn /= 2f;
                                dust2.color = new Color(255, 255, 255, 255);
                            }
                        }
                    }
                }

                // Reset timer and use stars next
                if (AITimer >= starShootPhaseDuration)
                {
                    AITimer = 0f;
                    AIState = (float)Phase.CrystalShards;
                }
            }

            // Move towards a location above the player
            if (distanceFromDestination.Length() > idealDistanceFromDestination)
            {
                float inertia = (bossRush || biomeEnraged) ? 28f : death ? 32f : revenge ? 34f : expertMode ? 36f : 40f;
                if (lifeRatio < 0.5f)
                    inertia *= 0.8f;
                if (Main.getGoodWorld)
                    inertia *= 0.8f;

                NPC.velocity = (NPC.velocity * (inertia - 1) + desiredVelocity) / inertia;

                return;
            }

            // Slow down when close enough to destination or when firing stars
            NPC.velocity *= 0.98f;
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            void drawGuardianInstance(Vector2 drawOffset, Color? colorOverride)
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
                Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianHealerGlow2").Value;
                Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
                Color color36 = Color.White;
                float amount9 = 0.5f;
                int num153 = 5;

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num155 = 1; num155 < num153; num155 += 2)
                    {
                        Color color38 = drawColor;
                        color38 = Color.Lerp(color38, color36, amount9);
                        color38 = NPC.GetAlpha(color38);
                        color38 *= (num153 - num155) / 15f;
                        if (colorOverride != null)
                            color38 = colorOverride.Value;

                        Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                        spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }
                }

                Vector2 vector43 = NPC.Center - screenPos;
                vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                spriteBatch.Draw(texture2D15, vector43, NPC.frame, colorOverride ?? NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianHealerGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                if (Main.remixWorld)
                {
                    texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianHealerGlowNight").Value;
                    color37 = Color.Cyan;
                }
                Color color42 = Color.Lerp(Color.White, Color.Violet, 0.5f);
                if (colorOverride != null)
                {
                    color37 = colorOverride.Value;
                    color42 = colorOverride.Value;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163++)
                    {
                        Color color41 = color37;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 = NPC.GetAlpha(color41);
                        color41 *= (num153 - num163) / 15f;
                        if (colorOverride != null)
                            color41 = colorOverride.Value;

                        Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                        spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                        Color color43 = color42;
                        color43 = Color.Lerp(color43, color36, amount9);
                        color43 = NPC.GetAlpha(color43);
                        color43 *= (num153 - num163) / 15f;
                        if (colorOverride != null)
                            color43 = colorOverride.Value;

                        spriteBatch.Draw(texture2D16, vector44, NPC.frame, color43, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                spriteBatch.Draw(texture2D16, vector43, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            // Draw laser effects
            float useLaserGateValue = 120f;
            float stopLaserGateValue = (CalamityWorld.revenge || BossRushEvent.BossRushActive) ? 235f : 315f;
            float maxIntensity = 45f;
            float increaseIntensityGateValue = useLaserGateValue - maxIntensity;
            float decreaseIntensityGateValue = stopLaserGateValue - maxIntensity;
            if (!NPC.IsABestiaryIconDummy)
            {
                bool usingLaser = Main.npc[CalamityGlobalNPC.doughnutBoss].ai[0] == 5f;
                if (usingLaser)
                {
                    bool increaseIntensity = Main.npc[CalamityGlobalNPC.doughnutBoss].ai[1] > increaseIntensityGateValue;
                    bool decreaseIntensity = Main.npc[CalamityGlobalNPC.doughnutBoss].ai[1] > decreaseIntensityGateValue;
                    float burnIntensity = decreaseIntensity ? Utils.GetLerpValue(0f, maxIntensity, maxIntensity - (Main.npc[CalamityGlobalNPC.doughnutBoss].ai[1] - decreaseIntensityGateValue), true) : Utils.GetLerpValue(0f, maxIntensity, Main.npc[CalamityGlobalNPC.doughnutBoss].ai[1], true);
                    int totalGuardiansToDraw = (int)MathHelper.Lerp(1f, 30f, burnIntensity);
                    for (int i = 0; i < totalGuardiansToDraw; i++)
                    {
                        float offsetAngle = MathHelper.TwoPi * i * 2f / totalGuardiansToDraw;
                        float drawOffsetFactor = (float)Math.Sin(offsetAngle * 6f + Main.GlobalTimeWrappedHourly * MathHelper.Pi);
                        drawOffsetFactor *= (float)Math.Pow(burnIntensity, 3f) * 50f;

                        Vector2 drawOffset = offsetAngle.ToRotationVector2() * drawOffsetFactor;
                        Color baseColor = Color.White * (MathHelper.Lerp(0.4f, 0.8f, burnIntensity) / totalGuardiansToDraw * 1.5f);
                        baseColor.A = 0;

                        baseColor = Color.Lerp(Color.White, baseColor, burnIntensity);
                        drawGuardianInstance(drawOffset, totalGuardiansToDraw == 1 ? null : baseColor);
                    }
                }
                else
                    drawGuardianInstance(Vector2.Zero, null);
            }
            else
                drawGuardianInstance(Vector2.Zero, null);

            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<RelicOfConvergence>(), 4);

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 80f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 180, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossH").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossH2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossH3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossH4").Type, 1f);
                }

                for (int k = 0; k < 50; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
