using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Providence
{
    [AutoloadBossHead]
    public class Providence : ModNPC
    {
        private enum Phase
        {
            PhaseChange = -1,
            HolyBlast = 0,
            HolyFire = 1,
            FlameCocoon = 2,
            MoltenBlobs = 3,
            HolyBomb = 4,
            SpearCocoon = 5,
            Crystal = 6,
            Laser = 7
        }

        private float AIState
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }

        private bool text = false;
        private bool useDefenseFrames = false;
        private float bossLife;
        private int biomeType = 0;
        private int flightPath = 0;
        private int phaseChange = 0;
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int frameUsed = 0;
        private int healTimer = 0;
        internal bool challenge = Main.expertMode/* && Main.netMode == NetmodeID.SinglePlayer*/; //Used to determine if Profaned Soul Crystal should drop, couldn't figure out mp mems always dropping it so challenge is singleplayer only.
        internal bool hasTakenDaytimeDamage = false;
        public bool Dying = false;
        public int DeathAnimationTimer;

        public static float normalDR = 0.3f;
        public static float cocoonDR = 0.9f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Providence, the Profaned Goddess");
            Main.npcFrameCount[npc.type] = 3;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 36f;
            npc.damage = 100;
            npc.width = 600;
            npc.height = 450;
            npc.defense = 50;
            npc.DR_NERD(normalDR);
            npc.LifeMaxNERB(312500, 375000, 1250000); // Old HP - 440000, 500000
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(3, 0, 0, 0);
            npc.boss = true;
            npc.Opacity = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("Providence") ?? MusicID.LunarBoss;
            npc.DeathSound = mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/ProvidenceDeath");
            bossBag = ModContent.ItemType<ProvidenceBag>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(text);
            writer.Write(useDefenseFrames);
            writer.Write(biomeType);
            writer.Write(phaseChange);
            writer.Write(biomeEnrageTimer);
            writer.Write(frameUsed);
            writer.Write(healTimer);
            writer.Write(flightPath);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
            writer.Write(npc.canGhostHeal);
            writer.Write(npc.localAI[2]);
            writer.Write(Dying);
            writer.Write(DeathAnimationTimer);
            for (int i = 0; i < 4; i++)
                writer.Write(npc.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool wasDyingBefore = Dying;
            text = reader.ReadBoolean();
            useDefenseFrames = reader.ReadBoolean();
            biomeType = reader.ReadInt32();
            phaseChange = reader.ReadInt32();
            biomeEnrageTimer = reader.ReadInt32();
            frameUsed = reader.ReadInt32();
            healTimer = reader.ReadInt32();
            flightPath = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
            npc.canGhostHeal = reader.ReadBoolean();
            npc.localAI[2] = reader.ReadSingle();
            Dying = reader.ReadBoolean();
            DeathAnimationTimer = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                npc.Calamity().newAI[i] = reader.ReadSingle();

            // Be sure to inform clients of the fact that Providence is dying if only the server recieved this packet.
            if (Main.netMode == NetmodeID.Server && !wasDyingBefore && Dying)
            {
                npc.netSpam = 0;
                npc.netUpdate = true;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable for Guardians and other things
            CalamityGlobalNPC.holyBoss = npc.whoAmI;

            // Rotation
            npc.rotation = npc.velocity.X * 0.004f;

            Vector2 vector = npc.Center;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, vector) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Target variable and boss center
            Player player = Main.player[npc.target];

            // Night bool
            bool malice = CalamityWorld.malice;
            bool nightTime = !Main.dayTime || malice;

            // Difficulty bools
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive || nightTime;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive || nightTime;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive || nightTime;

            // Target's current biome
            bool isHoly = player.ZoneHoly;
            bool isHell = player.ZoneUnderworldHeight;

            // Fire projectiles at normal rate or not
            bool normalAttackRate = true;

            // Is in spawning animation
            float spawnAnimationTime = 180f;
            bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Play enrage animation if night starts
            if (nightTime && calamityGlobalNPC.newAI[3] == spawnAnimationTime)
            {
                AIState = (int)Phase.HolyBlast;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                calamityGlobalNPC.newAI[1] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
                calamityGlobalNPC.newAI[3] = 0f;
                npc.netUpdate = true;
            }

            // Increase all projectile damage at night
            int projectileDamageMult = 1;
            if (nightTime)
                projectileDamageMult = 2;

            npc.Calamity().CurrentlyEnraged = (!BossRushEvent.BossRushActive && (nightTime || malice)) || biomeEnrageTimer <= 0;

            // Projectile damage values
            int holyLaserDamage = npc.GetProjectileDamage(ModContent.ProjectileType<ProvidenceHolyRay>()) * projectileDamageMult;
            int crystalDamage = npc.GetProjectileDamage(ModContent.ProjectileType<ProvidenceCrystal>()) * projectileDamageMult;
            int holySpearDamage = npc.GetProjectileDamage(ModContent.ProjectileType<HolySpear>()) * projectileDamageMult;
            int holyBombDamage = npc.GetProjectileDamage(ModContent.ProjectileType<HolyBomb>()) * projectileDamageMult;
            int moltenBlastDamage = npc.GetProjectileDamage(ModContent.ProjectileType<MoltenBlast>()) * projectileDamageMult;
            int holyFireDamage = npc.GetProjectileDamage(ModContent.ProjectileType<HolyFire>()) * projectileDamageMult;
            int holyBlastDamage = npc.GetProjectileDamage(ModContent.ProjectileType<HolyBlast>()) * projectileDamageMult;
            int holyStarDamage = npc.GetProjectileDamage(ModContent.ProjectileType<HolyBurnOrb>()) * projectileDamageMult;

            // Change dust type at night
            int dustType = Main.dayTime ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;

            // Phase times
            float phaseTime = nightTime ? (240f - 60f * (1f - lifeRatio)) : 300f;
            float crystalPhaseTime = nightTime ? (float)Math.Round(60f * lifeRatio) : death ? 60f : 120f;
            int nightCrystalTime = 210;
            float attackDelayAfterCocoon = phaseTime * 0.3f;

            // Phases
            bool ignoreGuardianAmt = lifeRatio < (death ? 0.2f : 0.15f);
            bool phase2 = lifeRatio < 0.75f && !nightTime;
            bool delayAttacks = npc.localAI[2] > 0f;

            // Spear phase
            float spearRateIncrease = 1f - lifeRatio;
            float bossRushSpearRateIncrease = 0.25f;
            float baseSpearRate = 18f;
            float spearRate = 1f + spearRateIncrease;

            if (BossRushEvent.BossRushActive)
                spearRate += bossRushSpearRateIncrease;

            // Projectile fire rate multiplier
            double attackRateMult = 1D;

            // Where projectiles are fired from during cocoon phases
            Vector2 fireFrom = new Vector2(vector.X, vector.Y + 20f);

            // Cocoon projectile initial velocity
            float cocoonProjVelocity = 3f + (death ? 2f * (1f - lifeRatio) : 0f);

            // Distance X needed from target in order to fire holy or molten blasts
            float distanceNeededToShoot = death ? 300f : revenge ? 360f : 420f;

            // X distance from target
            float distanceX = Math.Abs(vector.X - player.Center.X);

            // Inflict Holy Inferno if target is too far away
            float baseDistance = 2800f;
            float shorterFlameCocoonDistance = (CalamityWorld.death || BossRushEvent.BossRushActive || nightTime) ? 2200f : CalamityWorld.revenge ? 2400f : Main.expertMode ? 2600f : baseDistance;
            float shorterSpearCocoonDistance = (CalamityWorld.death || BossRushEvent.BossRushActive || nightTime) ? 1800f : CalamityWorld.revenge ? 2150f : Main.expertMode ? 2500f : baseDistance;
            float shorterDistance = AIState == (int)Phase.FlameCocoon ? shorterFlameCocoonDistance : shorterSpearCocoonDistance;
            float maxDistance = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon) ? shorterDistance : baseDistance;
            if (Vector2.Distance(player.Center, vector) > maxDistance)
            {
                if (!player.dead && player.active)
                    player.AddBuff(ModContent.BuffType<HolyInferno>(), 2);
            }

            // Count the remaining Guardians, healer especially because it allows the boss to heal
            int guardianAmt = 0;
            bool attackerAlive = false;
            bool defenderAlive = false;
            bool healerAlive = false;
            if (CalamityGlobalNPC.holyBossAttacker != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                {
                    guardianAmt++;
                    attackerAlive = true;
                }
            }
            if (CalamityGlobalNPC.holyBossDefender != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossDefender].active)
                {
                    guardianAmt++;
                    defenderAlive = true;
                }
            }
            if (CalamityGlobalNPC.holyBossHealer != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBossHealer].active)
                {
                    guardianAmt++;
                    healerAlive = true;
                }
            }

            // Change projectile fire rate depending on Guardian amount
            if (guardianAmt > 0)
            {
                normalAttackRate = ignoreGuardianAmt;
                if (!normalAttackRate)
                {
                    switch (guardianAmt)
                    {
                        case 1:
                            attackRateMult = 1.15;
                            break;
                        case 2:
                            attackRateMult = 1.3;
                            break;
                        case 3:
                            attackRateMult = 1.45;
                            break;
                        default:
                            break;
                    }
                }
            }

            // Whether the boss can be homed in on or healed off of
            npc.chaseable = normalAttackRate && AIState != (int)Phase.FlameCocoon && AIState != (int)Phase.SpearCocoon && AIState != (int)Phase.Laser;
            npc.canGhostHeal = npc.chaseable;

            // Prevent lag by stopping rain
            CalamityMod.StopRain();

            // Set target biome type
            if (biomeType == 0)
            {
                if (isHoly)
                    biomeType = 1;
                else if (isHell)
                    biomeType = 2;
            }

            // Become immune over time if target isn't in hell or hallow
            if (!isHoly && !isHell && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            // Take damage or not
            bool biomeEnraged = biomeEnrageTimer <= 0 || malice;
            npc.dontTakeDamage = Dying;

            // Do the death animation once killed.
            if (Dying)
            {
                DoDeathAnimation();
                return;
            }

            // Damage
            if (attackerAlive)
            {
                double damageMult = 1.25;
                holyLaserDamage = (int)(holyLaserDamage * damageMult);
                crystalDamage = (int)(crystalDamage * damageMult);
                holySpearDamage = (int)(holySpearDamage * damageMult);
                holyBombDamage = (int)(holyBombDamage * damageMult);
                moltenBlastDamage = (int)(moltenBlastDamage * damageMult);
                holyFireDamage = (int)(holyFireDamage * damageMult);
                holyBlastDamage = (int)(holyBlastDamage * damageMult);
                holyStarDamage = (int)(holyStarDamage * damageMult);
            }

            // Defense
            if (defenderAlive)
                npc.defense = npc.defDefense * 2;
            else
                npc.defense = npc.defDefense;

            // Healing
            if (healerAlive)
            {
                float healGateValue = revenge ? 60f : 90f;
                healTimer++;
                if (healTimer >= healGateValue)
                {
                    healTimer = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = npc.lifeMax / 200;
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

            // Despawn
            bool targetDead = false;
            if (!player.active || player.dead)
            {
                if (!player.active || player.dead)
                {
                    npc.TargetClosest(false);
                    player = Main.player[npc.target];
                }
                if (!player.active || player.dead)
                {
                    targetDead = true;

                    if (npc.timeLeft > 60)
                        npc.timeLeft = 60;

                    if (npc.velocity.X > 0f)
                        npc.velocity.X += 0.2f;
                    else
                        npc.velocity.X -= 0.2f;

                    npc.velocity.Y -= 0.2f;
                }
            }
            else if (npc.timeLeft < 1800)
                npc.timeLeft = 1800;

            // Guardian spawn
            if (!nightTime)
            {
                if (bossLife == 0f && npc.life > 0)
                    bossLife = npc.lifeMax;

                if (npc.life > 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num660 = (int)(npc.lifeMax * 0.66);
                        if ((npc.life + num660) < bossLife)
                        {
                            bossLife = npc.life;
                            Main.PlaySound(SoundID.Item, (int)npc.position.X, (int)npc.position.Y, 74);
                            int guardianRingAmt = 3;
                            int guardianSpread = 360 / guardianRingAmt;
                            int guardianDistance = 400;
                            for (int i = 0; i < guardianRingAmt; i++)
                            {
                                int type = i == 0 ? ModContent.NPCType<ProvSpawnDefense>() : i == 1 ? ModContent.NPCType<ProvSpawnHealer>() : ModContent.NPCType<ProvSpawnOffense>();
                                int spawn = NPC.NewNPC((int)(npc.Center.X + (Math.Sin(i * guardianSpread) * guardianDistance)), (int)(npc.Center.Y + (Math.Cos(i * guardianSpread) * guardianDistance)), type, npc.whoAmI, 0, 0, 0, -1);
                                Main.npc[spawn].ai[0] = i * guardianSpread;
                            }
                        }
                    }
                }
            }

            // Set DR based on current attack phase
            npc.Calamity().DR = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation) ?
                cocoonDR : delayAttacks ?
                MathHelper.Lerp(normalDR, cocoonDR, npc.localAI[2] / attackDelayAfterCocoon) : normalDR;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation;

            // Movement
            if (AIState != (int)Phase.FlameCocoon && AIState != (int)Phase.SpearCocoon)
            {
                // Firing holy ray or not
                bool firingLaser = AIState == (int)Phase.Laser;

                // Change X direction of movement
                if (flightPath == 0)
                {
                    if (vector.X < player.Center.X)
                    {
                        flightPath = 1;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                    else
                    {
                        flightPath = -1;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }

                // Increase speed over time if flying in same direction for too long
                if (revenge)
                    calamityGlobalNPC.newAI[0] += 1f;

                // Distance needed from target to change direction
                float num851 = 800f;

                // Increase distance from target when firing molten blasts or holy bombs
                bool stayAwayFromTarget = AIState == (int)Phase.MoltenBlobs || AIState == (int)Phase.HolyBomb;
                if (stayAwayFromTarget)
                    num851 += death ? 240f : revenge ? 180f : 120f;

                // Change X movement path if far enough away from target
                if (vector.X < player.Center.X && flightPath < 0 && distanceX > num851)
                    flightPath = 0;
                if (vector.X > player.Center.X && flightPath > 0 && distanceX > num851)
                    flightPath = 0;

                // Velocity and acceleration
                float speedIncreaseTimer = nightTime ? 75f : death ? 120f : 150f;
                bool increaseSpeed = calamityGlobalNPC.newAI[0] > speedIncreaseTimer || biomeEnraged;
                float accelerationBoost = death ? 0.3f * (1f - lifeRatio) : 0.2f * (1f - lifeRatio);
                float velocityBoost = death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                float acceleration = (expertMode ? 1.1f : 1.05f) + accelerationBoost;
                float velocity = (expertMode ? 16f : 15f) + velocityBoost;
                if (BossRushEvent.BossRushActive || nightTime)
                {
                    acceleration = 1.5f;
                    velocity = 25f;
                }
                if (firingLaser)
                {
                    acceleration *= 0.4f;
                    velocity *= 0.4f;
                }
                else if (increaseSpeed)
                {
                    velocity += (calamityGlobalNPC.newAI[0] - speedIncreaseTimer) * 0.04f;
                    if (velocity > 30f || biomeEnraged)
                        velocity = 30f;
                    if (biomeEnraged)
                        acceleration = 2f;
                }

                if (!targetDead)
                {
                    npc.velocity.X += flightPath * acceleration;
                    if (npc.velocity.X > velocity)
                        npc.velocity.X = velocity;
                    if (npc.velocity.X < -velocity)
                        npc.velocity.X = -velocity;

                    float num855 = player.position.Y - (npc.position.Y + npc.height);
                    if (num855 < (firingLaser ? 150f : 200f)) // 150
                        npc.velocity.Y -= 0.2f;
                    if (num855 > (firingLaser ? 200f : 250f)) // 200
                        npc.velocity.Y += 0.2f;

                    float speedVariance = 2f;
                    if (npc.velocity.Y > (firingLaser ? speedVariance : 6f))
                        npc.velocity.Y = firingLaser ? speedVariance : 6f;
                    if (npc.velocity.Y < (firingLaser ? -speedVariance : -6f))
                        npc.velocity.Y = firingLaser ? -speedVariance : -6f;
                }

                // Slowly drift down when spawning
                if (spawnAnimation)
                {
                    float minSpawnVelocity = 0.4f;
                    float maxSpawnVelocity = 4f;
                    float velocityY = maxSpawnVelocity - MathHelper.Lerp(minSpawnVelocity, maxSpawnVelocity, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
                    npc.velocity = new Vector2(0f, velocityY);
                }
            }

            // Phase switch
            switch ((int)AIState)
            {
                case (int)Phase.PhaseChange:

                    phaseChange++;
                    if (phaseChange > 14)
                        phaseChange = 0;

                    int phase = 0;

                    // Holy ray in hallow, Crystal in hell
                    bool useLaser = (phase2 && biomeType == 1) || BossRushEvent.BossRushActive;
                    bool useCrystal = (phase2 && biomeType == 2) || BossRushEvent.BossRushActive;

                    // Unique pattern for Death Mode and Boss Rush
                    if (death)
                    {
                        switch (phaseChange)
                        {
                            case 0:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 1:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 2:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 3:
                                phase = (useCrystal || nightTime) ? (int)Phase.Crystal : (int)Phase.MoltenBlobs;
                                break;
                            case 4:
                                phase = useCrystal ? (int)Phase.MoltenBlobs : (int)Phase.FlameCocoon;
                                break;
                            case 5:
                                phase = useCrystal ? (int)Phase.FlameCocoon : (int)Phase.HolyFire;
                                break;
                            case 6:
                                phase = (useLaser || nightTime) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 7:
                                phase = (useLaser || nightTime) ? (int)Phase.HolyBomb : (int)Phase.MoltenBlobs;
                                break;
                            case 8:
                                phase = (useLaser || nightTime) ? (int)Phase.MoltenBlobs : (int)Phase.SpearCocoon;
                                break;
                            case 9:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 10:
                                phase = (useCrystal || nightTime) ? (int)Phase.Crystal : (int)Phase.FlameCocoon;
                                break;
                            case 11:
                                phase = nightTime ? (int)Phase.FlameCocoon : (int)Phase.MoltenBlobs;
                                break;
                            case 12:
                                phase = (useLaser || nightTime) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 13:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 14:
                                phase = (useLaser || nightTime) ? (int)Phase.HolyBomb : (int)Phase.HolyBlast;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (phaseChange)
                        {
                            case 0:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 1:
                                phase = useLaser ? (int)Phase.Laser : (int)Phase.HolyFire;
                                break;
                            case 2:
                                phase = (int)Phase.HolyBomb;
                                break;
                            case 3:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 4:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 5:
                                phase = useCrystal ? (int)Phase.Crystal : (int)Phase.HolyBomb;
                                break;
                            case 6:
                                phase = (int)Phase.HolyFire;
                                break;
                            case 7:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 8:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 9:
                                phase = (int)Phase.FlameCocoon;
                                break;
                            case 10:
                                phase = (int)Phase.HolyBomb;
                                break;
                            case 11:
                                phase = useLaser ? (int)Phase.Laser : (int)Phase.HolyBlast;
                                break;
                            case 12:
                                phase = (int)Phase.HolyFire;
                                break;
                            case 13:
                                phase = (int)Phase.MoltenBlobs;
                                break;
                            case 14:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            default:
                                break;
                        }
                    }

                    // If too far from target, set phase to 0
                    if (Math.Abs(vector.X - player.Center.X) > 5600f)
                        phase = (int)Phase.HolyBlast;

                    // Reset attack delay for laser
                    if (phase == (int)Phase.Laser)
                        npc.localAI[2] = 0f;

                    // Reset arrays
                    AIState = phase;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    npc.netUpdate = true;
                    break;

                case (int)Phase.HolyBlast:

                    if (spawnAnimation)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[3] == 0f)
                            Projectile.NewProjectile(vector + new Vector2(0f, -80f), Vector2.Zero, ModContent.ProjectileType<HolyAura>(), 0, 0f, Main.myPlayer, biomeType, 0f);

                        if (calamityGlobalNPC.newAI[3] == 10f && nightTime)
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"), (int)npc.position.X, (int)npc.position.Y);

                        if (calamityGlobalNPC.newAI[3] > 10f && calamityGlobalNPC.newAI[3] < 150f)
                        {
                            int dustAmt = (int)MathHelper.Lerp(4f, 8f, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
                            for (int m = 0; m < dustAmt; m++)
                            {
                                float fade = MathHelper.Lerp(1.3f, 0.7f, npc.Opacity) * Utils.InverseLerp(0f, 120f, calamityGlobalNPC.newAI[3], clamped: true);
                                Color newColor = Main.hslToRgb(calamityGlobalNPC.newAI[3] / 180f, 1f, 0.5f);

                                if (!nightTime)
                                {
                                    newColor.R = 255;
                                    if (biomeType == 2)
                                        newColor.B = 0;
                                }
                                else
                                {
                                    newColor.B = 255;
                                    if (biomeType == 2)
                                        newColor.G = 0;
                                    else
                                        newColor.R = 0;
                                }

                                int dust = Dust.NewDust(npc.position, npc.width, npc.height, 267, 0f, 0f, 0, newColor);
                                Main.dust[dust].position = npc.Center + Main.rand.NextVector2Circular(npc.width * 2f, npc.height * 2f) + new Vector2(0f, -150f);
                                Main.dust[dust].velocity *= Main.rand.NextFloat() * 0.8f;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat() * 0.7f * fade;
                                Main.dust[dust].velocity += Vector2.UnitY * 3f;
                                Main.dust[dust].scale = 1.2f;

                                if (dust != 6000)
                                {
                                    Dust dust2 = Dust.CloneDust(dust);
                                    dust2.scale /= 2f;
                                    dust2.fadeIn *= 0.85f;
                                    dust2.color = new Color(255, 255, 0, 255);
                                }
                            }
                        }

                        npc.Opacity = MathHelper.Clamp(calamityGlobalNPC.newAI[3] / spawnAnimationTime, 0f, 1f);

                        calamityGlobalNPC.newAI[3] += 1f;

                        if (nightTime && calamityGlobalNPC.newAI[3] >= spawnAnimationTime)
                            calamityGlobalNPC.newAI[3] += 1f;

                        return;
                    }

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        npc.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && npc.position.Y < player.position.Y)
                    {
                        npc.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int num856 = (biomeEnraged ? 18 : expertMode ? 24 : 26) - shootBoost;

                        num856 = (int)(num856 * attackRateMult);

                        if (npc.ai[3] >= num856)
                            npc.ai[3] = -num856;

                        if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            vector.X += npc.velocity.X * 7f;
                            float num857 = player.position.X + player.width * 0.5f - vector.X;
                            float num858 = player.Center.Y - vector.Y;
                            float num859 = (float)Math.Sqrt(num857 * num857 + num858 * num858);

                            float velocityBoost = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float num860 = (expertMode ? 10.25f : 9f) + velocityBoost;

                            if (revenge)
                                num860 *= 1.15f;

                            num859 = num860 / num859;
                            num857 *= num859;
                            num858 *= num859;

                            Projectile.NewProjectile(vector.X, vector.Y, num857, num858, ModContent.ProjectileType<HolyBlast>(), holyBlastDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (npc.ai[3] < 0f)
                        npc.ai[3] += 1f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyFire:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        npc.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(6f * (1f - lifeRatio)) : (int)Math.Round(5f * (1f - lifeRatio));
                        int num864 = (expertMode ? 36 : 39) - shootBoost;
                        if (BossRushEvent.BossRushActive || biomeEnraged)
                            num864 = 27;

                        num864 = (int)(num864 * attackRateMult);

                        if (npc.ai[3] >= num864)
                        {
                            npc.ai[3] = 0f;

                            Vector2 vector113 = new Vector2(vector.X, npc.position.Y + npc.height - 14f);

                            float num865 = npc.velocity.Y;
                            if (num865 < 0f)
                                num865 = 0f;

                            num865 += expertMode ? 4f : 3f;

                            if (nightTime)
                                num865 *= 2f;

                            Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyFire>(), holyFireDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.FlameCocoon:

                    if (!targetDead)
                    {
                        if (npc.velocity.Length() <= 2f)
                            npc.velocity = Vector2.Zero;
                        if (npc.velocity.Length() > 2f)
                        {
                            npc.velocity *= 0.9f;
                            return;
                        }
                    }

                    float divisor = (expertMode ? 2f : 3f) + (float)Math.Floor(3f * lifeRatio) + (attackRateMult > 1D ? (float)Math.Ceiling(attackRateMult * 1.6) : 0f);
                    int totalFlameProjectiles = biomeEnraged ? 45 : 36;
                    int chains = 4;
                    float interval = totalFlameProjectiles / chains * divisor;
                    double patternInterval = Math.Floor(npc.ai[3] / interval);
                    int healingStarChance = revenge ? 8 : expertMode ? 6 : 4;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (patternInterval % 2 == 0)
                        {
                            if (npc.ai[3] % divisor == 0f)
                            {
                                Main.PlaySound(SoundID.Item20, npc.Center);
                                bool normalSpread = calamityGlobalNPC.newAI[1] % 2f == 0f;
                                double radians = MathHelper.TwoPi / chains;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = normalSpread ? new Vector2(0f, -cocoonProjVelocity) : new Vector2(-velocityX, -cocoonProjVelocity);
                                for (int i = 0; i < chains; i++)
                                {
                                    Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(npc.ai[2]));

                                    int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
                                    int dmgAmt = holyStarDamage;
                                    if (Main.rand.NextBool(healingStarChance) && !death)
                                    {
                                        projectileType = ModContent.ProjectileType<HolyLight>();
                                        dmgAmt = npc.GetProjectileDamageNoScaling(projectileType);
                                        Projectile.NewProjectile(fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, dmgAmt);
                                    }
                                    else
                                        Projectile.NewProjectile(fireFrom, vector2, projectileType, dmgAmt, 0f, Main.myPlayer);
                                }

                                // Radial offset
                                npc.ai[2] += 10f;
                            }
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.ai[2] = 0f;

                            totalFlameProjectiles = biomeEnraged ? 20 : 16;
                            if (npc.ai[3] % (divisor * totalFlameProjectiles) == 0f)
                            {
                                calamityGlobalNPC.newAI[1] += 1f;
                                double radians = MathHelper.TwoPi / totalFlameProjectiles;
                                Main.PlaySound(SoundID.Item20, npc.position);
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = npc.ai[3] % (divisor * totalFlameProjectiles * 2f) == 0f ? new Vector2(-velocityX, -cocoonProjVelocity) : new Vector2(0f, -cocoonProjVelocity);
                                for (int i = 0; i < totalFlameProjectiles; i++)
                                {
                                    Vector2 vector2 = spinningPoint.RotatedBy(radians * i);

                                    int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
                                    int dmgAmt = holyStarDamage;
                                    if (Main.rand.NextBool(healingStarChance) && !death)
                                    {
                                        projectileType = ModContent.ProjectileType<HolyLight>();
                                        dmgAmt = npc.GetProjectileDamageNoScaling(projectileType);
                                        Projectile.NewProjectile(fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, dmgAmt);
                                    }
                                    else
                                        Projectile.NewProjectile(fireFrom, vector2, projectileType, dmgAmt, 0f, Main.myPlayer);
                                }
                            }
                        }

                        // Fire a flame towards every player, with a limit of 5
                        if (npc.ai[3] % 60f == 0f && expertMode)
                        {
                            List<int> targets = new List<int>();
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                if (Main.player[p].active && !Main.player[p].dead)
                                    targets.Add(p);

                                if (targets.Count > 4)
                                    break;
                            }
                            foreach (int t in targets)
                            {
                                Vector2 velocity2 = Vector2.Normalize(Main.player[t].Center - fireFrom) * cocoonProjVelocity * 1.5f;
                                int type = ModContent.ProjectileType<HolyBurnOrb>();
                                Projectile.NewProjectile(fireFrom, velocity2, type, holyStarDamage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (npc.ai[3] == 0f)
                        DespawnSpecificProjectiles();

                    // Air is burning text
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= (phaseTime * 1.5f) && !text)
                    {
                        text = true;
                        string key = "Mods.CalamityMod.ProfanedBossText";
                        Color messageColor = Color.Orange;

                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }

                    // Inflict Icarus Folly
                    if (npc.ai[3] >= (phaseTime * 2f))
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Player player2 = Main.player[Main.myPlayer];
                            bool inLiquid = (player2.wet || player2.honeyWet) && !player2.lavaWet;

                            if (!player2.dead && player2.active && Vector2.Distance(player2.Center, vector) < 2800f && !inLiquid)
                            {
                                Main.PlaySound(SoundID.Item20, player2.position);
                                player2.AddBuff(ModContent.BuffType<ExtremeGravity>(), 3000, true);

                                for (int num621 = 0; num621 < 40; num621++)
                                {
                                    int num622 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[num622].velocity *= 3f;
                                    if (Main.rand.NextBool(2))
                                    {
                                        Main.dust[num622].scale = 0.5f;
                                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                    }
                                }

                                for (int num623 = 0; num623 < 60; num623++)
                                {
                                    int num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 3f);
                                    Main.dust[num624].noGravity = true;
                                    Main.dust[num624].velocity *= 5f;
                                    num624 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[num624].velocity *= 2f;
                                }
                            }
                        }

                        text = false;
                        AIState = (int)Phase.PhaseChange;
                        npc.localAI[2] = attackDelayAfterCocoon;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.MoltenBlobs:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        npc.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && npc.position.Y < player.position.Y)
                    {
                        npc.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int num856 = (expertMode ? 24 : 26) - shootBoost;
                        if (BossRushEvent.BossRushActive || biomeEnraged)
                            num856 = 18;

                        num856 = (int)(num856 * attackRateMult);

                        if (npc.ai[3] >= num856)
                            npc.ai[3] = -num856;

                        if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            vector.X += npc.velocity.X * 7f;
                            float num857 = player.position.X + player.width * 0.5f - vector.X;
                            float num858 = player.Center.Y - vector.Y;
                            float num859 = (float)Math.Sqrt(num857 * num857 + num858 * num858);

                            float shootBoost2 = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float num860 = (expertMode ? 10.25f : 9f) + shootBoost2;
                            if (BossRushEvent.BossRushActive)
                                num860 = 12.75f;

                            if (revenge)
                                num860 *= 1.15f;

                            num859 = num860 / num859;
                            num857 *= num859;
                            num858 *= num859;

                            Projectile.NewProjectile(vector.X, vector.Y, num857 * 0.1f, num858, ModContent.ProjectileType<MoltenBlast>(), moltenBlastDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    else if (npc.ai[3] < 0f)
                        npc.ai[3] += 1f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyBomb:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        npc.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(12f * (1f - lifeRatio)) : (int)Math.Round(10f * (1f - lifeRatio));
                        int num864 = (biomeEnraged ? 54 : expertMode ? 73 : 77) - shootBoost;

                        num864 = (int)(num864 * attackRateMult);

                        if (npc.ai[3] >= num864)
                        {
                            npc.ai[3] = 0f;

                            Vector2 vector113 = new Vector2(vector.X, npc.position.Y + npc.height - 14f);

                            float num865 = npc.velocity.Y;
                            if (num865 < 0f)
                                num865 = 0f;

                            num865 += expertMode ? 4f : 3f;

                            Projectile.NewProjectile(vector113.X, vector113.Y, npc.velocity.X * 0.25f, num865, ModContent.ProjectileType<HolyBomb>(), holyBombDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.SpearCocoon:

                    if (!targetDead)
                    {
                        if (npc.velocity.Length() <= 2f)
                            npc.velocity = Vector2.Zero;
                        if (npc.velocity.Length() > 2f)
                        {
                            npc.velocity *= 0.9f;
                            return;
                        }
                    }

                    if (npc.ai[1] == 0f)
                        DespawnSpecificProjectiles();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[2] += spearRate;
                        if (npc.ai[2] >= (float)(baseSpearRate * attackRateMult))
                        {
                            npc.ai[2] = 0f;

                            Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, fireFrom);

                            int projectileType = ModContent.ProjectileType<HolySpear>();

                            if (calamityGlobalNPC.newAI[2] % 2f == 0f)
                            {
                                int totalSpearProjectiles = biomeEnraged ? 15 : 12;
                                double radians = MathHelper.TwoPi / totalSpearProjectiles;
                                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-calamityGlobalNPC.newAI[1], -cocoonProjVelocity));

                                for (int i = 0; i < totalSpearProjectiles; i++)
                                {
                                    Vector2 vector2 = spinningPoint.RotatedBy(radians * i) * cocoonProjVelocity;
                                    Projectile.NewProjectile(fireFrom, vector2, projectileType, holySpearDamage, 0f, Main.myPlayer);
                                }

                                if (spearRateIncrease > 1f)
                                    spearRateIncrease = 1f;

                                float radialOffset = MathHelper.Lerp(0.2f, 0.4f, spearRateIncrease);
                                calamityGlobalNPC.newAI[1] += radialOffset;
                            }

                            calamityGlobalNPC.newAI[2] += 1f;

                            cocoonProjVelocity = death ? 14f : revenge ? 13f : expertMode ? 12f : 10f;
                            Vector2 velocity2 = Vector2.Normalize(player.Center - fireFrom) * cocoonProjVelocity;
                            Projectile.NewProjectile(fireFrom, velocity2, projectileType, holySpearDamage, 0f, Main.myPlayer, 1f, 0f);
                        }
                    }

                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.localAI[2] = attackDelayAfterCocoon;
                        npc.TargetClosest();
                    }

                    break;

                case (int)Phase.Crystal:

                    if (!targetDead)
                        npc.velocity *= 0.9f;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= crystalPhaseTime)
                    {
                        if (npc.ai[1] == crystalPhaseTime && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int proj = Projectile.NewProjectile(player.Center.X, player.Center.Y - 360f, 0f, 0f, ModContent.ProjectileType<ProvidenceCrystal>(), crystalDamage, 0f, player.whoAmI, lifeRatio, 0f);

                            if (nightTime)
                                Main.projectile[proj].timeLeft = nightCrystalTime;
                        }

                        if (npc.ai[1] >= crystalPhaseTime + nightCrystalTime || !nightTime)
                        {
                            AIState = (int)Phase.PhaseChange;
                            npc.TargetClosest();
                        }
                    }

                    break;

                case (int)Phase.Laser:

                    Vector2 value19 = new Vector2(27f, 59f);

                    float rotation = (nightTime ? 435f : 450f) + (guardianAmt * 5);

                    npc.ai[2] += 1f;
                    if (npc.ai[2] < 120f)
                    {
                        if (npc.ai[2] >= 40f)
                        {
                            int num1220 = 0;
                            if (npc.ai[2] >= 80f)
                                num1220 = 1;

                            for (int d = 0; d < 1 + num1220; d++)
                            {
                                float scalar = 1.2f;
                                if (d % 2 == 1)
                                    scalar = 2.8f;

                                Vector2 vector199 = new Vector2(vector.X, vector.Y + 32f) + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * value19 / 2f;
                                int index = Dust.NewDust(vector199 - Vector2.One * 8f, 16, 16, dustType, npc.velocity.X / 2f, npc.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[index].velocity = Vector2.Normalize(vector - vector199) * 3.5f * (10f - num1220 * 2f) / 10f;
                                Main.dust[index].noGravity = true;
                                Main.dust[index].scale = scalar;
                            }
                        }
                    }
                    else if (npc.ai[2] < (revenge ? 220f : 300f))
                    {
                        if (npc.ai[2] == 120f)
                        {
                            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 2800f)
                            {
                                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyRay"),
                                    (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = player.Center - vector;
                                velocity.Normalize();

                                float num1225 = -1f;
                                if (velocity.X < 0f)
                                    num1225 = 1f;

                                // 60 degrees offset
                                velocity = velocity.RotatedBy(-(double)num1225 * MathHelper.TwoPi / 6f);
                                Projectile.NewProjectile(vector.X, vector.Y + 32f, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

                                // -60 degrees offset
                                if (revenge)
                                    Projectile.NewProjectile(vector.X, vector.Y + 32f, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

                                if (nightTime && lifeRatio < 0.5f)
                                {
                                    rotation *= 0.33f;
                                    velocity = velocity.RotatedBy(-(double)num1225 * MathHelper.TwoPi / 2f);
                                    Projectile.NewProjectile(vector.X, vector.Y + 32f, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);

                                    if (revenge)
                                        Projectile.NewProjectile(vector.X, vector.Y + 32f, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -num1225 * MathHelper.TwoPi / rotation, npc.whoAmI);
                                }

                                npc.netUpdate = true;
                            }
                        }
                    }

                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= (revenge ? 235f : 315f))
                    {
                        AIState = (int)Phase.PhaseChange;
                        npc.TargetClosest();
                    }

                    break;
            }
        }

        public void DoDeathAnimation()
        {
            AIState = (int)Phase.HolyFire;
            useDefenseFrames = false;
            DeathAnimationTimer++;

            // Slow down to a halt and define rotation based off of that.
            npc.velocity *= 0.9f;
            npc.rotation = npc.velocity.X * 0.004f;

            // Play an animation sound immediately. Also delete various projectiles.
            if (DeathAnimationTimer == 1f)
            {
                if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.WithinRange(npc.Center, 4800f))
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceDeathAnimation").WithVolume(1.65f), Main.LocalPlayer.Center);

                DespawnSpecificProjectiles();

                int laserType = ModContent.ProjectileType<ProvidenceHolyRay>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (!Main.projectile[i].active || Main.projectile[i].type != laserType)
                        continue;
                    Main.projectile[i].Kill();
                }
            }

            // Begin fading out before the exploding sun animation happens.
            if (DeathAnimationTimer >= 370f)
                npc.Opacity *= 0.97f;

            // Create an explosive wave shortly after the death animation begins.
            // The temporal offset coincides with the point at which the crystal shatter sound happens in the
            // above defeat scene sound.
            if (DeathAnimationTimer == 92f)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ProvidenceHolyBlastImpact"), npc.Center);
                Main.PlaySound(npc.DeathSound, npc.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionBoom>(), 0, 0f);
            }

            // Explode as an enormous holy star before dying and dropping loot.
            if (Main.netMode != NetmodeID.MultiplayerClient && DeathAnimationTimer == 310f)
            {
                for (int i = 0; i < 80; i++)
                {
                    Vector2 sparkleVelocity = Main.rand.NextVector2Circular(23f, 23f);
                    Projectile.NewProjectile(npc.Center, sparkleVelocity, ModContent.ProjectileType<MajesticSparkle>(), 0, 0f);
                }
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<DyingSun>(), 0, 0f, 255);
            }

            // Idly release harmless cindiers.
            int shootRate = (int)MathHelper.Lerp(12f, 5f, Utils.InverseLerp(0f, 250f, DeathAnimationTimer, true));
            if (DeathAnimationTimer % shootRate == shootRate - 1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 shootVelocity = Main.rand.NextVector2CircularEdge(13f, 13f) * Main.rand.NextFloat(0.7f, 1.3f);
                    Projectile.NewProjectile(npc.Center, shootVelocity, ModContent.ProjectileType<SwirlingFire>(), 0, 0f, 255);
                }
            }

            // Do periodic syncs.
            if (Main.netMode == NetmodeID.Server && DeathAnimationTimer % 45f == 44f)
                npc.netUpdate = true;

            // Die and create drops after the star is gone.
            if (DeathAnimationTimer >= 345f)
            {
                npc.active = false;
                npc.HitEffect();
                npc.NPCLoot();
                npc.netUpdate = true;
            }
        }

        public float CalculateBurnIntensity()
        {
            float distanceToTarget = Vector2.Distance(Main.player[npc.target].Center, npc.Center);
            float aiTimer = npc.ai[3];

            // Night bool
            bool malice = CalamityWorld.malice;
            bool nightTime = !Main.dayTime || malice;

            float baseDistance = 2800f;
            float shorterFlameCocoonDistance = (CalamityWorld.death || BossRushEvent.BossRushActive || nightTime) ? 600f : CalamityWorld.revenge ? 400f : Main.expertMode ? 200f : 0f;
            float shorterSpearCocoonDistance = (CalamityWorld.death || BossRushEvent.BossRushActive || nightTime) ? 1000f : CalamityWorld.revenge ? 650f : Main.expertMode ? 300f : 0f;
            float shorterDistance = AIState == (int)Phase.FlameCocoon ? shorterFlameCocoonDistance : shorterSpearCocoonDistance;

            bool guardianAlive = false;
            if (CalamityGlobalNPC.holyBossAttacker != -1 && Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                guardianAlive = true;

            if (CalamityGlobalNPC.holyBossDefender != -1 && Main.npc[CalamityGlobalNPC.holyBossDefender].active)
                guardianAlive = true;

            if (CalamityGlobalNPC.holyBossHealer != -1 && Main.npc[CalamityGlobalNPC.holyBossHealer].active)
                guardianAlive = true;

            float maxDistance = baseDistance;

            // A factor which measures how much of the distance shortening shave-off should be taken into account.
            // It is determined based on how much time has elapsed during the attack thus far, specifically for the two cocoon attacks.
            // This shave-off does not happen when guardians are present.
            float shorterDistanceFade = Utils.InverseLerp(0f, 120f, aiTimer, true);
            if (!guardianAlive)
            {
                maxDistance = baseDistance;
                if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
                    maxDistance -= shorterDistance * shorterDistanceFade;
            }

            float drawFireDistanceStart = maxDistance - 800f;
            return Utils.InverseLerp(drawFireDistanceStart, maxDistance, distanceToTarget, true);
        }

        private void DespawnSpecificProjectiles(bool dying = false)
        {
            for (int x = 0; x < Main.maxProjectiles; x++)
            {
                Projectile projectile = Main.projectile[x];
                if (projectile.active)
                {
                    if (projectile.type == ModContent.ProjectileType<HolyFire2>() || projectile.type == ModContent.ProjectileType<HolyFlare>())
                        projectile.Kill();
                    else if (projectile.type == ModContent.ProjectileType<HolyBlast>() || projectile.type == ModContent.ProjectileType<HolyFire>())
                        projectile.active = false;

                    if (dying)
                    {
                        if (projectile.type == ModContent.ProjectileType<ProvidenceHolyRay>() || projectile.type == ModContent.ProjectileType<ProvidenceCrystal>() ||
                            projectile.type == ModContent.ProjectileType<ProvidenceCrystalShard>() || projectile.type == ModContent.ProjectileType<HolySpear>() ||
                            projectile.type == ModContent.ProjectileType<HolyBomb>() || projectile.type == ModContent.ProjectileType<MoltenBlob>() ||
                            projectile.type == ModContent.ProjectileType<HolyBurnOrb>() || projectile.type == ModContent.ProjectileType<HolyLight>())
                            projectile.Kill();
                        else if (projectile.type == ModContent.ProjectileType<MoltenBlast>())
                            projectile.active = false;
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void NPCLoot()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(npc);

            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<ProvidenceTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeProvidence>(), true, !CalamityWorld.downedProvidence);

            DropHelper.DropItemCondition(npc, ModContent.ItemType<RuneofCos>(), true, !CalamityWorld.downedProvidence);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, CalamityWorld.downedProvidence);

            // Accessories clientside only in Expert. Both drop if she is defeated at night.
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianWings>(), Main.expertMode, biomeType != 2 || !hasTakenDaytimeDamage);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ElysianAegis>(), Main.expertMode, biomeType == 2 || !hasTakenDaytimeDamage);

            // Drops pre-scal, cannot be sold, does nothing aka purely vanity. Requires at least expert for consistency with other post scal dev items.
            bool shouldDrop = challenge/* || (Main.expertMode && Main.rand.NextBool(CalamityWorld.downedSCal ? 10 : 200))*/;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ProfanedSoulCrystal>(), true, shouldDrop);

            // Special drop for defeating her at night
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ProfanedMoonlightDye>(), true, CalamityWorld.malice || !hasTakenDaytimeDamage, 3, 4);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<UnholyEssence>(), 20, 30);
                DropHelper.DropItemSpray(npc, ModContent.ItemType<DivineGeode>(), 15, 20);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<HolyCollider>(w),
                    DropHelper.WeightStack<SolarFlare>(w),
                    DropHelper.WeightStack<TelluricGlare>(w),
                    DropHelper.WeightStack<BlissfulBombardier>(w),
                    DropHelper.WeightStack<PurgeGuzzler>(w),
                    DropHelper.WeightStack<DazzlingStabberStaff>(w),
                    DropHelper.WeightStack<MoltenAmputator>(w)
                );

                // Equipment
                DropHelper.DropItem(npc, ModContent.ItemType<BlazingCore>(), true);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<ProvidenceMask>(), 7);
            }

            DropHelper.DropItemCondition(npc, ModContent.ItemType<PristineFury>(), !Main.expertMode, 0.1f);

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.Top.Y >= (Main.maxTilesY - 240f) * 16f)
                SpawnLootBox();

            // If Providence has not been killed, notify players of Uelibloom Ore
            if (!CalamityWorld.downedProvidence)
            {
                string key2 = "Mods.CalamityMod.ProfanedBossText3";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.TreeOreText";
                Color messageColor3 = Color.LightGreen;

                CalamityUtils.SpawnOre(ModContent.TileType<UelibloomOre>(), 15E-05, 0.4f, 0.8f, 3, 8, TileID.Mud);

                CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                CalamityUtils.DisplayLocalizedText(key3, messageColor3);
            }

            if (challenge)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue("Mods.CalamityMod.ProfanedBossText4"), Color.DarkOrange);
                }
            }

            // Mark Providence as dead
            CalamityWorld.downedProvidence = true;
            CalamityNetcode.SyncWorld();
        }

        private void SpawnLootBox()
        {
            int tileCenterX = (int)npc.Center.X / 16;
            int tileCenterY = (int)npc.Center.Y / 16;
            int halfBox = 5;
            for (int x = tileCenterX - halfBox; x <= tileCenterX + halfBox; x++)
            {
                for (int y = tileCenterY - halfBox; y <= tileCenterY + halfBox; y++)
                {
                    if ((x == tileCenterX - halfBox || x == tileCenterX + halfBox || y == tileCenterY - halfBox || y == tileCenterY + halfBox)
                        && !Main.tile[x, y].active())
                    {
                        Main.tile[x, y].type = (ushort)ModContent.TileType<ProfanedRock>();
                        Main.tile[x, y].active(true);
                    }
                    Main.tile[x, y].lava(false);
                    Main.tile[x, y].liquid = 0;

                    if (Main.netMode == NetmodeID.Server)
                        NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                    else
                        WorldGen.SquareTileFrame(x, y, true);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            void drawProvidenceInstance(Vector2 drawOffset, Color? colorOverride)
            {
                bool malice = CalamityWorld.malice;
                bool nightTime = !Main.dayTime || malice;

                string baseTextureString = "CalamityMod/NPCs/Providence/";
                string baseGlowTextureString = baseTextureString + "Glowmasks/";

                string getTextureString = baseTextureString + "Providence";
                string getTextureGlowString = baseGlowTextureString + "ProvidenceGlow";
                string getTextureGlow2String = baseGlowTextureString + "ProvidenceGlow2";

                if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
                {
                    if (!useDefenseFrames)
                    {
                        getTextureString = baseTextureString + "ProvidenceDefense";
                        getTextureGlowString = baseGlowTextureString + "ProvidenceDefenseGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceDefenseGlow2";
                    }
                    else
                    {
                        getTextureString = baseTextureString + "ProvidenceDefenseAlt";
                        getTextureGlowString = baseGlowTextureString + "ProvidenceDefenseAltGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceDefenseAltGlow2";
                    }
                }
                else
                {
                    if (frameUsed == 0)
                    {
                        getTextureGlowString = baseGlowTextureString + "ProvidenceGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceGlow2";
                    }
                    else if (frameUsed == 1)
                    {
                        getTextureString = baseTextureString + "ProvidenceAlt";
                        getTextureGlowString = baseGlowTextureString + "ProvidenceAltGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceAltGlow2";
                    }
                    else if (frameUsed == 2)
                    {
                        getTextureString = baseTextureString + "ProvidenceAttack";
                        getTextureGlowString = baseGlowTextureString + "ProvidenceAttackGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackGlow2";
                    }
                    else
                    {
                        getTextureString = baseTextureString + "ProvidenceAttackAlt";
                        getTextureGlowString = baseGlowTextureString + "ProvidenceAttackAltGlow";
                        getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackAltGlow2";
                    }
                }

                if (nightTime)
                {
                    getTextureString += "Night";
                    getTextureGlowString += "Night";
                    getTextureGlow2String += "Night";
                }

                Texture2D texture = ModContent.GetTexture(getTextureString);
                Texture2D textureGlow = ModContent.GetTexture(getTextureGlowString);
                Texture2D textureGlow2 = ModContent.GetTexture(getTextureGlow2String);

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

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
                        if (colorOverride != null)
                            color38 = colorOverride.Value;

                        Vector2 vector41 = npc.oldPos[num155] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                        vector41 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                        vector41 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
                        spriteBatch.Draw(texture, vector41, npc.frame, color38, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                    }
                }

                Vector2 vector43 = npc.Center - Main.screenPosition;
                vector43 -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
                spriteBatch.Draw(texture, vector43, npc.frame, colorOverride ?? npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                Color color37 = Color.Lerp(Color.White, nightTime ? Color.Cyan : Color.Yellow, 0.5f) * npc.Opacity;
                Color color42 = Color.Lerp(Color.White, nightTime ? Color.BlueViolet : Color.Violet, 0.5f) * npc.Opacity;
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
                        color41 = npc.GetAlpha(color41);
                        color41 *= (num153 - num163) / 15f;
                        if (colorOverride != null)
                            color41 = colorOverride.Value;

                        Vector2 vector44 = npc.oldPos[num163] + new Vector2(npc.width, npc.height) / 2f - Main.screenPosition;
                        vector44 -= new Vector2(textureGlow.Width, textureGlow.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
                        vector44 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY) + drawOffset;
                        spriteBatch.Draw(textureGlow, vector44, npc.frame, color41, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                        Color color43 = color42;
                        color43 = Color.Lerp(color43, color36, amount9);
                        color43 = npc.GetAlpha(color43);
                        color43 *= (num153 - num163) / 15f;
                        if (colorOverride != null)
                            color43 = colorOverride.Value;
                        spriteBatch.Draw(textureGlow2, vector44, npc.frame, color43, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(textureGlow, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

                spriteBatch.Draw(textureGlow2, vector43, npc.frame, color42, npc.rotation, vector11, npc.scale, spriteEffects, 0f);
            }

            float burnIntensity = Utils.InverseLerp(0f, 45f, DeathAnimationTimer, true);
            int totalProvidencesToDraw = (int)MathHelper.Lerp(1f, 30f, burnIntensity);
            for (int i = 0; i < totalProvidencesToDraw; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i * 2f / totalProvidencesToDraw;
                float drawOffsetFactor = (float)Math.Sin(offsetAngle * 6f + Main.GlobalTime * MathHelper.Pi);
                drawOffsetFactor *= (float)Math.Pow(burnIntensity, 3f) * 50f;

                Vector2 drawOffset = offsetAngle.ToRotationVector2() * drawOffsetFactor;
                Color baseColor = Color.White * (MathHelper.Lerp(0.4f, 0.8f, burnIntensity) / totalProvidencesToDraw * 1.5f);
                baseColor.A = 0;

                baseColor = Color.Lerp(Color.White, baseColor, burnIntensity);
                drawProvidenceInstance(drawOffset, totalProvidencesToDraw == 1 ? null : (Color?)baseColor);
            }
            return false;
        }

        public override void FindFrame(int frameHeight) //9 total frames
        {
            if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
            {
                if (!useDefenseFrames)
                {
                    npc.frameCounter += Dying ? 0.25 : 1.0;
                    if (npc.frameCounter > 8.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 3)
                    {
                        npc.frame.Y = 0;
                        useDefenseFrames = true;
                    }
                }
                else
                {
                    npc.frameCounter += Dying ? 0.25 : 1.0;
                    if (npc.frameCounter > 8.0)
                    {
                        npc.frame.Y = npc.frame.Y + frameHeight;
                        npc.frameCounter = 0.0;
                    }
                    if (npc.frame.Y >= frameHeight * 2)
                        npc.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (useDefenseFrames)
                    useDefenseFrames = false;

                npc.frameCounter += Dying ? 0.25 : 1.0;
                if (npc.frameCounter > (npc.Calamity().newAI[3] < 180f ? 8.0 : 5.0))
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 3) //6
                {
                    npc.frame.Y = 0;
                    frameUsed++;
                }
                if (frameUsed > 3)
                    frameUsed = 0;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (!hasTakenDaytimeDamage)
            {
                if (Main.dayTime)
                {
                    hasTakenDaytimeDamage = true;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
                        netMessage.Write((byte)npc.whoAmI);
                        netMessage.Write(hasTakenDaytimeDamage);
                        netMessage.Send();
                    }
                }
            }

            if (challenge)
            {
                List<int> exceptionList = new List<int>()
                {
                    ModContent.ProjectileType<GoldenGunProj>(),
                    ModContent.ProjectileType<MiniGuardianDefense>(),
                    ModContent.ProjectileType<MiniGuardianAttack>(),
                    ModContent.ProjectileType<SilvaCrystalExplosion>(),
                    ModContent.ProjectileType<GhostlyMine>(),
                    ModContent.ProjectileType<EnergyOrb>(),
                    ModContent.ProjectileType<IrradiatedAura>(),
                    ModContent.ProjectileType<SummonAstralExplosion>(),
                    ModContent.ProjectileType<ApparatusExplosion>()
                };

                bool allowedClass = projectile.IsSummon() || (!projectile.melee && !projectile.ranged && !projectile.magic && !projectile.thrown && !projectile.Calamity().rogue);
                bool allowedDamage = allowedClass && damage <= 75; //Flat 75 regardless of difficulty.
                //Absorber on-hit effects likely won't proc this but Deific Amulet and Astral Bulwark stars will proc this.
                bool allowedBabs = Main.player[projectile.owner].Calamity().pArtifact && !Main.player[projectile.owner].Calamity().profanedCrystalBuffs;

                if ((exceptionList.TrueForAll(x => projectile.type != x) && !allowedDamage) || !allowedBabs)
                {
                    challenge = false;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
                        netMessage.Write((byte)npc.whoAmI);
                        netMessage.Write(challenge);
                        netMessage.Send();
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (!hasTakenDaytimeDamage)
            {
                if (Main.dayTime)
                {
                    hasTakenDaytimeDamage = true;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
                        netMessage.Write((byte)npc.whoAmI);
                        netMessage.Write(hasTakenDaytimeDamage);
                        netMessage.Send();
                    }
                }
            }

            if (challenge)
            {
                challenge = false;

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
                    netMessage.Write((byte)npc.whoAmI);
                    netMessage.Write(challenge);
                    netMessage.Send();
                }
            }
        }

        public override bool CheckDead()
        {
            if (!Dying)
            {
                DespawnSpecificProjectiles(true);
                Dying = true;
                npc.active = true;
                npc.netUpdate = true;
            }
            return false;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if ((damage * (crit ? 2D : 1D)) >= npc.life)
            {
                damage = 0D;

                npc.life = 1;
                npc.dontTakeDamage = true;
                if (!Dying)
                    CheckDead();
                return false;
            }

            return base.StrikeNPC(ref damage, defense, ref knockback, hitDirection, ref crit);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.soundDelay == 0 && !Dying)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/ProvidenceHurt"), npc.Center);
            }

            int dustType = Main.dayTime ? (int)CalamityDusts.ProfanedFire : (int)CalamityDusts.Nightwither;
            for (int k = 0; k < 15; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, dustType, hitDirection, -1f, 0, default, 1f);

            if (npc.life <= 0)
            {
                float randomSpread = Main.rand.Next(-50, 50) / 100;
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence2"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence3"), 1f);
                Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/Providence4"), 1f);
                npc.position = npc.Center;
                npc.width = 400;
                npc.height = 350;
                npc.position -= npc.Size * 0.5f;
                for (int d = 0; d < 60; d++)
                {
                    int fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 90; d++)
                {
                    int fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    fire = Dust.NewDust(npc.position, npc.width, npc.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                }
            }
        }
    }
}
