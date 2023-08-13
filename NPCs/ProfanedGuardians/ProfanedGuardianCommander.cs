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
using CalamityMod.Items.Materials;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianCommander : ModNPC
    {
        private int spearType = 0;
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private const float TimeForShieldDespawn = 120f;
        public static readonly SoundStyle HolyRaySound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRay") { Volume = 1.25f };
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash");
        public static readonly SoundStyle ShieldDeathSound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianShieldDeactivate");

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
            NPC.LifeMaxNERB(100000, 120000, 200000);
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
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ProfanedGuardianCommander")
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
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
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
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
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

            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

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

            // Healing and laser
            bool hasDoneLaser = calamityGlobalNPC.newAI[2] == 1f;

            if (healerAlive)
            {
                // Laser attack when the healer is at or below 50% HP
                bool shouldDoLaser = (Main.npc[CalamityGlobalNPC.doughnutBossHealer].life / (float)Main.npc[CalamityGlobalNPC.doughnutBossHealer].lifeMax) <= 0.5f;
                if (!hasDoneLaser && shouldDoLaser && NPC.ai[0] != 5f)
                {
                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.localAI[3] = 0f;
                    NPC.netUpdate = true;
                }

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
                            int healAmt = NPC.lifeMax / 10;
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

                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].ai[0] == 599 && Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Gain more health once the healer's channel heal is done
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

            if ((!Main.dayTime && !Main.remixWorld) || !player.active || player.dead || Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if ((!Main.dayTime && !Main.remixWorld) || !player.active || player.dead || Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
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
                        SoundEngine.PlaySound(ShieldDeathSound, NPC.Center);

                    NPC.localAI[1] += 1f;
                }
            }

            // Side swap variables for Phase 1
            float moveToOtherSideInPhase1GateValue = 900f;
            float timeBeforeMoveToOtherSideInPhase1Reset = moveToOtherSideInPhase1GateValue * 2f;
            float totalGoLowDuration = 240f;
            float goLowDuration = totalGoLowDuration * 0.5f;
            float goLowOrHighDistance = 540f;

            // Side swap variables for Phase 2
            float defenderCommanderGuardPhase2Duration = (bossRush || biomeEnraged) ? 420f : death ? 480f : revenge ? 510f : expertMode ? 540f : 600f;
            float moveToOtherSideInPhase2GateValue = defenderCommanderGuardPhase2Duration - 120f;
            float timeBeforeMoveToOtherSideInPhase2Reset = moveToOtherSideInPhase2GateValue * 2f;
            float totalGoLowDurationPhase2 = 210f;
            float goLowDurationPhase2 = totalGoLowDurationPhase2 * 0.5f;

            // Charge variables
            float chargeVelocityMult = 0.25f;
            float maxChargeVelocity = (bossRush || biomeEnraged) ? 32f : death ? 28f : revenge ? 26f : expertMode ? 24f : 20f;
            if (Main.getGoodWorld)
                maxChargeVelocity *= 1.15f;
            if (CalamityWorld.LegendaryMode && revenge)
                maxChargeVelocity *= 2f;

            float inertia = (bossRush || biomeEnraged) ? 40f : death ? 45f : revenge ? 47f : expertMode ? 50f : 55f;
            if (lifeRatio < 0.5f)
                inertia *= 0.8f;
            if (!phase1)
                inertia *= 0.75f;
            if (Main.getGoodWorld)
                inertia *= 0.8f;

            bool speedUp = Vector2.Distance(NPC.Center, player.Center) > 960f;

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

                // Dictates when the commander and defender will swap to the other side in phase 1
                if (healerAlive)
                    NPC.localAI[3] += 1f;
                else
                    NPC.localAI[3] = 0f;

                // Go low just before moving to the other side to avoid bullshit hits
                float roundedGoLowCheck = (float)Math.Round(goLowDuration * 0.5);
                bool goLow = (NPC.localAI[3] > (moveToOtherSideInPhase1GateValue - goLowDuration) && NPC.localAI[3] <= (moveToOtherSideInPhase1GateValue + roundedGoLowCheck)) ||
                    NPC.localAI[3] > (timeBeforeMoveToOtherSideInPhase1Reset - goLowDuration) || NPC.localAI[3] <= (-roundedGoLowCheck);

                // Swap sides while going low
                if (NPC.localAI[3] == (moveToOtherSideInPhase1GateValue - roundedGoLowCheck) || NPC.localAI[3] == (timeBeforeMoveToOtherSideInPhase1Reset - roundedGoLowCheck))
                    calamityGlobalNPC.newAI[0] *= -1f;

                // Reset the timer to a negative value
                if (NPC.localAI[3] > timeBeforeMoveToOtherSideInPhase1Reset)
                    NPC.localAI[3] = -goLowDuration;

                bool canSwapPlacesWithDefender = false;
                bool defenderCharging = false;
                if (defenderAlive)
                {
                    if (Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 1f && Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[1] < -120f)
                        canSwapPlacesWithDefender = true;

                    if ((Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 0f && Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[3] > 0f) ||
                        (Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 1f && Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[1] >= -60f) ||
                        Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 2f || Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 3f)
                        defenderCharging = true;
                }

                // Dictates when the commander and defender will swap sides in phase 2
                if (!healerAlive && defenderAlive)
                {
                    if (canSwapPlacesWithDefender)
                        calamityGlobalNPC.newAI[1] += 1f;
                }
                else
                    calamityGlobalNPC.newAI[1] = 0f;

                // Go low or high just before moving to the other side in phase 2 to avoid bullshit hits
                float roundedGoLowPhase2Check = (float)Math.Round(goLowDurationPhase2 * 0.5);
                bool goLowPhase2 = calamityGlobalNPC.newAI[1] > (moveToOtherSideInPhase2GateValue - goLowDurationPhase2) && calamityGlobalNPC.newAI[1] <= (moveToOtherSideInPhase2GateValue + roundedGoLowPhase2Check);
                bool goHigh = calamityGlobalNPC.newAI[1] > (timeBeforeMoveToOtherSideInPhase2Reset - goLowDurationPhase2) || calamityGlobalNPC.newAI[1] <= (-roundedGoLowPhase2Check);

                // Swap sides while going low or high
                if (calamityGlobalNPC.newAI[1] == (moveToOtherSideInPhase2GateValue - roundedGoLowPhase2Check) || calamityGlobalNPC.newAI[1] == (timeBeforeMoveToOtherSideInPhase2Reset - roundedGoLowPhase2Check))
                    calamityGlobalNPC.newAI[0] *= -1f;

                // Reset the timer to a negative value
                if (calamityGlobalNPC.newAI[1] > timeBeforeMoveToOtherSideInPhase2Reset)
                    calamityGlobalNPC.newAI[1] = -goLowDurationPhase2;

                // Set side to stay on while not going low or high
                if (!goLow && !goLowPhase2 && !goHigh)
                    calamityGlobalNPC.newAI[0] = -NPC.direction;

                float velocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;
                if (healerAlive)
                    velocity *= 0.8f;

                // Reduce inertia and boost velocity while far away from target or swapping sides
                if (speedUp)
                {
                    inertia *= 0.5f;
                    velocity *= 2f;
                }
                if (goLowPhase2 || goHigh)
                {
                    inertia *= 0.5f;
                    velocity *= 2f;
                }
                else if (goLow)
                {
                    inertia *= 0.66f;
                    velocity *= 1.66f;
                }

                // Slow down while close enough to the player and the defender is charging
                if (defenderCharging && !speedUp)
                {
                    inertia *= 1.5f;
                    velocity *= 0.75f;
                }

                float distanceToStayAwayFromTarget = defenderAlive ? 800f : 600f;
                Vector2 destination = player.Center + Vector2.UnitX * distanceToStayAwayFromTarget * calamityGlobalNPC.newAI[0];
                if (goLow || goLowPhase2 || goHigh)
                    destination.Y += goHigh ? -goLowOrHighDistance : goLowOrHighDistance;

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
                    float projectileShootGateValue = (bossRush || biomeEnraged) ? 40f : death ? 60f : revenge ? 90f : expertMode ? 100f : 150f;
                    if (NPC.ai[2] % projectileShootGateValue == 0f)
                    {
                        if (phase1)
                        {
                            // Fire a few extra spears and holy fires after the healer is dead and the defender is defending the commander
                            bool fireExtraProjectiles = false;
                            if (defenderAlive)
                            {
                                if (Main.npc[CalamityGlobalNPC.doughnutBossDefender].ai[0] == 2f)
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

                            if (fireExtraProjectiles && shootSpear)
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
                            float holyBlastVelocity = (bossRush || biomeEnraged) ? 20f : death ? 18f : revenge ? 17f : expertMode ? 16f : 14f;
                            int projTimeLeft = (int)(2000f / holyBlastVelocity);
                            Vector2 finalHolyBlastVelocity = Vector2.Normalize(player.Center - shootFrom) * holyBlastVelocity;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<HolyBlast>();
                                int damage = NPC.GetProjectileDamage(type);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, finalHolyBlastVelocity, type, damage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                                    Main.projectile[proj].timeLeft = projTimeLeft;
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
                SoundEngine.PlaySound(DashSound, NPC.Center);
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
                    {
                        inertia *= 1.5f;
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector * (NPC.velocity.Length() + num1006)) / inertia;
                    }

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

                // Catch up to the target
                // Enrage after this occurs and fire spears twice as fast
                if (Vector2.Distance(NPC.Center, player.Center) > 1600f)
                    NPC.ai[2] = 1f;

                bool targetRanAwayAndWillNowBeFucked = NPC.ai[2] == 1f;
                bool boostVelocityToCatchUp = NPC.ai[1] == 0f || targetRanAwayAndWillNowBeFucked;
                float velocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;
                if (boostVelocityToCatchUp)
                    velocity *= 2f;

                float distanceToStayAwayFromTargetForSpears = 640f;
                Vector2 destination = player.Center + Vector2.UnitX * distanceToStayAwayFromTargetForSpears * -NPC.direction;
                Vector2 targetVector = destination - NPC.Center;
                Vector2 desiredVelocity = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;

                float totalSpears = 12f;
                float shootDuration = (bossRush || biomeEnraged) ? 240f : death ? 280f : revenge ? 300f : expertMode ? 320f : 360f;
                float dontShootTime = shootDuration * 0.3f;
                float phaseGateValue = dontShootTime + shootDuration;

                // Don't attack until close enough to the target
                if (NPC.ai[1] > 0f)
                    NPC.ai[1] += 1f;

                if (NPC.ai[1] < phaseGateValue)
                {
                    // Move towards destination
                    if (Vector2.Distance(NPC.Center, destination) > 80f)
                    {
                        inertia *= targetRanAwayAndWillNowBeFucked ? 0.25f : 0.5f;
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + desiredVelocity) / inertia;
                    }
                    else
                    {
                        // Set the Commander to be able to attack
                        if (NPC.ai[1] == 0f)
                            NPC.ai[1] = 1f;

                        NPC.velocity *= 0.96f;
                    }
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
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] % spearShootDivisor == 0f)
                    {
                        float spearVelocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                        if (Main.getGoodWorld)
                            spearVelocity *= 1.25f;
                        if (boostVelocityToCatchUp)
                            spearVelocity *= 1.5f;

                        Vector2 velocity2 = Vector2.Normalize(player.Center - shootFrom) * spearVelocity;
                        Vector2 knockbackVelocity = velocity2 * 0.1f;
                        int type = ModContent.ProjectileType<HolySpear>();
                        int damage = NPC.GetProjectileDamage(type);

                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, shootFrom);
                        for (int k = 0; k < totalDustPerProjectile; k++)
                            Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, velocity2.X, velocity2.Y, 0, default, 1f);

                        if (NPC.ai[1] % (spearShootDivisor * 3) == 0f || targetRanAwayAndWillNowBeFucked)
                        {
                            knockbackVelocity *= 2f;
                            int baseProjectileAmt = (bossRush || biomeEnraged) ? 8 : expertMode ? 6 : 4;
                            int spread = (bossRush || biomeEnraged) ? 60 : expertMode ? 50 : 40;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < baseProjectileAmt; i++)
                            {
                                Vector2 perturbedSpeed = velocity2.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(baseProjectileAmt - 1))) * 0.3f;

                                for (int k = 0; k < totalDustPerProjectile; k++)
                                    Dust.NewDust(shootFrom, 30, 30, (int)CalamityDusts.ProfanedFire, perturbedSpeed.X, perturbedSpeed.Y, 0, default, 1f);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, perturbedSpeed, type, damage, 0f, Main.myPlayer, targetRanAwayAndWillNowBeFucked ? -2f : -1f, -30f);
                            }
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, velocity2 * 0.85f, type, damage, 0f, Main.myPlayer, 1f, 0f);

                        if (!targetRanAwayAndWillNowBeFucked)
                            NPC.velocity = -knockbackVelocity;
                    }
                }
            }
            else if (NPC.ai[0] == 5f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                float laserGateValue = 120f;
                float velocity = (bossRush || biomeEnraged) ? 4.5f : death ? 4f : revenge ? 3.75f : expertMode ? 3.5f : 3f;
                if (NPC.ai[1] < laserGateValue)
                    velocity *= 6f;
                if (Main.getGoodWorld)
                    velocity *= 1.25f;

                calamityGlobalNPC.newAI[0] = -NPC.direction;
                float distanceToStayAwayFromTargetForLaser = 720f;
                Vector2 destination = player.Center + Vector2.UnitX * distanceToStayAwayFromTargetForLaser * calamityGlobalNPC.newAI[0];
                Vector2 targetVector = destination - NPC.Center;
                Vector2 desiredVelocity = targetVector.SafeNormalize(new Vector2(NPC.direction, 0f)) * velocity;

                // Move towards destination
                if (Vector2.Distance(NPC.Center, destination) > 80f)
                    NPC.velocity = (NPC.velocity * (inertia - 1f) + desiredVelocity) / inertia;
                else
                    NPC.velocity *= 0.98f;

                // Limit Y velocity while firing laser
                if (NPC.ai[1] >= laserGateValue)
                {
                    float speedCap = Main.getGoodWorld ? 4f : 2f;
                    if (NPC.velocity.Y > speedCap)
                        NPC.velocity.Y = speedCap;
                    if (NPC.velocity.Y < -speedCap)
                        NPC.velocity.Y = -speedCap;
                }

                NPC.ai[2] += 1f;
                if (NPC.ai[2] < laserGateValue)
                {
                    Vector2 dustPosOffset = new Vector2(27f, 59f);
                    if (NPC.ai[2] >= 40f)
                    {
                        int extraDustAmt = 0;
                        if (NPC.ai[2] >= 80f)
                            extraDustAmt = 1;

                        for (int d = 0; d < 1 + extraDustAmt; d++)
                        {
                            float scalar = 1.2f;
                            if (d % 2 == 1)
                                scalar = 2.8f;

                            Vector2 dustPos = shootFrom + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * dustPosOffset / 2f;
                            int index = Dust.NewDust(dustPos - Vector2.One * 8f, 16, 16, (int)CalamityDusts.ProfanedFire, NPC.velocity.X / 2f, NPC.velocity.Y / 2f, 0, default, 1f);
                            Main.dust[index].velocity = Vector2.Normalize(NPC.Center - dustPos) * 3.5f * (10f - extraDustAmt * 2f) / 10f;
                            Main.dust[index].noGravity = true;
                            Main.dust[index].scale = scalar;
                        }
                    }
                }
                else if (NPC.ai[2] < (revenge ? 220f : 300f))
                {
                    if (NPC.ai[2] == laserGateValue)
                    {
                        float rotation = (bossRush || biomeEnraged) ? 435f : death ? 445f : revenge ? 450f : expertMode ? 455f : 465f;

                        if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < 2800f)
                            SoundEngine.PlaySound(HolyRaySound, Main.LocalPlayer.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 laserVelocity = player.Center - NPC.Center;
                            laserVelocity.Normalize();

                            float beamDirection = -1f;
                            if (laserVelocity.X < 0f)
                                beamDirection = 1f;

                            int type = ModContent.ProjectileType<ProvidenceHolyRay>();
                            int damage = NPC.GetProjectileDamage(type);

                            // 60 degrees offset
                            laserVelocity = laserVelocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 6f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, laserVelocity, type, damage, 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);

                            // -60 degrees offset
                            if (revenge)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, -laserVelocity, type, damage, 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);

                            if (CalamityWorld.LegendaryMode && revenge)
                            {
                                rotation *= 0.33f;
                                laserVelocity = laserVelocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 2f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, laserVelocity, ModContent.ProjectileType<ProvidenceHolyRay>(), damage, 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, -laserVelocity, ModContent.ProjectileType<ProvidenceHolyRay>(), damage, 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);
                            }

                            NPC.netUpdate = true;
                        }
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (revenge ? 235f : 315f))
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    calamityGlobalNPC.newAI[2] = 1f;
                    NPC.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            void drawGuardianInstance(Vector2 drawOffset, Color? colorOverride)
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
                        if (colorOverride != null)
                            color38 = colorOverride.Value;

                        Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                        spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }
                }

                Vector2 vector43 = drawPos;
                vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                spriteBatch.Draw(texture2D15, vector43, NPC.frame, colorOverride ?? NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommanderGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                if (Main.remixWorld)
                {
                    texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianCommanderGlowNight").Value;
                    color37 = Color.Cyan;
                }
                if (colorOverride != null)
                    color37 = colorOverride.Value;

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
                    }
                }

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            // Draw laser effects
            float useLaserGateValue = 120f;
            float stopLaserGateValue = (CalamityWorld.revenge || BossRushEvent.BossRushActive) ? 235f : 315f;
            float maxIntensity = 45f;
            float increaseIntensityGateValue = useLaserGateValue - maxIntensity;
            float decreaseIntensityGateValue = stopLaserGateValue - maxIntensity;
            bool usingLaser = NPC.ai[0] == 5f;
            bool increaseIntensity = NPC.ai[1] > increaseIntensityGateValue;
            bool decreaseIntensity = NPC.ai[1] > decreaseIntensityGateValue;
            if (usingLaser)
            {
                float burnIntensity = decreaseIntensity ? Utils.GetLerpValue(0f, maxIntensity, maxIntensity - (NPC.ai[1] - decreaseIntensityGateValue), true) : Utils.GetLerpValue(0f, maxIntensity, NPC.ai[1], true);
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

            if (NPC.IsABestiaryIconDummy)
                return false;

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

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.SuperHealingPotion;

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

            // GFB Chicken Nugget and Divine Geode drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ItemID.ChickenNugget, 1, 1, 9999);
                GFBOnly.Add(ModContent.ItemType<DivineGeode>(), 1, 25, 30);
            }

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

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300, true);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);

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
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
