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
        private const float TimeForShieldDespawn = 120f;

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
            NPC.LifeMaxNERB(90000, 108000, 200000);
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
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            spearType = reader.ReadInt32();
            healTimer = reader.ReadInt32();
            biomeEnrageTimer = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
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

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1.1f, 0.9f, 0f);

            // Projectile and dust spawn location variables
            Vector2 dustAndProjectileOffset = new Vector2(40f * NPC.direction, 20f);
            Vector2 shootFrom = NPC.Center + dustAndProjectileOffset;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.005f;

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
            {
                NPC.localAI[0] = 1f;
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ProfanedGuardianDefender>());
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ProfanedGuardianHealer>());
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
                        SoundEngine.PlaySound(SoundID.Item8, shootFrom);

                        int maxHealDustIterations = (int)distanceFromHealer;
                        int maxDust = 100;
                        int dustDivisor = maxHealDustIterations / maxDust;
                        if (dustDivisor < 2)
                            dustDivisor = 2;

                        Vector2 healDustOffset = new Vector2(40f * Main.npc[CalamityGlobalNPC.doughnutBossHealer].direction, 20f);
                        Vector2 dustLineStart = Main.npc[CalamityGlobalNPC.doughnutBossHealer].Center + healDustOffset;
                        Vector2 dustLineEnd = shootFrom;
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

                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].ai[0] == 599 && CalamityWorld.getFixedBoi && Main.netMode != NetmodeID.MultiplayerClient)
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

            if ((!Main.dayTime && !CalamityWorld.getFixedBoi) || !player.active || player.dead || Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if ((!Main.dayTime && !CalamityWorld.getFixedBoi) || !player.active || player.dead || Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
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

                    // Tells the other Guardians that it's time to despawn
                    NPC.ai[3] = -1f;

                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Reset the despawn variable to be used for spear attacks
            if (NPC.ai[3] < 0f)
                NPC.ai[3] = 0f;

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

            bool phase1 = healerAlive || defenderAlive;

            NPC.chaseable = !phase1;

            if (!phase1)
            {
                if (NPC.localAI[1] < TimeForShieldDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[1] == 0f)
                        SoundEngine.PlaySound(SoundID.Item105, NPC.Center);

                    NPC.localAI[1] += 1f;
                }
            }

            // Charge variables
            float chargeVelocityMult = 0.25f;
            float maxChargeVelocity = (bossRush || biomeEnraged) ? 32f : death ? 28f : revenge ? 26f : expertMode ? 24f : 20f;
            if (Main.getGoodWorld)
                maxChargeVelocity *= 1.15f;

            float inertia = (bossRush || biomeEnraged) ? 50f : death ? 60f : revenge ? 65f : expertMode ? 70f : 80f;
            if (lifeRatio < 0.5f)
                inertia *= 0.8f;
            if (!phase1)
                inertia *= 0.75f;
            if (Main.getGoodWorld)
                inertia *= 0.8f;

            float num1006 = 0.111111117f * inertia;

            int totalDustPerProjectile = 15;

            if (NPC.ai[0] == 0f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                float velocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;

                float distanceToStayAwayFromTarget = healerAlive ? 800f : defenderAlive ? 720f : 600f;
                Vector2 destination = player.Center + Vector2.UnitX * distanceToStayAwayFromTarget * -NPC.direction;
                Vector2 targetVector = destination - NPC.Center;
                Vector2 desiredVelocity = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;
                float phaseGateValue = (bossRush || biomeEnraged) ? 50f : death ? 66f : revenge ? 75f : expertMode ? 83f : 100f;
                bool continueShootingProjectiles = phase1 || (NPC.ai[2] < (phaseGateValue * 5f));

                if (continueShootingProjectiles)
                {
                    // Move towards destination
                    if (Vector2.Distance(NPC.Center, destination) > 80f)
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + desiredVelocity) / inertia;
                    else
                        NPC.velocity *= 0.98f;

                    // Alternate between firing profaned spears and holy fire
                    // Shoot holy blasts in final phase
                    NPC.ai[2] += 1f;
                    float projectileShootGateValue = (bossRush || biomeEnraged) ? 60f : death ? 80f : revenge ? 90f : expertMode ? 100f : 120f;
                    if (healerAlive || !phase1)
                        projectileShootGateValue = (int)(projectileShootGateValue * 1.25f);

                    if (NPC.ai[2] % projectileShootGateValue == 0f)
                    {
                        if (phase1)
                        {
                            // Fire a few extra spears and holy fires after the healer is dead and the defender is defending the commander
                            bool fireExtraProjectiles = false;
                            if (defenderAlive)
                            {
                                if (Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 1f)
                                    fireExtraProjectiles = true;
                            }

                            bool shootSpear = NPC.ai[2] % (projectileShootGateValue * 2f) == 0f;
                            float projectileVelocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                            Vector2 finalProjectileVelocity = Vector2.Normalize(player.Center - shootFrom) * projectileVelocity;
                            int type = shootSpear ? ModContent.ProjectileType<ProfanedSpear>() : ModContent.ProjectileType<HolyFire2>();
                            int damage = NPC.GetProjectileDamage(type);

                            if (type == ModContent.ProjectileType<HolyFire2>())
                                finalProjectileVelocity *= 0.5f;

                            for (int k = 0; k < totalDustPerProjectile; k++)
                                Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, finalProjectileVelocity.X, finalProjectileVelocity.Y, 0, default, 1f);

                            if (fireExtraProjectiles)
                            {
                                int baseProjectileAmt = (bossRush || biomeEnraged) ? 4 : 2;
                                int spread = (bossRush || biomeEnraged) ? 18 : 10;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < baseProjectileAmt; i++)
                                {
                                    Vector2 perturbedSpeed = finalProjectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(baseProjectileAmt - 1)));

                                    for (int k = 0; k < totalDustPerProjectile; k++)
                                        Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, perturbedSpeed.X, perturbedSpeed.Y, 0, default, 1f);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                }
                            }
                            else if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, finalProjectileVelocity, type, damage, 0f, Main.myPlayer);
                        }
                        else
                        {
                            // Shoot holy blasts
                            float holyBlastVelocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                            Vector2 finalHolyBlastVelocity = Vector2.Normalize(player.Center - shootFrom) * holyBlastVelocity;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<HolyBlast>();
                                int damage = NPC.GetProjectileDamage(type);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, finalHolyBlastVelocity, type, damage, 0f, Main.myPlayer);
                                    Main.projectile[proj].timeLeft = 180;
                                }
                            }

                            // Dust for blasting out the holy blasts
                            for (int i = 0; i < 100; i++)
                            {
                                int dustID;
                                switch (Main.rand.Next(6))
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                        dustID = (int)CalamityDusts.ProfanedFire;
                                        break;
                                    default:
                                        dustID = DustID.OrangeTorch;
                                        break;
                                }

                                // Choose a random speed and angle to blast the fire out
                                float dustSpeed = Main.rand.NextFloat(holyBlastVelocity * 0.5f, holyBlastVelocity);
                                float angleRandom = 0.06f;
                                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(finalHolyBlastVelocity.ToRotation());
                                dustVel = dustVel.RotatedBy(-angleRandom);
                                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                                // Pick a size for the fire particles
                                float scale = Main.rand.NextFloat(1f, 2f);

                                // Actually spawn the fire
                                int idx = Dust.NewDust(shootFrom, 180, 180, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                                Main.dust[idx].noGravity = true;
                            }

                            NPC.velocity = -finalHolyBlastVelocity * 0.5f;
                        }
                    }
                }
                else
                {
                    // Slow down and transition to charge phase
                    NPC.velocity *= 0.98f;
                    if (NPC.ai[1] >= phaseGateValue)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }
                    else
                        NPC.ai[1] += 1f;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                NPC.ai[0] = 2f;
                NPC.netUpdate = true;
                Vector2 targetVector = player.Center - NPC.Center;
                Vector2 velocity = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f));
                velocity *= maxChargeVelocity;

                // Start slow and accelerate
                NPC.velocity = velocity * chargeVelocityMult;

                // Dust ring and sound right as charge begins
                SoundEngine.PlaySound(SoundID.Item74, shootFrom);
                int totalDust = 36;
                for (int k = 0; k < totalDust; k++)
                {
                    Vector2 dustSpawnPos = NPC.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(160f, 160f);
                    dustSpawnPos = dustSpawnPos.RotatedBy((double)((k - (totalDust / 2 - 1)) * MathHelper.TwoPi / totalDust), default) + shootFrom;
                    Vector2 dustVelocity = dustSpawnPos - shootFrom;
                    int dust = Dust.NewDust(dustSpawnPos + dustVelocity, 0, 0, (int)CalamityDusts.ProfanedFire, dustVelocity.X, dustVelocity.Y, 0, default, 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].noLight = true;
                    Main.dust[dust].scale = 3f;
                    Main.dust[dust].velocity = dustVelocity * 0.3f;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                // Face the direction of the charge
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] += 1f;
                float phaseGateValue = (bossRush || biomeEnraged) ? 90f : death ? 100f : revenge ? 110f : expertMode ? 120f : 135f;
                if (NPC.ai[1] >= phaseGateValue)
                {
                    NPC.ai[0] = 3f;

                    // Slown down duration ranges from 20 to 40 frames in rev, otherwise it's always 40 frames
                    float slowDownDurationAfterCharge = revenge ? Main.rand.Next(20, 41) : 40f;
                    NPC.ai[1] = slowDownDurationAfterCharge;
                    NPC.localAI[2] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 targetVector = (player.Center - NPC.Center).SafeNormalize(new Vector2(NPC.direction, 0f));

                    if (NPC.localAI[2] == 0f)
                    {
                        // Accelerate
                        if (NPC.velocity.Length() < maxChargeVelocity)
                        {
                            float velocityMult = (bossRush || biomeEnraged) ? 1.053333f : death ? 1.05037f : revenge ? 1.047407f : expertMode ? 1.044444f : 1.04f;
                            NPC.velocity = targetVector * (NPC.velocity.Length() * velocityMult);
                            if (NPC.velocity.Length() > maxChargeVelocity)
                            {
                                NPC.localAI[2] = 1f;
                                NPC.velocity = NPC.velocity.SafeNormalize(new Vector2(NPC.direction, 0f)) * maxChargeVelocity;
                            }
                        }
                    }
                    else
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector * (NPC.velocity.Length() + num1006)) / inertia;

                    // Throw down holy fire while charging
                    int projectileGateValue = (int)(phaseGateValue * 0.4f);
                    if (NPC.ai[1] % projectileGateValue == 0f)
                    {
                        float projectileVelocityY = NPC.velocity.Y;
                        if (projectileVelocityY < 0f)
                            projectileVelocityY = 0f;

                        projectileVelocityY += expertMode ? 3f : 2f;
                        Vector2 projectileVelocity = new Vector2(NPC.velocity.X * 0.2f, projectileVelocityY);

                        for (int k = 0; k < totalDustPerProjectile; k++)
                            Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, projectileVelocity.X, projectileVelocity.Y, 0, default, 1f);

                        int type = ModContent.ProjectileType<HolyFire>();
                        int damage = NPC.GetProjectileDamage(type);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, projectileVelocity, type, damage, 0f, Main.myPlayer);
                    }
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                // Face the direction of the charge
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] -= 1f;
                if (NPC.ai[1] <= 0f)
                {
                    NPC.ai[2] += 1f;

                    // Charges one to three times in a row in rev, otherwise it will always charge twice in a row
                    float totalCharges = revenge ? (Main.rand.Next(3) + 1f) : 2f;
                    bool dontCharge = NPC.ai[2] >= totalCharges;
                    bool useSpears = NPC.ai[3] % 2f == 0f;
                    NPC.ai[0] = dontCharge ? (useSpears ? 4f : 0f) : 1f;
                    if (dontCharge)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[3] += 1f;
                    }

                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.95f;
            }
            else if (NPC.ai[0] == 4f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                float velocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;

                float distanceToStayAwayFromTarget = 800f;
                Vector2 destination = player.Center + Vector2.UnitX * distanceToStayAwayFromTarget * -NPC.direction;
                Vector2 targetVector = destination - NPC.Center;
                Vector2 desiredVelocity = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;

                NPC.ai[1] += 1f;
                float totalSpears = 10f;
                float shootDuration = (bossRush || biomeEnraged) ? 180f : death ? 200f : revenge ? 210f : expertMode ? 220f : 240f;
                float dontShootTime = shootDuration * 0.3f;
                float phaseGateValue = dontShootTime + shootDuration;

                if (NPC.ai[1] < phaseGateValue)
                {
                    // Move towards destination
                    if (Vector2.Distance(NPC.Center, destination) > 80f)
                    {
                        inertia *= 0.5f;
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + desiredVelocity) / inertia;
                    }
                    else
                        NPC.velocity *= 0.98f;
                }

                int spearShootDivisor = (int)(shootDuration / totalSpears);
                float totalPhaseDuration = phaseGateValue + dontShootTime;
                if (NPC.ai[1] >= dontShootTime)
                {
                    if (NPC.ai[1] >= phaseGateValue)
                    {
                        // Slow down and transition to charge phase
                        NPC.velocity *= 0.98f;

                        if (NPC.ai[1] >= totalPhaseDuration)
                        {
                            NPC.ai[0] = 1f;
                            NPC.ai[1] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] % spearShootDivisor == 0f)
                    {
                        float spearVelocity = velocity;
                        Vector2 velocity2 = Vector2.Normalize(player.Center - shootFrom) * spearVelocity;
                        Vector2 knockbackVelocity = velocity2 * 0.1f;
                        int type = ModContent.ProjectileType<HolySpear>();
                        int damage = NPC.GetProjectileDamage(type);

                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, shootFrom);
                        for (int k = 0; k < totalDustPerProjectile; k++)
                            Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, velocity2.X, velocity2.Y, 0, default, 1f);

                        if (NPC.ai[1] % (spearShootDivisor * 3) == 0f)
                        {
                            knockbackVelocity *= 5f;
                            int baseProjectileAmt = (bossRush || biomeEnraged) ? 8 : expertMode ? 4 : 2;
                            int spread = (bossRush || biomeEnraged) ? 36 : expertMode ? 20 : 12;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < baseProjectileAmt; i++)
                            {
                                Vector2 perturbedSpeed = velocity2.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(baseProjectileAmt - 1))) * 0.25f;

                                for (int k = 0; k < totalDustPerProjectile; k++)
                                    Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, perturbedSpeed.X, perturbedSpeed.Y, 0, default, 1f);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, velocity2, type, damage, 0f, Main.myPlayer, 1f, 0f);

                        NPC.velocity = -knockbackVelocity;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 drawPos = NPC.Center - screenPos;
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

            Vector2 vector43 = drawPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommanderGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
            if (CalamityWorld.getFixedBoi)
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

            // Draw shields while healer or defender are alive
            if (NPC.localAI[1] < TimeForShieldDespawn)
            {
                float maxOscillation = 60f;
                float minScale = 0.8f;
                float maxPulseScale = 1f - minScale;
                float minOpacity = 0.5f;
                float maxOpacityScale = 1f - minOpacity;
                float currentOscillation = MathHelper.Lerp(0f, maxOscillation, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi) + 1f) * 0.5f);
                float shieldOpacity = minOpacity + maxOpacityScale * Utils.Remap(currentOscillation, 0f, maxOscillation, 1f, 0f);
                float oscillationRatio = currentOscillation / maxOscillation;
                float invertedOscillationRatio = 1f - (1f - oscillationRatio) * (1f - oscillationRatio);
                float oscillationScale = 1f - (1f - invertedOscillationRatio) * (1f - invertedOscillationRatio);
                float remappedOscillation = Utils.Remap(currentOscillation, maxOscillation - 15f, maxOscillation, 0f, 1f);
                float twoOscillationsMultipliedTogetherForScaleCalculation = remappedOscillation * remappedOscillation;
                float invertedOscillationUsedForScale = MathHelper.Lerp(minScale, 1f, 1f - twoOscillationsMultipliedTogetherForScaleCalculation);
                float shieldScale = (minScale + maxPulseScale * oscillationScale) * invertedOscillationUsedForScale;
                float smallerRemappedOscillation = Utils.Remap(currentOscillation, 20f, maxOscillation, 0f, 1f);
                float invertedSmallerOscillationRatio = 1f - (1f - smallerRemappedOscillation) * (1f - smallerRemappedOscillation);
                float smallerOscillationScale = 1f - (1f - invertedSmallerOscillationRatio) * (1f - invertedSmallerOscillationRatio);
                float shieldScale2 = (minScale + maxPulseScale * smallerOscillationScale) * invertedOscillationUsedForScale;
                Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleOpenCircle").Value;
                Rectangle shieldFrame = shieldTexture.Frame();
                Vector2 origin = shieldFrame.Size() * 0.5f;
                Vector2 shieldDrawPos = NPC.Center - screenPos;
                shieldDrawPos -= new Vector2(shieldTexture.Width, shieldTexture.Height) * NPC.scale / 2f;
                shieldDrawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                float minHue = (defenderAlive && healerAlive) ? 0.18f : 0.06f;
                float maxHue = minHue + 0.12f;
                float opacityScaleDuringShieldDespawn = (TimeForShieldDespawn - NPC.localAI[1]) / TimeForShieldDespawn;
                float scaleDuringShieldDespawnScale = 1.8f;
                float scaleDuringShieldDespawn = (1f - opacityScaleDuringShieldDespawn) * scaleDuringShieldDespawnScale;
                float colorScale = MathHelper.Lerp(0f, shieldOpacity, opacityScaleDuringShieldDespawn);
                Color color = Main.hslToRgb(MathHelper.Lerp(maxHue - minHue, maxHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                Color color2 = Main.hslToRgb(MathHelper.Lerp(minHue, maxHue - minHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi * 3f) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                color2.A = 0;
                color *= 0.6f;
                color2 *= 0.6f;
                float scaleMult = 1.2f + scaleDuringShieldDespawn;
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color, NPC.rotation, origin, shieldScale2 * scaleMult, SpriteEffects.None, 0f);
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale2 * scaleMult * 0.95f, SpriteEffects.None, 0f);
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color, NPC.rotation, origin, shieldScale * scaleMult, SpriteEffects.None, 0f);
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale * scaleMult * 0.95f, SpriteEffects.None, 0f);
            }

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
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedGuardians, ModContent.ItemType<LoreProfanedGuardians>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark the Profaned Guardians as dead
            DownedBossSystem.downedGuardians = true;
            CalamityNetcode.SyncWorld();
        }

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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);

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
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
