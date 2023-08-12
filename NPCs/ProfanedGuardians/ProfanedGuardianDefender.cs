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
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.ProfanedGuardians
{
    [AutoloadBossHead]
    public class ProfanedGuardianDefender : ModNPC
    {
        private int healTimer = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private const float TimeForShieldDespawn = 120f;
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianDash");
        public static readonly SoundStyle RockShieldSpawnSound = new("CalamityMod/Sounds/Custom/ProfanedGuardians/GuardianRockShieldActivate");
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
            NPC.defense = 50;
            NPC.DR_NERD(0.4f);
            NPC.LifeMaxNERB(40000, 48000, 35000);
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
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ProfanedGuardianDefender")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(healTimer);
            writer.Write(biomeEnrageTimer);
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            healTimer = reader.ReadInt32();
            biomeEnrageTimer = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
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
            CalamityGlobalNPC.doughnutBossDefender = NPC.whoAmI;

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1.1f, 0.9f, 0f);

            if (CalamityGlobalNPC.doughnutBoss < 0 || !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.active = false;
                NPC.netUpdate = true;
                return;
            }

            // Projectile and dust spawn location variables
            Vector2 dustAndProjectileOffset = new Vector2(40f * NPC.direction, 20f);
            Vector2 shootFrom = NPC.Center + dustAndProjectileOffset;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.005f;

            bool healerAlive = false;
            if (CalamityGlobalNPC.doughnutBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.doughnutBossHealer].active)
                    healerAlive = true;
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
                    // gain more health once the healer's channel heal is done
                    NPC.lifeMax += 7500;
                    NPC.life += NPC.lifeMax - NPC.life;
                    NPC.HealEffect(NPC.lifeMax - NPC.life, true);
                    NPC.netUpdate = true;
                }
            }

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

            // Become immune over time if target isn't in hell or hallow
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

            bool phase1 = healerAlive;

            NPC.chaseable = !phase1;

            // Phase durations
            float commanderGuardPhase2Duration = (bossRush || biomeEnraged) ? 420f : death ? 480f : revenge ? 510f : expertMode ? 540f : 600f;
            float timeBeforeRocksRespawnInPhase2 = 90f;
            float throwRocksGateValue = 60f;

            // Distance
            float distanceInFrontOfCommander = 160f;

            // Charge variables
            float chargeVelocityMult = 0.25f;
            float maxChargeVelocity = (bossRush || biomeEnraged) ? 25f : death ? 22f : revenge ? 20.5f : expertMode ? 19f : 16f;
            if (Main.getGoodWorld)
                maxChargeVelocity *= 1.15f;

            // Whether the commander is calling all guardians together for the laser attack
            bool commanderUsingLaser = Main.npc[CalamityGlobalNPC.doughnutBoss].ai[0] == 5f;

            // Go low just before moving to the other side to avoid bullshit hits
            float moveToOtherSideInPhase1GateValue = 900f;
            float timeBeforeMoveToOtherSideInPhase1Reset = moveToOtherSideInPhase1GateValue * 2f;
            float totalGoLowDurationPhase1 = 240f;
            float goLowDurationPhase1 = totalGoLowDurationPhase1 * 0.5f;
            float roundedGoLowPhase1Check = (float)Math.Round(goLowDurationPhase1 * 0.5);
            bool commanderGoingLowOrHighInPhase1 = (Main.npc[CalamityGlobalNPC.doughnutBoss].localAI[3] > (moveToOtherSideInPhase1GateValue - goLowDurationPhase1) &&
                Main.npc[CalamityGlobalNPC.doughnutBoss].localAI[3] <= (moveToOtherSideInPhase1GateValue + roundedGoLowPhase1Check)) ||
                Main.npc[CalamityGlobalNPC.doughnutBoss].localAI[3] > (timeBeforeMoveToOtherSideInPhase1Reset - goLowDurationPhase1) ||
                Main.npc[CalamityGlobalNPC.doughnutBoss].localAI[3] <= (-roundedGoLowPhase1Check);

            float moveToOtherSideInPhase2GateValue = commanderGuardPhase2Duration - 120f;
            float timeBeforeMoveToOtherSideInPhase2Reset = moveToOtherSideInPhase2GateValue * 2f;
            float totalGoLowDurationPhase2 = 210f;
            float goLowDurationPhase2 = totalGoLowDurationPhase2 * 0.5f;
            float roundedGoLowPhase2Check = (float)Math.Round(goLowDurationPhase2 * 0.5);
            bool commanderGoingLowOrHighInPhase2 = (Main.npc[CalamityGlobalNPC.doughnutBoss].Calamity().newAI[1] > (moveToOtherSideInPhase2GateValue - goLowDurationPhase2) &&
                Main.npc[CalamityGlobalNPC.doughnutBoss].Calamity().newAI[1] <= (moveToOtherSideInPhase2GateValue + roundedGoLowPhase2Check)) ||
                Main.npc[CalamityGlobalNPC.doughnutBoss].Calamity().newAI[1] > (timeBeforeMoveToOtherSideInPhase2Reset - goLowDurationPhase2) ||
                Main.npc[CalamityGlobalNPC.doughnutBoss].Calamity().newAI[1] <= (-roundedGoLowPhase2Check);

            // Tell rocks to fade out and shrink
            if (commanderGoingLowOrHighInPhase1 || commanderGoingLowOrHighInPhase2)
                NPC.localAI[3] = 1f;
            else
                NPC.localAI[3] = 0f;

            // Spawn rock shield
            bool respawnRocksInPhase2 = NPC.ai[1] == (-commanderGuardPhase2Duration + timeBeforeRocksRespawnInPhase2) && !commanderGoingLowOrHighInPhase2;
            int rockTypes = 6;
            int maxRocks = respawnRocksInPhase2 ? 18 : 36;
            int rockRings = 3;
            int totalRocksPerRing = maxRocks / rockRings;
            int spacing = 360 / totalRocksPerRing;
            int distance2 = 200;
            bool justSpawnedRocks = false;
            if (NPC.localAI[0] == 0f || respawnRocksInPhase2)
            {
                justSpawnedRocks = true;
                NPC.localAI[0] = 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < totalRocksPerRing; i++)
                    {
                        int rockType = Main.rand.Next(rockTypes) + 1;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 0f, rockType, 0f);
                        rockType = Main.rand.Next(rockTypes) + 1;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 1f, rockType, 0f);
                        rockType = Main.rand.Next(rockTypes) + 1;
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 2f, rockType, 0f);
                    }
                }
            }

            // Generate new rock shields if too many are broken in phase 1
            if (phase1)
            {
                int minRocks = maxRocks / 2;
                int numRockShields = NPC.CountNPCS(ModContent.NPCType<ProfanedRocks>());
                if (numRockShields < minRocks)
                {
                    justSpawnedRocks = true;
                    totalRocksPerRing = minRocks / rockRings;
                    spacing = 360 / totalRocksPerRing;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < totalRocksPerRing; i++)
                        {
                            int rockType = Main.rand.Next(rockTypes) + 1;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 0f, rockType, 0f);
                            rockType = Main.rand.Next(rockTypes) + 1;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 1f, rockType, 0f);
                            rockType = Main.rand.Next(rockTypes) + 1;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), ModContent.NPCType<ProfanedRocks>(), NPC.whoAmI, i * spacing, 2f, rockType, 0f);
                        }
                    }
                }
            }
            else
            {
                if (NPC.localAI[1] < TimeForShieldDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[1] == 0f)
                        SoundEngine.PlaySound(ShieldDeathSound, NPC.Center);

                    NPC.localAI[1] += 1f;
                }
            }

            // Spawn three dust circles and play noise whenever rock shields are spawned
            if (justSpawnedRocks)
            {
                // Meteor Staff use sound and dust circles
                SoundEngine.PlaySound(RockShieldSpawnSound, NPC.Center);
                int totalDust = maxRocks;
                for (int j = 0; j < rockRings; j++)
                {
                    for (int k = 0; k < totalDust; k++)
                    {
                        Vector2 dustSpawnPos = NPC.velocity.SafeNormalize(Vector2.UnitY) * new Vector2(distance2, distance2);
                        dustSpawnPos = dustSpawnPos.RotatedBy((double)((k - (totalDust / 2 - 1)) * MathHelper.TwoPi / totalDust), default) + NPC.Center;
                        Vector2 dustVelocity = dustSpawnPos - NPC.Center;
                        Color dustColor = Main.hslToRgb(Main.rgbToHsl(Color.Orange).X, 1f, 0.5f);
                        dustColor.A = 255;
                        int dust = Dust.NewDust(dustSpawnPos + dustVelocity, 0, 0, 267, dustVelocity.X, dustVelocity.Y, 0, dustColor, 1.4f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                        Main.dust[dust].velocity = dustVelocity * (j * 0.1f + 0.1f);
                    }
                }
            }

            float moveVelocity = (bossRush || biomeEnraged) ? 24f : death ? 22f : revenge ? 21f : expertMode ? 20f : 18f;
            if (Main.getGoodWorld)
                moveVelocity *= 1.25f;
            if (healerAlive)
                moveVelocity *= 0.8f;

            float distanceToStayAwayFromTarget = 800f;
            bool speedUp = Vector2.Distance(NPC.Center, player.Center) > (distanceToStayAwayFromTarget + 160f);
            if (speedUp)
                moveVelocity *= 2f;
            if (commanderGoingLowOrHighInPhase2)
                moveVelocity *= 2f;
            else if (commanderGoingLowOrHighInPhase1)
                moveVelocity *= 1.66f;

            if (NPC.ai[0] == 0f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                // Slow down and tell the Profaned Rocks to spin and fling themselves at the target by setting NPC.ai[0] = 1f
                if (!phase1)
                {
                    NPC.velocity *= 0.9f;
                    if (NPC.velocity.Length() <= 2f)
                        NPC.velocity = Vector2.Zero;

                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= throwRocksGateValue)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }

                    return;
                }

                // Lay a holy bomb every once in a while in phase 1 and while not doing the laser attack
                if (!commanderUsingLaser)
                {
                    float projectileShootGateValue = (bossRush || biomeEnraged) ? 420f : death ? 480f : revenge ? 510f : expertMode ? 540f : 600f;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= projectileShootGateValue)
                    {
                        NPC.ai[1] = 0f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;
                            Vector2 projectileVelocity = new Vector2(NPC.velocity.X * 0.25f, projectileVelocityY);
                            int type = ModContent.ProjectileType<HolyBomb>();
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, projectileVelocity, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }

                // Defend the commander
                Vector2 distanceFromDestination = Main.npc[CalamityGlobalNPC.doughnutBoss].Center + Vector2.UnitX * ((commanderUsingLaser || commanderGoingLowOrHighInPhase1) ? 0f : distanceInFrontOfCommander) * Main.npc[CalamityGlobalNPC.doughnutBoss].direction - NPC.Center;
                Vector2 desiredVelocity = distanceFromDestination.SafeNormalize(new Vector2(NPC.direction, 0f)) * moveVelocity;
                if (distanceFromDestination.Length() > 40f)
                {
                    float inertia = (commanderUsingLaser || commanderGoingLowOrHighInPhase1) ? 10f : 15f;
                    if (Main.getGoodWorld)
                        inertia *= 0.8f;

                    NPC.velocity = (NPC.velocity * (inertia - 1) + desiredVelocity) / inertia;
                }
                else
                    NPC.velocity *= 0.9f;
            }

            // Phase 2
            // Throw rocks, charge, summon holy bombs and shoot molten blasts
            else if (NPC.ai[0] == 1f)
            {
                // Face the target
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }

                // Do not increment the phase timer while swapping sides along with the commander in phase 2
                if (!commanderGoingLowOrHighInPhase2)
                    NPC.ai[1] += 1f;

                // Slow down before throwing rock shields
                if (NPC.ai[1] >= -throwRocksGateValue)
                {
                    NPC.velocity *= 0.8f;
                    if (Main.getGoodWorld)
                        NPC.velocity *= 0.5f;
                }

                // Defend the commander
                else
                {
                    // Shoot molten blasts
                    int moltenBlastsDivisor = 4;
                    float shootMoltenBlastsGateValue = commanderGuardPhase2Duration / moltenBlastsDivisor;
                    if (NPC.ai[1] % shootMoltenBlastsGateValue == 0f && !commanderGoingLowOrHighInPhase2)
                    {
                        float moltenBlastVelocity = (bossRush || biomeEnraged) ? 18f : death ? 16f : revenge ? 15f : expertMode ? 14f : 12f;
                        int projTimeLeft = (int)(2400f / moltenBlastVelocity);
                        Vector2 velocity = Vector2.Normalize(player.Center - shootFrom) * moltenBlastVelocity;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<MoltenBlast>();
                            int damage = NPC.GetProjectileDamage(type);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, velocity, type, damage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                                Main.projectile[proj].timeLeft = projTimeLeft;
                            }
                        }

                        // Dust for blasting out the molten blasts
                        for (int i = 0; i < 50; i++)
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
                            float dustSpeed = Main.rand.NextFloat(moltenBlastVelocity * 0.5f, moltenBlastVelocity);
                            float angleRandom = 0.06f;
                            Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(velocity.ToRotation());
                            dustVel = dustVel.RotatedBy(-angleRandom);
                            dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                            // Pick a size for the fire particles
                            float scale = Main.rand.NextFloat(1f, 2f);

                            // Actually spawn the fire
                            int idx = Dust.NewDust(shootFrom, 42, 42, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                            Main.dust[idx].noGravity = true;
                        }

                        NPC.velocity = -velocity * 0.5f;
                    }

                    Vector2 distanceFromDestination = Main.npc[CalamityGlobalNPC.doughnutBoss].Center + (commanderGoingLowOrHighInPhase2 ? Vector2.Zero : (Vector2.UnitX * distanceInFrontOfCommander * Main.npc[CalamityGlobalNPC.doughnutBoss].direction)) - NPC.Center;
                    Vector2 desiredVelocity = distanceFromDestination.SafeNormalize(new Vector2(NPC.direction, 0f)) * moveVelocity;
                    if (distanceFromDestination.Length() > 40f)
                    {
                        float inertia = commanderGoingLowOrHighInPhase2 ? 8f : 15f;
                        if (Main.getGoodWorld)
                            inertia *= 0.8f;

                        NPC.velocity = (NPC.velocity * (inertia - 1) + desiredVelocity) / inertia;
                    }
                    else
                        NPC.velocity *= 0.9f;
                }

                // Charge at target
                if (NPC.ai[1] >= 0f)
                {
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
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
            }

            // Charge at the target and spawn holy bombs
            else if (NPC.ai[0] == 2f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] += 1f;
                float phaseGateValue = (bossRush || biomeEnraged) ? 120f : death ? 140f : revenge ? 150f : expertMode ? 160f : 180f;
                if (NPC.ai[1] >= phaseGateValue)
                {
                    NPC.ai[0] = 3f;

                    // Slown down duration ranges from 30 to 60 frames in rev, otherwise it's always 60 frames
                    float slowDownDurationAfterCharge = revenge ? Main.rand.Next(30, 61) : 60f;
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
                            float velocityMult = (bossRush || biomeEnraged) ? 1.04f : death ? 1.036667f : revenge ? 1.035f : expertMode ? 1.033333f : 1.03f;
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
                        // Charge towards target
                        float inertia = (bossRush || biomeEnraged) ? 57f : death ? 63f : revenge ? 66f : expertMode ? 69f : 75f;
                        float num1006 = 0.111111117f * inertia;
                        NPC.velocity = (NPC.velocity * (inertia - 1f) + targetVector * (NPC.velocity.Length() + num1006)) / inertia;
                    }

                    // Lay holy bombs while charging
                    int projectileGateValue = (int)(phaseGateValue * 0.4f);
                    if (NPC.ai[1] % projectileGateValue == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;
                            Vector2 projectileVelocity = new Vector2(NPC.velocity.X * 0.25f, projectileVelocityY);
                            int type = ModContent.ProjectileType<HolyBomb>();
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom, projectileVelocity, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            // Slow down and pause for a bit
            else if (NPC.ai[0] == 3f)
            {
                if (Math.Sign(NPC.velocity.X) != 0)
                    NPC.spriteDirection = -Math.Sign(NPC.velocity.X);
                NPC.spriteDirection = Math.Sign(NPC.velocity.X);

                NPC.ai[1] -= 1f;
                if (NPC.ai[1] <= 0f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[2] += 1f;

                    // Charges either once or twice in a row in rev, otherwise it will always charge twice in a row
                    float totalCharges = revenge ? (Main.rand.Next(2) + 1f) : 2f;
                    bool dontCharge = NPC.ai[2] >= totalCharges;
                    NPC.ai[1] = dontCharge ? -commanderGuardPhase2Duration : 0f;
                    if (dontCharge)
                        NPC.ai[2] = 0f;

                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.97f;
            }

            if (Main.zenithWorld)
            {
                if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }
                // Block the main guardian
                Vector2 guardPos = Main.npc[CalamityGlobalNPC.doughnutBoss].Center;
                Vector2 playerPos = player.Center;
                Vector2 midPoint = ((guardPos - playerPos) / 1.25f) + playerPos;
                NPC.position = midPoint;
                return;
            }
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

                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianDefenderGlow").Value;
                Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                if (Main.remixWorld)
                {
                    texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/ProfanedGuardians/ProfanedGuardianDefenderGlowNight").Value;
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

            if (NPC.IsABestiaryIconDummy)
                return false;

            // Draw shields while healer is alive
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
                float minHue = 0.06f;
                float maxHue = 0.18f;
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

        public override void ModifyNPCLoot(NPCLoot npcLoot) => npcLoot.Add(ModContent.ItemType<RelicOfResilience>(), 4);

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
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 240, true);
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // eat projectiles but take more damage based on piercing in the zenith seed
            if (Main.zenithWorld && !projectile.minion)
            {
                if (projectile.penetrate <= -1 || projectile.penetrate > 5)
                {
                    modifiers.SourceDamage *= 2.5f;
                }
                else
                {
                    modifiers.SourceDamage *= projectile.penetrate / 2f;
                }
                projectile.active = false;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossT").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossT2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossT3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ProfanedGuardianBossT4").Type, 1f);
                }

                for (int k = 0; k < 50; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.ProfanedFire, hit.HitDirection, -1f, 0, default, 1f);
            }
        }
    }
}
