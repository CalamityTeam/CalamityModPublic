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
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Filters = Terraria.Graphics.Effects.Filters;

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
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public enum BossMode
        {
            Night = -1,
            Day = 0,
            Red = 1,
            Orange = 2,
            Yellow = 3,
            Green = 4,
            Blue = 5,
            Violet = 6
        }
        public int colorShiftTimer = -1;

        private bool text = false;
        private bool useDefenseFrames = false;
        private float bossLife;
        private int biomeType = 0;
        private int flightPath = 0;
        private int phaseChange = 0;
        private int frameUsed = 0;
        private int healTimer = 0;
        internal bool challenge = Main.expertMode; //Used to determine if Profaned Soul Crystal should drop, couldn't figure out mp mems always dropping it so challenge is singleplayer only.
        internal bool hasTakenDaytimeDamage = false;
        public static bool shouldDrawInfernoBorder = true; //This is only here for other mods to disable it if they don't want it drawing.
        public bool Dying = false;
        public int DeathAnimationTimer;
        public static float borderRadius = 3000f;

        //Sounds
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceSpawn") { Volume = 1.2f };
        public static readonly SoundStyle HolyRaySound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyRay") { Volume = 1.25f }; //note : Volume gets clamped between 0 and 1. I don't think this does anything, but it was in the original ModSound so im keeping it just in case
        public static readonly SoundStyle HurtSound = new("CalamityMod/Sounds/NPCHit/ProvidenceHurt");
        public static readonly SoundStyle DeathAnimationSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceDeathAnimation");

        public static readonly SoundStyle NearBurnSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceSizzle");
        public static readonly SoundStyle BurnStartSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceBurn");
        public static readonly SoundStyle BurnLoopSound = new SoundStyle("CalamityMod/Sounds/Custom/Providence/ProvidenceBurnLoop") with { IsLooped = true };
        //Sound slot for the burning damage over time effect
        public SlotId BurningSoundSlot;
        //Level of sound playing
        public float SoundWarningLevel = -1f;
        
        public static float normalDR = 0.3f;
        public static float cocoonDR = 0.9f;

        private const float TimeForStarDespawn = 120f;
        private const float TimeForShieldDespawn = 120f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.2f,
                PortraitScale = 0.32f,
                PortraitPositionYOverride = 16f
            };
            value.Position.Y += 6f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.npcSlots = 36f;
            NPC.damage = 100;
            NPC.width = 600;
            NPC.height = 450;
            NPC.defense = 50;
            NPC.DR_NERD(normalDR);
            NPC.LifeMaxNERB(312500, 375000, 1250000); // Old HP - 440000, 500000
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(3, 0, 0, 0);
            NPC.boss = true;
            NPC.Opacity = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.DeathSound = null;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToWater = true;

            if (Main.getGoodWorld)
                NPC.scale *= 0.25f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheHallow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Providence")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(text);
            writer.Write(useDefenseFrames);
            writer.Write(biomeType);
            writer.Write(colorShiftTimer);
            writer.Write(phaseChange);
            writer.Write(frameUsed);
            writer.Write(healTimer);
            writer.Write(flightPath);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.chaseable);
            writer.Write(NPC.canGhostHeal);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(SoundWarningLevel);
            writer.Write(Dying);
            writer.Write(DeathAnimationTimer);
            writer.Write(borderRadius);
            writer.Write(shouldDrawInfernoBorder);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool wasDyingBefore = Dying;
            text = reader.ReadBoolean();
            useDefenseFrames = reader.ReadBoolean();
            biomeType = reader.ReadInt32();
            colorShiftTimer = reader.ReadInt32();
            phaseChange = reader.ReadInt32();
            frameUsed = reader.ReadInt32();
            healTimer = reader.ReadInt32();
            flightPath = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();
            NPC.canGhostHeal = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            SoundWarningLevel = reader.ReadSingle();
            Dying = reader.ReadBoolean();
            DeathAnimationTimer = reader.ReadInt32();
            borderRadius = reader.ReadSingle();
            shouldDrawInfernoBorder = reader.ReadBoolean();
            
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();

            // Be sure to inform clients of the fact that Providence is dying if only the server recieved this packet.
            if (Main.netMode == NetmodeID.Server && !wasDyingBefore && Dying)
            {
                NPC.netSpam = 0;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            // Set the border drawing to true if it isn't set to true
            // Can happen when another mod sets to false for a difficulty and that difficulty is then toggled off.
            shouldDrawInfernoBorder = true;

            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // whoAmI variable for Guardians and other things
            CalamityGlobalNPC.holyBoss = NPC.whoAmI;

            // Rotation
            NPC.rotation = NPC.velocity.X * 0.004f;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            // Target variable and boss center
            Player player = Main.player[NPC.target];

            // Night bool and Color shifting
            bool bossRush = BossRushEvent.BossRushActive;

            bool getFuckedAI = Main.zenithWorld;
            int timeToShift = 30; //Switches color every half-second
            if (getFuckedAI)
            {
                colorShiftTimer++;

                if (colorShiftTimer == -1) //Initiate
                {
                    NPC.localAI[1] = (float)BossMode.Red;
                    colorShiftTimer = 0;
                }
                else if (colorShiftTimer >= timeToShift)
                {
                    NPC.localAI[1]++;
                    if (NPC.localAI[1] > (float)BossMode.Violet)
                        NPC.localAI[1] = (float)BossMode.Red;
                    colorShiftTimer = 0;
                }
            }
            else if (!Main.dayTime || bossRush) //Normal Night time activity
                NPC.localAI[1] = (float)BossMode.Night;
            else
                NPC.localAI[1] = (float)BossMode.Day;

            //Has Night AI if it's any color except day
            bool nightAI = NPC.localAI[1] != (float)BossMode.Day;

            // Difficulty bools
            bool death = CalamityWorld.death || nightAI;
            bool revenge = CalamityWorld.revenge || nightAI;
            bool expertMode = Main.expertMode || nightAI;

            // Target's current biome
            bool isHoly = player.ZoneHallow;
            bool isHell = player.ZoneUnderworldHeight;

            // Fire projectiles at normal rate or not
            bool normalAttackRate = true;

            // Is in spawning animation
            float spawnAnimationTime = 180f;
            bool spawnAnimation = calamityGlobalNPC.newAI[3] < spawnAnimationTime;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Play enrage animation if night starts
            if (!getFuckedAI && nightAI && calamityGlobalNPC.newAI[3] == spawnAnimationTime)
            {
                AIState = (int)Phase.HolyBlast;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                calamityGlobalNPC.newAI[1] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
                calamityGlobalNPC.newAI[3] = 0f;
                NPC.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (NPC.netSpam >= 10)
                    NPC.netSpam = 9;
            }

            // Increase all projectile damage at night, but reduce to 0 for Zenith
            int projectileDamageMult = 1;
            if (nightAI)
                projectileDamageMult = 2;

            NPC.Calamity().CurrentlyEnraged = !bossRush && nightAI;

            // Projectile damage values
            int holyLaserDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<ProvidenceHolyRay>()) * projectileDamageMult;
            int crystalDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<ProvidenceCrystal>()) * projectileDamageMult;
            int holySpearDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<HolySpear>()) * projectileDamageMult;
            int holyBombDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<HolyBomb>()) * projectileDamageMult;
            int moltenBlastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<MoltenBlast>()) * projectileDamageMult;
            int holyFireDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<HolyFire>()) * projectileDamageMult;
            int holyBlastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<HolyBlast>()) * projectileDamageMult;
            int holyStarDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<HolyBurnOrb>()) * projectileDamageMult;

            // Change dust type at night
            int dustType = ProvUtils.GetDustID(NPC.localAI[1]);

            // Phase times
            float phaseTime = nightAI ? (240f - 60f * (1f - lifeRatio)) : 300f;
            float crystalPhaseTime = nightAI ? (float)Math.Round(60f * lifeRatio) : death ? 60f : 120f;
            int nightCrystalTime = 210;
            int gfbCrystalTime = 1500 + nightCrystalTime;
            float attackDelayAfterCocoon = phaseTime * 0.3f;

            // Phases
            bool ignoreGuardianAmt = lifeRatio < (death ? 0.2f : 0.15f);
            bool phase2 = lifeRatio < 0.75f && !nightAI;
            bool delayAttacks = NPC.localAI[2] > 0f;

            // Spear phase
            float spearRateIncrease = 1f - lifeRatio;
            float bossRushSpearRateIncrease = 0.25f;
            float baseSpearRate = 18f;
            float spearRate = 1f + spearRateIncrease;

            if (bossRush)
                spearRate += bossRushSpearRateIncrease;

            // Projectile fire rate multiplier
            double attackRateMult = 1D;

            // Where projectiles are fired from during cocoon phases
            Vector2 fireFrom = new Vector2(NPC.Center.X, NPC.Center.Y + 20f * NPC.scale);

            // Cocoon projectile initial velocity
            float cocoonProjVelocity = 3f + (death ? 2f * (1f - lifeRatio) : 0f);

            // Distance X needed from target in order to fire holy or molten blasts
            float distanceNeededToShoot = (death ? 300f : revenge ? 360f : 420f) * NPC.scale;

            // X distance from target
            float distanceX = Math.Abs(NPC.Center.X - player.Center.X);

            // Inflict Holy Inferno if target is too far away
            float burnIntensity = CalculateBurnIntensity(attackDelayAfterCocoon);    

            if (!player.dead && player.active && !player.creativeGodMode && !Dying)
            {
                //The debuff applies
                if (burnIntensity >= 1f)
                {
                    if (SoundWarningLevel < 2f)
                    {
                        //Initialize sound
                        SoundEngine.PlaySound(BurnStartSound, player.Center);
                        BurningSoundSlot = SoundEngine.PlaySound(BurnLoopSound, player.Center);
                        SoundWarningLevel = 2f;
                    }
                    player.AddBuff(ModContent.BuffType<HolyInferno>(), 2);
                }
                //If the sound is still playing, make it go slowly kinda
                else if (SoundWarningLevel > 1f)
                {
                    SoundWarningLevel -= 1 / 100f;
                    if (SoundWarningLevel < 1f)
                        SoundWarningLevel = 1f;
                }
                //The player starts to get fire particles
                else if (burnIntensity > 0.45f)
                {
                    //If the player goes from 0 to 1, then play the sound. Doesn't play when descending.
                    if (SoundWarningLevel < 1f)
                        SoundEngine.PlaySound(NearBurnSound, player.Center);

                    SoundWarningLevel = 1f;
                }
                //The player has sparks if intensity is above 0, otherwise nothing happens
                else if (burnIntensity <= 0f)
                {
                    //Reset the sound
                    SoundWarningLevel = 0f;
                }
            }
            else if (SoundWarningLevel > 0f)
                SoundWarningLevel -= 1 / 50f;

            // Updating the looping sound
            if (SoundEngine.TryGetActiveSound(BurningSoundSlot, out var burningSound) && burningSound.IsPlaying)
                burningSound.Position = player.Center;

            // Adjust the volume or break the loop accordingly
            if (burningSound is not null)
            {
                if (SoundWarningLevel <= 1f)
                    burningSound?.Stop();
                else if(SoundWarningLevel <= 2f)
                    burningSound.Volume = SoundWarningLevel - 1f;
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

            // Makes it so star shit can only happen if the attacker has spawned
            if (attackerAlive && NPC.localAI[0] == 0f)
                NPC.localAI[0] = 1f;

            // Can only run after the attacker has spawned and died
            if (!attackerAlive && NPC.localAI[0] > 0f)
            {
                if (NPC.localAI[0] < TimeForStarDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[0] == 1f)
                        SoundEngine.PlaySound(SoundID.Item105, NPC.Center);

                    NPC.localAI[0] += 1f;
                }
            }

            // Makes it so shield shit can only happen if the defender has spawned
            if (defenderAlive && NPC.localAI[3] == 0f)
                NPC.localAI[3] = 1f;

            // Can only run after the defender has spawned and died
            if (!defenderAlive && NPC.localAI[3] > 0f)
            {
                if (NPC.localAI[3] < TimeForShieldDespawn)
                {
                    // Star Wrath use sound
                    if (NPC.localAI[3] == 1f)
                        SoundEngine.PlaySound(SoundID.Item105, NPC.Center);

                    NPC.localAI[3] += 1f;
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
            NPC.chaseable = normalAttackRate;
            NPC.canGhostHeal = NPC.chaseable;

            // Prevent lag by stopping rain
            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();

            // Set target biome type
            if (biomeType == 0)
            {
                if (isHell)
                    biomeType = 2;
                else if (isHoly)
                    biomeType = 1;
            }

            // Do the death animation once killed.
            if (Dying)
            {
                DoDeathAnimation();
                return;
            }
            // Trigger the death animation
            else if (NPC.life <= 1)
            {
                NPC.life = 1;
                DespawnSpecificProjectiles(true);
                Dying = true;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (NPC.netSpam >= 10)
                    NPC.netSpam = 9;

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
                NPC.defense = NPC.defDefense * 2;
            else
                NPC.defense = NPC.defDefense;

            // Healing
            if (healerAlive)
            {
                float distanceFromHealer = Vector2.Distance(Main.npc[CalamityGlobalNPC.holyBossHealer].Center, NPC.Center);
                bool dontHeal = Main.npc[CalamityGlobalNPC.holyBossHealer].justHit || NPC.life == NPC.lifeMax;
                if (dontHeal)
                {
                    healTimer = 0;
                }
                else
                {
                    float healGateValue = revenge ? 60f : 90f;
                    healTimer++;
                    if (healTimer >= healGateValue)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                        int maxHealDustIterations = (int)distanceFromHealer;
                        int maxDust = 100;
                        int dustDivisor = maxHealDustIterations / maxDust;
                        if (dustDivisor < 2)
                            dustDivisor = 2;

                        Vector2 dustLineStart = Main.npc[CalamityGlobalNPC.holyBossHealer].Center;
                        Vector2 dustLineEnd = NPC.Center;
                        Vector2 currentDustPos = default;
                        Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                        Vector2 dustVelocityMult = new Vector2(2.1f, 2f);
                        Color dustColor = Main.hslToRgb(Main.rgbToHsl(new Color(255, 200, Main.DiscoB)).X, 1f, 0.5f);
                        dustColor.A = 255;
                        for (int i = 0; i < maxHealDustIterations; i++)
                        {
                            if (i % dustDivisor == 0)
                            {
                                currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, i / (float)maxHealDustIterations);
                                int dust = Dust.NewDust(currentDustPos, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = currentDustPos;
                                Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * i / maxHealDustIterations) * dustVelocityMult * (0.8f + Main.rand.NextFloat() * 0.4f) + NPC.velocity;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 1f;
                                Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                Dust dust2 = Dust.CloneDust(dust);
                                Dust dust3 = dust2;
                                dust3.scale /= 2f;
                                dust3 = dust2;
                                dust3.fadeIn /= 2f;
                                dust2.color = new Color(255, 255, 255, 255);
                            }
                        }

                        healTimer = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int healAmt = NPC.lifeMax / 200;
                            if (healAmt > NPC.lifeMax - NPC.life)
                                healAmt = NPC.lifeMax - NPC.life;

                            if (healAmt > 0)
                            {
                                NPC.life += healAmt;
                                NPC.HealEffect(healAmt, true);

                                // Prevent netUpdate from being blocked by the spam counter.
                                if (NPC.netSpam >= 10)
                                    NPC.netSpam = 9;

                                NPC.netUpdate = true;
                            }
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
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                }
                if (!player.active || player.dead)
                {
                    targetDead = true;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X += 0.2f;
                    else
                        NPC.velocity.X -= 0.2f;

                    NPC.velocity.Y -= 0.2f;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Guardian spawn unless it's night time specifically (can still spawn on Zenith)
            if (NPC.localAI[1] != (float)BossMode.Night)
            {
                if (bossLife == 0f && NPC.life > 0)
                    bossLife = NPC.lifeMax;

                if (NPC.life > 0)
                {
                    int guardianHealthThreshold = (int)(NPC.lifeMax * 0.66);
                    if ((NPC.life + guardianHealthThreshold) < bossLife)
                    {
                        bossLife = NPC.life;
                        SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int guardianRingAmt = 3;
                            int guardianSpread = 360 / guardianRingAmt;
                            int guardianDistance = 400;
                            for (int i = 0; i < guardianRingAmt; i++)
                            {
                                int type = i == 0 ? ModContent.NPCType<ProvSpawnDefense>() : i == 1 ? ModContent.NPCType<ProvSpawnHealer>() : ModContent.NPCType<ProvSpawnOffense>();
                                int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * guardianSpread) * guardianDistance)), (int)(NPC.Center.Y + (Math.Cos(i * guardianSpread) * guardianDistance)), type, NPC.whoAmI, 0, 0, 0, -1);
                                Main.npc[spawn].ai[0] = i * guardianSpread;
                            }
                        }
                    }
                }
            }

            // Set DR based on current attack phase
            NPC.Calamity().DR = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation) ?
                cocoonDR : delayAttacks ?
                MathHelper.Lerp(normalDR, cocoonDR, NPC.localAI[2] / attackDelayAfterCocoon) : normalDR;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon || AIState == (int)Phase.Laser || spawnAnimation;

            // Movement
            if (getFuckedAI || (AIState != (int)Phase.FlameCocoon && AIState != (int)Phase.SpearCocoon))
            {
                // Slowly drift down when spawning
                if (spawnAnimation)
                {
                    colorShiftTimer++; // Also double the shift speed in the mean time :)
                    float minSpawnVelocity = 0.4f;
                    float maxSpawnVelocity = 4f;
                    float velocityY = maxSpawnVelocity - MathHelper.Lerp(minSpawnVelocity, maxSpawnVelocity, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
                    NPC.velocity = new Vector2(0f, velocityY);
                }
                else
                {
                    // Slows down while firing Holy Rays. It would've not slowed down for the Zenith seed but apparently it was too fast (shockers).
                    bool laserPhaseSlow = AIState == (int)Phase.Laser;

                    // Change X direction of movement
                    if (flightPath == 0)
                    {
                        if (NPC.Center.X < player.Center.X)
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
                    float changeDirectionThreshold = 800f;

                    // Increase distance from target when firing molten blasts or holy bombs
                    bool stayAwayFromTarget = AIState == (int)Phase.MoltenBlobs || AIState == (int)Phase.HolyBomb;
                    if (stayAwayFromTarget)
                        changeDirectionThreshold += death ? 240f : revenge ? 180f : 120f;

                    // Change X movement path if far enough away from target
                    if (NPC.Center.X < player.Center.X && flightPath < 0 && distanceX > changeDirectionThreshold)
                        flightPath = 0;
                    if (NPC.Center.X > player.Center.X && flightPath > 0 && distanceX > changeDirectionThreshold)
                        flightPath = 0;

                    // Velocity and acceleration
                    float speedIncreaseTimer = nightAI ? 75f : death ? 120f : 150f;
                    bool increaseSpeed = calamityGlobalNPC.newAI[0] > speedIncreaseTimer || bossRush;
                    float accelerationBoost = death ? 0.3f * (1f - lifeRatio) : 0.2f * (1f - lifeRatio);
                    float velocityBoost = death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                    float acceleration = (expertMode ? 1.1f : 1.05f) + accelerationBoost;
                    float velocity = (expertMode ? 16f : 15f) + velocityBoost;
                    if (nightAI)
                    {
                        acceleration = 1.5f;
                        velocity = 25f;
                    }
                    if (laserPhaseSlow)
                    {
                        acceleration *= getFuckedAI ? 0.6f : 0.4f;
                        velocity *= getFuckedAI ? 0.6f : 0.4f;
                    }
                    else if (increaseSpeed)
                    {
                        velocity += (calamityGlobalNPC.newAI[0] - speedIncreaseTimer) * 0.04f;
                        if (velocity > 30f || bossRush)
                            velocity = 30f;
                        if (bossRush)
                            acceleration = 2f;
                    }

                    if (Main.getGoodWorld)
                    {
                        velocity *= 1.2f;
                        acceleration *= 1.2f;
                    }

                    if (!targetDead)
                    {
                        NPC.velocity.X += flightPath * acceleration;
                        if (NPC.velocity.X > velocity)
                            NPC.velocity.X = velocity;
                        if (NPC.velocity.X < -velocity)
                            NPC.velocity.X = -velocity;

                        float moveUpThreshold = player.position.Y - (NPC.position.Y + NPC.height);
                        if (moveUpThreshold < (laserPhaseSlow ? 150f : 200f)) // 150
                            NPC.velocity.Y -= Main.getGoodWorld ? 0.4f : 0.2f;
                        if (moveUpThreshold > (laserPhaseSlow ? 200f : 250f)) // 200
                            NPC.velocity.Y += Main.getGoodWorld ? 0.4f : 0.2f;

                        float speedCap = laserPhaseSlow ? 2f : 6f;
                        if (Main.getGoodWorld)
                            speedCap *= 1.5f;

                        if (NPC.velocity.Y > speedCap)
                            NPC.velocity.Y = speedCap;
                        if (NPC.velocity.Y < -speedCap)
                            NPC.velocity.Y = -speedCap;
                    }
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
                    bool useLaser = (phase2 && biomeType == 1) || bossRush;
                    bool useCrystal = (phase2 && biomeType == 2) || bossRush;

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
                                phase = (useCrystal || nightAI) ? (int)Phase.Crystal : (int)Phase.MoltenBlobs;
                                break;
                            case 4:
                                phase = useCrystal ? (int)Phase.MoltenBlobs : (int)Phase.FlameCocoon;
                                break;
                            case 5:
                                phase = useCrystal ? (int)Phase.FlameCocoon : (int)Phase.HolyFire;
                                break;
                            case 6:
                                phase = (useLaser || nightAI) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 7:
                                phase = (useLaser || nightAI) ? (int)Phase.HolyBomb : (int)Phase.MoltenBlobs;
                                break;
                            case 8:
                                phase = (useLaser || nightAI) ? (int)Phase.MoltenBlobs : (int)Phase.SpearCocoon;
                                break;
                            case 9:
                                phase = (int)Phase.HolyBlast;
                                break;
                            case 10:
                                phase = (useCrystal || nightAI) ? (int)Phase.Crystal : (int)Phase.FlameCocoon;
                                break;
                            case 11:
                                phase = nightAI ? (int)Phase.FlameCocoon : (int)Phase.MoltenBlobs;
                                break;
                            case 12:
                                phase = (useLaser || nightAI) ? (int)Phase.Laser : (int)Phase.HolyBomb;
                                break;
                            case 13:
                                phase = (int)Phase.SpearCocoon;
                                break;
                            case 14:
                                phase = (useLaser || nightAI) ? (int)Phase.HolyBomb : (int)Phase.HolyBlast;
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
                    if (Math.Abs(NPC.Center.X - player.Center.X) > 5600f)
                        phase = (int)Phase.HolyBlast;

                    // Reset attack delay for laser
                    if (phase == (int)Phase.Laser)
                        NPC.localAI[2] = 0f;

                    // Reset arrays
                    AIState = phase;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;

                    // Prevent netUpdate from being blocked by the spam counter.
                    if (NPC.netSpam >= 10)
                        NPC.netSpam = 9;

                    NPC.netUpdate = true;

                    break;

                case (int)Phase.HolyBlast:

                    if (spawnAnimation)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient && calamityGlobalNPC.newAI[3] == 0f)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0f, -80f), Vector2.Zero, ModContent.ProjectileType<HolyAura>(), 0, 0f, Main.myPlayer, biomeType, 0f);

                        if (calamityGlobalNPC.newAI[3] == 10f)
                            SoundEngine.PlaySound(HolyRaySound, NPC.Center);

                        if (calamityGlobalNPC.newAI[3] > 10f && calamityGlobalNPC.newAI[3] < 150f)
                        {
                            int dustAmt = (int)MathHelper.Lerp(4f, 8f, calamityGlobalNPC.newAI[3] / spawnAnimationTime);
                            for (int m = 0; m < dustAmt; m++)
                            {
                                float fade = MathHelper.Lerp(1.3f, 0.7f, NPC.Opacity) * Utils.GetLerpValue(0f, 120f, calamityGlobalNPC.newAI[3], clamped: true);
                                Color newColor = Main.hslToRgb(calamityGlobalNPC.newAI[3] / 180f, 1f, 0.5f);

                                if (!nightAI)
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

                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, 267, 0f, 0f, 0, newColor);
                                Main.dust[dust].position = NPC.Center + Main.rand.NextVector2Circular(NPC.width * 2f, NPC.height * 2f) + new Vector2(0f, -150f);
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

                        NPC.Opacity = MathHelper.Clamp(calamityGlobalNPC.newAI[3] / spawnAnimationTime, 0f, 1f);

                        calamityGlobalNPC.newAI[3] += 1f;

                        if (nightAI && calamityGlobalNPC.newAI[3] >= spawnAnimationTime)
                            calamityGlobalNPC.newAI[3] += 1f;

                        return;
                    }

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && NPC.position.Y < player.position.Y)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int projectileShootGateValue = (bossRush ? 18 : expertMode ? 24 : 26) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                            NPC.ai[3] = -projectileShootGateValue;

                        if (NPC.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 npcCenter = NPC.Center;
                            npcCenter.X += NPC.velocity.X * 7f;
                            float playerXDist = player.position.X + player.width * 0.5f - npcCenter.X;
                            float playerYDist = player.Center.Y - npcCenter.Y;
                            float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);

                            float velocityBoost = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float projSpeed = (expertMode ? 10.25f : 9f) + velocityBoost;

                            if (revenge)
                                projSpeed *= 1.15f;

                            playerDistance = projSpeed / playerDistance;
                            playerXDist *= playerDistance;
                            playerYDist *= playerDistance;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), npcCenter.X, npcCenter.Y, playerXDist, playerYDist, ModContent.ProjectileType<HolyBlast>(), holyBlastDamage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                        }
                    }
                    else if (NPC.ai[3] < 0f)
                        NPC.ai[3] += 1f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyFire:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(6f * (1f - lifeRatio)) : (int)Math.Round(5f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 36 : 39) - shootBoost;
                        if (bossRush)
                            projectileShootGateValue = 27;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                        {
                            NPC.ai[3] = 0f;

                            Vector2 shootFrom = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 64f * NPC.scale);

                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;

                            if (nightAI)
                                projectileVelocityY *= 2f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom.X, shootFrom.Y, NPC.velocity.X * 0.25f, projectileVelocityY, ModContent.ProjectileType<HolyFire>(), holyFireDamage, 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.FlameCocoon:

                    if (!targetDead && !getFuckedAI)
                    {
                        if (NPC.velocity.Length() <= 2f)
                            NPC.velocity = Vector2.Zero;

                        if (NPC.velocity.Length() > 2f)
                        {
                            NPC.velocity *= 0.9f;
                            return;
                        }
                    }

                    float divisor = (expertMode ? 2f : 3f) + (float)Math.Floor(3f * lifeRatio) + (attackRateMult > 1D ? (float)Math.Ceiling(attackRateMult * 1.6) : 0f);
                    int totalFlameProjectiles = bossRush ? 45 : 36;
                    int chains = 4;
                    float interval = totalFlameProjectiles / chains * divisor;
                    double patternInterval = Math.Floor(NPC.ai[3] / interval);
                    int healingStarChance = revenge ? 8 : expertMode ? 6 : 4;

                    if (patternInterval % 2 == 0)
                    {
                        if (NPC.ai[3] % divisor == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                            bool normalSpread = calamityGlobalNPC.newAI[1] % 2f == 0f;
                            double radians = MathHelper.TwoPi / chains;
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = normalSpread ? new Vector2(0f, -cocoonProjVelocity) : new Vector2(-velocityX, -cocoonProjVelocity);
                            for (int i = 0; i < chains; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i + MathHelper.ToRadians(NPC.ai[2]));

                                int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
                                int dmgAmt = holyStarDamage;
                                if (Main.rand.NextBool(healingStarChance) && !death)
                                {
                                    projectileType = ModContent.ProjectileType<HolyLight>();
                                    dmgAmt = NPC.GetProjectileDamageNoScaling(projectileType);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, dmgAmt);
                                }
                                else if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, dmgAmt, 0f, Main.myPlayer);

                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(nightAI ? new Color(100, 200, 250) : (projectileType == ModContent.ProjectileType<HolyBurnOrb>() ? Color.Orange : Color.Green)).X, 1f, 0.5f);
                                dustColor.A = 255;
                                int maxDust = 3;
                                for (int j = 0; j < maxDust; j++)
                                {
                                    int dust = Dust.NewDust(fireFrom, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                    Main.dust[dust].position = fireFrom;
                                    Main.dust[dust].velocity = vector2 * cocoonProjVelocity * (j * 0.5f + 1f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].scale = 1f + j;
                                    Main.dust[dust].fadeIn = Main.rand.NextFloat() * 2f;
                                    Dust dust2 = Dust.CloneDust(dust);
                                    Dust dust3 = dust2;
                                    dust3.scale /= 2f;
                                    dust3 = dust2;
                                    dust3.fadeIn /= 2f;
                                    dust2.color = new Color(255, 255, 255, 255);
                                }
                            }

                            // Radial offset
                            NPC.ai[2] += 10f;
                        }

                        NPC.netUpdate = true;

                        // Prevent netUpdate from being blocked by the spam counter.
                        if (NPC.netSpam >= 10)
                            NPC.netSpam = 9;
                    }
                    else
                    {
                        NPC.ai[2] = 0f;

                        totalFlameProjectiles = bossRush ? 20 : 16;
                        if (NPC.ai[3] % (divisor * totalFlameProjectiles) == 0f)
                        {
                            calamityGlobalNPC.newAI[1] += 1f;
                            double radians = MathHelper.TwoPi / totalFlameProjectiles;
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                            double angleA = radians * 0.5;
                            double angleB = MathHelper.ToRadians(90f) - angleA;
                            float velocityX = (float)(cocoonProjVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                            Vector2 spinningPoint = NPC.ai[3] % (divisor * totalFlameProjectiles * 2f) == 0f ? new Vector2(-velocityX, -cocoonProjVelocity) : new Vector2(0f, -cocoonProjVelocity);
                            for (int i = 0; i < totalFlameProjectiles; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i);

                                int projectileType = ModContent.ProjectileType<HolyBurnOrb>();
                                int dmgAmt = holyStarDamage;
                                if (Main.rand.NextBool(healingStarChance) && !death)
                                {
                                    projectileType = ModContent.ProjectileType<HolyLight>();
                                    dmgAmt = NPC.GetProjectileDamageNoScaling(projectileType);
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, 0, 0f, Main.myPlayer, 0f, dmgAmt);
                                }
                                else if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, dmgAmt, 0f, Main.myPlayer);

                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(nightAI ? new Color(100, 200, 250) : (projectileType == ModContent.ProjectileType<HolyBurnOrb>() ? Color.Orange : Color.Green)).X, 1f, 0.5f);
                                dustColor.A = 255;
                                int maxDust = 3;
                                for (int j = 0; j < maxDust; j++)
                                {
                                    int dust = Dust.NewDust(fireFrom, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                    Main.dust[dust].position = fireFrom;
                                    Main.dust[dust].velocity = vector2 * cocoonProjVelocity * (j * 0.5f + 1f);
                                    Main.dust[dust].noGravity = true;
                                    Main.dust[dust].scale = 1f + j;
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

                    // Fire a flame towards every player, with a limit of 5
                    if (NPC.ai[3] % 60f == 0f && expertMode)
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
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, velocity2, type, holyStarDamage, 0f, Main.myPlayer);

                            Color dustColor = Main.hslToRgb(Main.rgbToHsl(nightAI ? new Color(100, 200, 250) : Color.Orange).X, 1f, 0.5f);
                            dustColor.A = 255;
                            int maxDust = 3;
                            for (int j = 0; j < maxDust; j++)
                            {
                                int dust = Dust.NewDust(fireFrom, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                Main.dust[dust].position = fireFrom;
                                Main.dust[dust].velocity = velocity2 * cocoonProjVelocity * 2f;
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].scale = 3f;
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

                    if (NPC.ai[3] == 0f)
                        DespawnSpecificProjectiles();

                    // Air is burning text
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= (phaseTime * 1.5f) && !text)
                    {
                        text = true;
                        string key = "Mods.CalamityMod.Status.Boss.ProfanedBossText";
                        Color messageColor = Color.Orange;

                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }

                    // Inflict Icarus Folly
                    if (NPC.ai[3] >= (phaseTime * 2f))
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Player player2 = Main.player[Main.myPlayer];
                            bool inLiquid = (player2.wet || player2.honeyWet) && !player2.lavaWet;

                            if (!player2.dead && player2.active && Vector2.Distance(player2.Center, NPC.Center) < 2800f && !inLiquid)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, player2.Center);
                                player2.AddBuff(ModContent.BuffType<IcarusFolly>(), 3000, true);

                                for (int i = 0; i < 40; i++)
                                {
                                    int icarusFollyDust = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[icarusFollyDust].velocity *= 3f;
                                    Main.dust[icarusFollyDust].noGravity = true;
                                    if (Main.rand.NextBool())
                                    {
                                        Main.dust[icarusFollyDust].scale = 0.5f;
                                        Main.dust[icarusFollyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                    }
                                }

                                for (int j = 0; j < 60; j++)
                                {
                                    int icarusFollyDust2 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 3f);
                                    Main.dust[icarusFollyDust2].noGravity = true;
                                    Main.dust[icarusFollyDust2].velocity *= 5f;
                                    icarusFollyDust2 = Dust.NewDust(new Vector2(player2.position.X, player2.position.Y),
                                        player2.width, player2.height, dustType, 0f, 0f, 100, default, 2f);
                                    Main.dust[icarusFollyDust2].velocity *= 2f;
                                    Main.dust[icarusFollyDust2].noGravity = true;
                                }
                            }
                        }

                        text = false;
                        AIState = (int)Phase.PhaseChange;
                        NPC.localAI[2] = attackDelayAfterCocoon;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.MoltenBlobs:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (distanceX > distanceNeededToShoot && NPC.position.Y < player.position.Y)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(5f * (1f - lifeRatio)) : (int)Math.Round(4f * (1f - lifeRatio));
                        int projectileShootGateValue = (expertMode ? 24 : 26) - shootBoost;
                        if (bossRush)
                            projectileShootGateValue = 18;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                            NPC.ai[3] = -projectileShootGateValue;

                        if (NPC.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 npcCenter = NPC.Center;
                            npcCenter.X += NPC.velocity.X * 7f;
                            float playerXDist = player.position.X + player.width * 0.5f - npcCenter.X;
                            float playerYDist = player.Center.Y - npcCenter.Y;
                            float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);

                            float shootBoost2 = death ? 4f * (1f - lifeRatio) : 2.5f * (1f - lifeRatio);
                            float projSpeed = (expertMode ? 10.25f : 9f) + shootBoost2;
                            if (bossRush)
                                projSpeed = 12.75f;

                            if (revenge)
                                projSpeed *= 1.15f;

                            playerDistance = projSpeed / playerDistance;
                            playerXDist *= playerDistance;
                            playerYDist *= playerDistance;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), npcCenter.X, npcCenter.Y, playerXDist * 0.1f, playerYDist, ModContent.ProjectileType<MoltenBlast>(), moltenBlastDamage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                        }
                    }
                    else if (NPC.ai[3] < 0f)
                        NPC.ai[3] += 1f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.HolyBomb:

                    // Attack delay after cocoon phase
                    if (delayAttacks)
                    {
                        NPC.localAI[2] -= 1f;
                        return;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[3] += 1f;

                        int shootBoost = death ? (int)Math.Round(12f * (1f - lifeRatio)) : (int)Math.Round(10f * (1f - lifeRatio));
                        int projectileShootGateValue = (bossRush ? 54 : expertMode ? 73 : 77) - shootBoost;

                        projectileShootGateValue = (int)(projectileShootGateValue * attackRateMult);

                        if (NPC.ai[3] >= projectileShootGateValue)
                        {
                            NPC.ai[3] = 0f;

                            Vector2 shootFrom = new Vector2(NPC.Center.X, NPC.position.Y + NPC.height - 14f * NPC.scale);

                            float projectileVelocityY = NPC.velocity.Y;
                            if (projectileVelocityY < 0f)
                                projectileVelocityY = 0f;

                            projectileVelocityY += expertMode ? 4f : 3f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), shootFrom.X, shootFrom.Y, NPC.velocity.X * 0.25f, projectileVelocityY, ModContent.ProjectileType<HolyBomb>(), holyBombDamage, 0f, Main.myPlayer);
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.SpearCocoon:

                    if (!targetDead && !getFuckedAI)
                    {
                        if (NPC.velocity.Length() <= 2f)
                            NPC.velocity = Vector2.Zero;

                        if (NPC.velocity.Length() > 2f)
                        {
                            NPC.velocity *= 0.9f;
                            return;
                        }
                    }

                    if (NPC.ai[1] == 0f)
                        DespawnSpecificProjectiles();

                    NPC.ai[2] += spearRate;
                    if (NPC.ai[2] >= (float)(baseSpearRate * attackRateMult))
                    {
                        NPC.ai[2] = 0f;

                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, fireFrom);

                        int projectileType = ModContent.ProjectileType<HolySpear>();

                        int totalDustPerSpear = 15;

                        if (calamityGlobalNPC.newAI[2] % 2f == 0f)
                        {
                            int totalSpearProjectiles = bossRush ? 15 : 12;
                            double radians = MathHelper.TwoPi / totalSpearProjectiles;
                            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-calamityGlobalNPC.newAI[1], -cocoonProjVelocity));

                            for (int i = 0; i < totalSpearProjectiles; i++)
                            {
                                Vector2 vector2 = spinningPoint.RotatedBy(radians * i) * cocoonProjVelocity;

                                for (int k = 0; k < totalDustPerSpear; k++)
                                {
                                    int dust = Dust.NewDust(fireFrom, 30, 30, dustType, vector2.X, vector2.Y, 0, default, 1f);
                                    Main.dust[dust].noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, vector2, projectileType, holySpearDamage, 0f, Main.myPlayer);

                                    if (CalamityWorld.LegendaryMode && revenge)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, -vector2, projectileType, holySpearDamage, 0f, Main.myPlayer);
                                }
                            }

                            if (spearRateIncrease > 1f)
                                spearRateIncrease = 1f;

                            float radialOffset = MathHelper.Lerp(0.2f, 0.4f, spearRateIncrease);
                            calamityGlobalNPC.newAI[1] += radialOffset;
                        }

                        calamityGlobalNPC.newAI[2] += 1f;

                        cocoonProjVelocity = death ? 14f : revenge ? 13f : expertMode ? 12f : 10f;
                        Vector2 velocity2 = Vector2.Normalize(player.Center - fireFrom) * cocoonProjVelocity;

                        for (int k = 0; k < totalDustPerSpear; k++)
                        {
                            int dust = Dust.NewDust(fireFrom, 30, 30, dustType, velocity2.X, velocity2.Y, 0, default, 1f);
                            Main.dust[dust].noGravity = true;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, velocity2, projectileType, holySpearDamage, 0f, Main.myPlayer, 1f, 0f);

                            if (CalamityWorld.LegendaryMode && revenge)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, -velocity2, projectileType, holySpearDamage, 0f, Main.myPlayer, 1f, 0f);
                        }
                    }

                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= phaseTime)
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.localAI[2] = attackDelayAfterCocoon;
                        NPC.TargetClosest();
                    }

                    break;

                case (int)Phase.Crystal:

                    if (!targetDead && !getFuckedAI)
                        NPC.velocity *= 0.9f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= crystalPhaseTime)
                    {
                        if (NPC.ai[1] == crystalPhaseTime)
                        {
                            Vector2 crystalSpawnPos = new Vector2(player.Center.X, player.Center.Y - 360f);
                            float distanceFromCrystalSpawnPos = Vector2.Distance(crystalSpawnPos, NPC.Center);

                            int maxHealDustIterations = (int)distanceFromCrystalSpawnPos;
                            int maxDust = 100;
                            int dustDivisor = maxHealDustIterations / maxDust;
                            if (dustDivisor < 2)
                                dustDivisor = 2;

                            Vector2 dustLineStart = new Vector2(NPC.Center.X, NPC.Center.Y + 64f * NPC.scale);
                            Vector2 dustLineEnd = crystalSpawnPos;
                            Vector2 currentDustPos = default;
                            Vector2 spinningpoint = new Vector2(0f, -3f).RotatedByRandom(MathHelper.Pi);
                            Vector2 dustVelocityMult = new Vector2(2.1f, 2f);
                            int dustSpawned = 0;
                            int maxDustLines = 3;
                            int blue = Main.DiscoB;
                            for (int i = 0; i < maxDustLines; i++)
                            {
                                for (int j = 0; j < maxHealDustIterations; j++)
                                {
                                    if (j % dustDivisor == 0)
                                    {
                                        currentDustPos = Vector2.Lerp(dustLineStart, dustLineEnd, j / (float)maxHealDustIterations);
                                        Color dustColor = Main.hslToRgb(Main.rgbToHsl(nightAI ? new Color(100, 200, 250) : new Color(255, 200, Math.Abs(Math.Abs(blue) - (int)(dustSpawned * 2.55f)))).X, 1f, 0.5f);
                                        dustColor.A = 255;
                                        int dust = Dust.NewDust(currentDustPos, 0, 0, 267, 0f, 0f, 0, dustColor, 1f);
                                        Main.dust[dust].position = currentDustPos + new Vector2(32f, 32f).RotatedByRandom(MathHelper.TwoPi) * i;
                                        Main.dust[dust].velocity = spinningpoint.RotatedBy(MathHelper.TwoPi * j / maxHealDustIterations) * dustVelocityMult * (0.8f + Main.rand.NextFloat() * 0.4f);
                                        Main.dust[dust].noGravity = true;
                                        Main.dust[dust].scale = 1f + i;
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

                                if (!nightAI)
                                    blue -= 255 / (maxDustLines - 1);
                            }

                            int totalDust = 36;
                            int circleDustSpawned = 0;
                            for (int k = 0; k < totalDust; k++)
                            {
                                Vector2 dustSpawnPos = Vector2.Normalize(NPC.velocity) * new Vector2(80f, 160f);
                                dustSpawnPos = dustSpawnPos.RotatedBy((double)((k - (totalDust / 2 - 1)) * MathHelper.TwoPi / totalDust), default) + dustLineEnd;
                                Vector2 dustVelocity = dustSpawnPos - dustLineEnd;
                                Color dustColor = Main.hslToRgb(Main.rgbToHsl(nightAI ? new Color(100, 200, 250) : new Color(255, 200, Math.Abs(Math.Abs(blue) - (int)(circleDustSpawned * 7.08f)))).X, 1f, 0.5f);
                                dustColor.A = 255;
                                int dust = Dust.NewDust(dustSpawnPos + dustVelocity, 0, 0, 267, dustVelocity.X, dustVelocity.Y, 0, dustColor, 1.4f);
                                Main.dust[dust].noGravity = true;
                                Main.dust[dust].noLight = true;
                                Main.dust[dust].velocity = dustVelocity * 0.33f;
                                circleDustSpawned++;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), crystalSpawnPos, Vector2.Zero, ModContent.ProjectileType<ProvidenceCrystal>(), crystalDamage, 0f, Main.myPlayer, lifeRatio, 0f);

                                if (nightAI)
                                    Main.projectile[proj].timeLeft = getFuckedAI ? gfbCrystalTime : nightCrystalTime;
                            }
                        }

                        if (NPC.ai[1] >= crystalPhaseTime + nightCrystalTime || !nightAI)
                        {
                            AIState = (int)Phase.PhaseChange;
                            NPC.TargetClosest();
                        }
                    }

                    break;

                case (int)Phase.Laser:

                    Vector2 dustPosOffset = new Vector2(27f, 59f);

                    float rotation = (nightAI ? 435f : 450f) + (guardianAmt * 5);

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] < 120f)
                    {
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

                                Vector2 dustPos = new Vector2(NPC.Center.X, NPC.Center.Y + 64f * NPC.scale) + ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * dustPosOffset / 2f;
                                int index = Dust.NewDust(dustPos - Vector2.One * 8f, 16, 16, dustType, NPC.velocity.X / 2f, NPC.velocity.Y / 2f, 0, default, 1f);
                                Main.dust[index].velocity = Vector2.Normalize(NPC.Center - dustPos) * 3.5f * (10f - extraDustAmt * 2f) / 10f;
                                Main.dust[index].noGravity = true;
                                Main.dust[index].scale = scalar;
                            }
                        }
                    }
                    else if (NPC.ai[2] < (revenge ? 220f : 300f))
                    {
                        if (NPC.ai[2] == 120f)
                        {
                            if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < 2800f)
                            {
                                SoundEngine.PlaySound(HolyRaySound, Main.LocalPlayer.Center);
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 velocity = player.Center - NPC.Center;
                                velocity.Normalize();

                                float beamDirection = -1f;
                                if (velocity.X < 0f)
                                    beamDirection = 1f;

                                // 60 degrees offset
                                velocity = velocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 6f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);

                                // -60 degrees offset
                                if (revenge)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);

                                if (nightAI && lifeRatio < 0.5f)
                                {
                                    rotation *= 0.33f;
                                    velocity = velocity.RotatedBy(-(double)beamDirection * MathHelper.TwoPi / 2f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, velocity.X, velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);

                                    if (revenge)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 64f * NPC.scale, -velocity.X, -velocity.Y, ModContent.ProjectileType<ProvidenceHolyRay>(), holyLaserDamage, 0f, Main.myPlayer, -beamDirection * MathHelper.TwoPi / rotation, NPC.whoAmI);
                                }

                                NPC.netUpdate = true;

                                // Prevent netUpdate from being blocked by the spam counter.
                                if (NPC.netSpam >= 10)
                                    NPC.netSpam = 9;
                            }
                        }
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= (revenge ? 235f : 315f))
                    {
                        AIState = (int)Phase.PhaseChange;
                        NPC.TargetClosest();
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
            NPC.velocity *= 0.9f;
            NPC.rotation = NPC.velocity.X * 0.004f;

            // Play an animation sound immediately. Also delete various projectiles.
            if (DeathAnimationTimer == 1f)
            {
                if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.WithinRange(NPC.Center, 4800f))
                    SoundEngine.PlaySound(DeathAnimationSound with { Volume = 1.65f });

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
                NPC.Opacity *= 0.97f;

            // Create an explosive wave shortly after the death animation begins.
            // The temporal offset coincides with the point at which the crystal shatter sound happens in the
            // above defeat scene sound.
            if (DeathAnimationTimer == 92f)
            {
                SoundEngine.PlaySound(HolyBlast.ImpactSound, NPC.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionBoom>(), 0, 0f);
            }

            // Explode as an enormous holy star before dying and dropping loot.
            if (Main.netMode != NetmodeID.MultiplayerClient && DeathAnimationTimer == 310f)
            {
                for (int i = 0; i < 80; i++)
                {
                    Vector2 sparkleVelocity = Main.rand.NextVector2Circular(23f, 23f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, sparkleVelocity, ModContent.ProjectileType<MajesticSparkle>(), 0, 0f);
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DyingSun>(), 0, 0f, 255);
            }

            // Idly release harmless cindiers.
            int shootRate = (int)MathHelper.Lerp(12f, 5f, Utils.GetLerpValue(0f, 250f, DeathAnimationTimer, true));
            if (DeathAnimationTimer % shootRate == shootRate - 1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 shootVelocity = Main.rand.NextVector2CircularEdge(13f, 13f) * Main.rand.NextFloat(0.7f, 1.3f);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, shootVelocity, ModContent.ProjectileType<SwirlingFire>(), 0, 0f, 255);
                }
            }

            // Do periodic syncs.
            if (Main.netMode == NetmodeID.Server && DeathAnimationTimer % 45f == 44f)
            {
                NPC.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (NPC.netSpam >= 10)
                    NPC.netSpam = 9;
            }

            // Die and create drops after the star is gone.
            if (DeathAnimationTimer >= 345f)
            {
                NPC.active = false;
                NPC.HitEffect();
                NPC.NPCLoot();

                NPC.netUpdate = true;

                // Prevent netUpdate from being blocked by the spam counter.
                if (NPC.netSpam >= 10)
                    NPC.netSpam = 9;
            }
        }

        public float CalculateBurnIntensity(float attackDelayAfterCocoon = 1f)
        {
            float distanceToTarget = Vector2.Distance(Main.player[NPC.target].Center, NPC.Center);
            float aiTimer = NPC.ai[3];

            // This bool is only relevant for non-Zenith night AI
            bool nightTime = NPC.localAI[1] == (float)BossMode.Night;

            float baseDistance = 2800f;
            float shorterFlameCocoonDistance = (CalamityWorld.death || nightTime) ? 600f : CalamityWorld.revenge ? 400f : Main.expertMode ? 200f : 0f;
            float shorterSpearCocoonDistance = (CalamityWorld.death || nightTime) ? 1000f : CalamityWorld.revenge ? 650f : Main.expertMode ? 300f : 0f;
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
            float shorterDistanceFade = Utils.GetLerpValue(0f, 120f, aiTimer, true);
            
            //Distance does not get shorter if in GFB / Guardians are alive
            if (!guardianAlive && NPC.localAI[1] < (float)BossMode.Red)
            {
                maxDistance = baseDistance;
                if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
                    maxDistance -= shorterDistance * shorterDistanceFade;
                else if (attackDelayAfterCocoon > 1f)
                    maxDistance -= shorterDistance * (NPC.localAI[2] / attackDelayAfterCocoon);
            }

            float drawFireDistanceStart = maxDistance - 800f;
            float previousBorderEnd = borderRadius;
            float clampedDistance = MathHelper.Clamp(maxDistance, previousBorderEnd - 10, previousBorderEnd + 10);
            // Only set the border distance if it's not called from playermisceffects, that way it has mod compatability
            borderRadius = clampedDistance;
            return Utils.GetLerpValue(drawFireDistanceStart, clampedDistance, distanceToTarget, true);
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override bool CheckDead()
        {
            NPC.life = 1;
            DespawnSpecificProjectiles(true);
            Dying = true;
            NPC.active = true;
            NPC.dontTakeDamage = true;

            NPC.netUpdate = true;

            // Prevent netUpdate from being blocked by the spam counter.
            if (NPC.netSpam >= 10)
                NPC.netSpam = 9;

            return false;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.Top.Y >= (Main.maxTilesY - 240f) * 16f)
                SpawnLootBox();

            // If Providence has not been killed, notify players of Uelibloom Ore
            if (!DownedBossSystem.downedProvidence)
            {
                string key2 = "Mods.CalamityMod.Status.Progression.ProfanedBossText3";
                Color messageColor2 = Color.Orange;
                string key3 = "Mods.CalamityMod.Status.Progression.TreeOreText";
                Color messageColor3 = Color.LightGreen;

                CalamityUtils.SpawnOre(ModContent.TileType<UelibloomOre>(), 17E-05, 0.55f, 0.9f, 8, 14, TileID.Mud);

                CalamityUtils.DisplayLocalizedText(key2, messageColor2);
                CalamityUtils.DisplayLocalizedText(key3, messageColor3);
            }

            if (challenge)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue("Mods.CalamityMod.Status.Progression.ProfanedBossText4"), Color.DarkOrange);
                }
            }

            // Mark Providence as dead
            DownedBossSystem.downedProvidence = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<ProvidenceBag>()));

            // Drops Rune of Cos on first kill
            npcLoot.AddIf(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<RuneofKos>(), desc: DropHelper.FirstKillText);

            npcLoot.AddConditionalPerPlayer(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.biomeType != 2 || !prov.hasTakenDaytimeDamage;
            }, ModContent.ItemType<ElysianWings>(), desc: DropHelper.ProvidenceHallowText);
            npcLoot.AddConditionalPerPlayer(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.biomeType == 2 || !prov.hasTakenDaytimeDamage;
            }, ModContent.ItemType<ElysianAegis>(), desc: DropHelper.ProvidenceUnderworldText);
            npcLoot.DefineConditionalDropSet(DropHelper.If((info) =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return prov.challenge;
            }, () => Main.expertMode, DropHelper.ProvidenceChallengeText)).Add(ModContent.ItemType<ProfanedSoulCrystal>());
            npcLoot.AddIf(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return !prov.hasTakenDaytimeDamage;
            }, ModContent.ItemType<ProfanedMoonlightDye>(), 1, 4, 4, desc: DropHelper.ProvidenceNightText);
            npcLoot.AddIf(info =>
            {
                Providence prov = info.npc.ModNPC<Providence>();
                return !prov.hasTakenDaytimeDamage;
            }, ModContent.ItemType<DivineGeode>(), 1, 30, 40);

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<HolyCollider>(),
                    ModContent.ItemType<SolarFlare>(),
                    ModContent.ItemType<TelluricGlare>(),
                    ModContent.ItemType<BlissfulBombardier>(),
                    ModContent.ItemType<PurgeGuzzler>(),
                    ModContent.ItemType<DazzlingStabberStaff>(),
                    ModContent.ItemType<MoltenAmputator>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<PristineFury>(), 10);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BlazingCore>()));

                // Materials
                normalOnly.Add(ModContent.ItemType<DivineGeode>(), 1, 25, 30);
                normalOnly.Add(ModContent.ItemType<UnholyEssence>(), 1, 20, 30);

                // Vanity
                normalOnly.Add(ModContent.ItemType<ProvidenceMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<ProvidenceTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<ProvidenceRelic>());

            // GFB ASE and Blasphemous Donut drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<AscendantSpiritEssence>(), 1, 1, 99, true);
                GFBOnly.Add(ModContent.ItemType<BlasphemousDonut>(), 1, 1117, 2201, true); // reference to the versions the guards were added and got their latest resprites
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<LoreProvidence>(), desc: DropHelper.FirstKillText);
        }

        private void SpawnLootBox()
        {
            int tileCenterX = (int)NPC.Center.X / 16;
            int tileCenterY = (int)NPC.Center.Y / 16;
            int halfBox = 5;
            for (int x = tileCenterX - halfBox; x <= tileCenterX + halfBox; x++)
            {
                for (int y = tileCenterY - halfBox; y <= tileCenterY + halfBox; y++)
                {
                    if ((x == tileCenterX - halfBox || x == tileCenterX + halfBox || y == tileCenterY - halfBox || y == tileCenterY + halfBox)
                        && !Main.tile[x, y].HasTile)
                    {
                        Main.tile[x, y].TileType = (ushort)ModContent.TileType<ProfanedRock>();
                        Main.tile[x, y].Get<TileWallWireStateData>().HasTile = true;
                    }
                    Main.tile[x, y].Get<LiquidData>().LiquidType = LiquidID.Water;
                    Main.tile[x, y].LiquidAmount = 0;

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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            void drawProvidenceInstance(Vector2 drawOffset, Color? colorOverride)
            {
                // This night bool is used for any off-color activity
                bool offColor = NPC.localAI[1] != (float)BossMode.Day;

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
                    switch (frameUsed)
                    {
                        case 0:
                            getTextureGlowString = baseGlowTextureString + "ProvidenceGlow";
                            getTextureGlow2String = baseGlowTextureString + "ProvidenceGlow2";
                            break;

                        case 1:
                            getTextureString = baseTextureString + "ProvidenceAlt";
                            getTextureGlowString = baseGlowTextureString + "ProvidenceAltGlow";
                            getTextureGlow2String = baseGlowTextureString + "ProvidenceAltGlow2";
                            break;
                        
                        case 2:
                            getTextureString = baseTextureString + "ProvidenceAttack";
                            getTextureGlowString = baseGlowTextureString + "ProvidenceAttackGlow";
                            getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackGlow2";
                            break;
                        
                        case 3:
                            getTextureString = baseTextureString + "ProvidenceAttackAlt";
                            getTextureGlowString = baseGlowTextureString + "ProvidenceAttackAltGlow";
                            getTextureGlow2String = baseGlowTextureString + "ProvidenceAttackAltGlow2";
                            break;

                        default:
                            break;
                    }
                }

                if (offColor)
                {
                    getTextureString += "Night";
                    getTextureGlowString += "Night";
                    getTextureGlow2String += "Night";
                }

                Texture2D texture = ModContent.Request<Texture2D>(getTextureString).Value;
                Texture2D textureGlow = ModContent.Request<Texture2D>(getTextureGlowString).Value;
                Texture2D textureGlow2 = ModContent.Request<Texture2D>(getTextureGlow2String).Value;

                SpriteEffects spriteEffects = SpriteEffects.None;
                if (NPC.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                // Draw the main boss texture + its afterimages
                Vector2 RotationCenter = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
                Color BaseColor = Color.White;
                float Brightness = 0.5f; // Ranges from 0 (full vibrance) to 1 (pure white)
                int maxAfterimages = 5;

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int i = 1; i < maxAfterimages; i += 2)
                    {
                        Color AfterimageColor = drawColor;
                        AfterimageColor = Color.Lerp(AfterimageColor, BaseColor, Brightness);
                        AfterimageColor = NPC.GetAlpha(AfterimageColor);
                        AfterimageColor *= (maxAfterimages - i) / 15f;
                        if (colorOverride != null)
                            AfterimageColor = colorOverride.Value;

                        Vector2 AfterimageBodyPosition = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        AfterimageBodyPosition -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        AfterimageBodyPosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                        spriteBatch.Draw(texture, AfterimageBodyPosition, NPC.frame, AfterimageColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
                    }
                }

                Vector2 BasePosition = NPC.Center - screenPos;
                BasePosition -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                BasePosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                spriteBatch.Draw(texture, BasePosition, NPC.frame, colorOverride ?? NPC.GetAlpha(drawColor), NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                // Draw the glowmask textures + their afterimages
                // These are the colors at their strongest point. It'll shift towards white by the brightness value used earlier.
                Color WingColor = Color.Yellow; //Default to day
                Color CrystalColor = Color.Violet;
                switch (NPC.localAI[1])
                {
                    case (float)BossMode.Red:
                        WingColor = Color.Red;
                        CrystalColor = Color.BlueViolet;
                        break;
                    case (float)BossMode.Orange:
                        WingColor = Color.Orange;
                        CrystalColor = Color.HotPink;
                        break;
                    case (float)BossMode.Yellow: // Same as day
                        break;
                    case (float)BossMode.Green:
                        WingColor = Color.Green;
                        CrystalColor = Color.Gold;
                        break;
                    case (float)BossMode.Blue: // Same as night
                    case (float)BossMode.Night:
                        WingColor = Color.Cyan;
                        CrystalColor = Color.BlueViolet;
                        break;
                    case (float)BossMode.Violet:
                        WingColor = Color.Magenta;
                        CrystalColor = Color.GreenYellow;
                        break;
                    default:
                        break;
                }

                Color BaseWingColor = Color.Lerp(WingColor, BaseColor, Brightness) * NPC.Opacity;
                Color BaseCrystalColor = Color.Lerp(CrystalColor, BaseColor, Brightness) * NPC.Opacity;
                if (colorOverride != null)
                {
                    BaseWingColor = colorOverride.Value;
                    BaseCrystalColor = colorOverride.Value;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int j = 1; j < maxAfterimages; j++)
                    {
                        Color AfterimageWingColor = BaseWingColor;
                        AfterimageWingColor = Color.Lerp(AfterimageWingColor, BaseColor, Brightness);
                        AfterimageWingColor = NPC.GetAlpha(AfterimageWingColor);
                        AfterimageWingColor *= (maxAfterimages - j) / 15f;
                        if (colorOverride != null)
                            AfterimageWingColor = colorOverride.Value;

                        Vector2 AfterimageGlowPosition = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        AfterimageGlowPosition -= new Vector2(textureGlow.Width, textureGlow.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        AfterimageGlowPosition += RotationCenter * NPC.scale + new Vector2(0f, NPC.gfxOffY) + drawOffset;
                        spriteBatch.Draw(textureGlow, AfterimageGlowPosition, NPC.frame, AfterimageWingColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                        Color AfterimageCrystalColor = BaseCrystalColor;
                        AfterimageCrystalColor = Color.Lerp(AfterimageCrystalColor, BaseColor, Brightness);
                        AfterimageCrystalColor = NPC.GetAlpha(AfterimageCrystalColor);
                        AfterimageCrystalColor *= (maxAfterimages - j) / 15f;
                        if (colorOverride != null)
                            AfterimageCrystalColor = colorOverride.Value;
                        spriteBatch.Draw(textureGlow2, AfterimageGlowPosition, NPC.frame, AfterimageCrystalColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(textureGlow, BasePosition, NPC.frame, BaseWingColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);

                spriteBatch.Draw(textureGlow2, BasePosition, NPC.frame, BaseCrystalColor, NPC.rotation, RotationCenter, NPC.scale, spriteEffects, 0f);
            }

            float burnIntensity = Utils.GetLerpValue(0f, 45f, DeathAnimationTimer, true);
            int totalProvidencesToDraw = (int)MathHelper.Lerp(1f, 30f, burnIntensity);
            for (int i = 0; i < totalProvidencesToDraw; i++)
            {
                float offsetAngle = MathHelper.TwoPi * i * 2f / totalProvidencesToDraw;
                float drawOffsetFactor = (float)Math.Sin(offsetAngle * 6f + Main.GlobalTimeWrappedHourly * MathHelper.Pi);
                drawOffsetFactor *= (float)Math.Pow(burnIntensity, 3f) * 50f;

                Vector2 drawOffset = offsetAngle.ToRotationVector2() * drawOffsetFactor;
                Color baseColor = Color.White * (MathHelper.Lerp(0.4f, 0.8f, burnIntensity) / totalProvidencesToDraw * 1.5f);
                baseColor.A = 0;

                baseColor = Color.Lerp(Color.White, baseColor, burnIntensity);
                drawProvidenceInstance(drawOffset, totalProvidencesToDraw == 1 ? null : (Color?)baseColor);
            }

            if (NPC.IsABestiaryIconDummy)
                return false;

            // Draw orange star while attacker is alive
            if (NPC.localAI[0] > 0f && NPC.localAI[0] < TimeForStarDespawn)
            {
                float lerpMult = MathHelper.Lerp(0.5f, 1.5f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) / 2f + 1f);
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/StarProj").Value;
                float drawOffsetAmt = (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon) ? 20f : 64f;
                Vector2 drawPos = NPC.Center + Vector2.UnitY * drawOffsetAmt * NPC.scale - Main.screenPosition;
                Color baseColor = Color.Lerp(Color.Yellow, Color.OrangeRed, (float)Math.Sin(Main.GlobalTimeWrappedHourly) / 2f + 1f);
                baseColor *= 0.5f;
                baseColor.A = 0;
                Color colorA = baseColor;
                Color colorB = baseColor * 0.5f;
                float opacityScaleDuringStarDespawn = (TimeForStarDespawn - NPC.localAI[0]) / TimeForStarDespawn;
                float scaleDuringStarDespawnScale = 1.8f;
                float scaleDuringStarDespawn = (1f - opacityScaleDuringStarDespawn) * scaleDuringStarDespawnScale;
                float colorScale = MathHelper.Lerp(0f, lerpMult, opacityScaleDuringStarDespawn);
                colorA *= colorScale;
                colorB *= colorScale;
                Vector2 origin = texture.Size() / 2f;
                Vector2 scale = new Vector2(1.5f + scaleDuringStarDespawn, 2.5f + scaleDuringStarDespawn) * lerpMult;
                float upRight = MathHelper.PiOver4 + NPC.rotation;
                float up = MathHelper.PiOver2 + NPC.rotation;
                float upLeft = 3f * MathHelper.PiOver4 + NPC.rotation;
                float left = MathHelper.Pi + NPC.rotation;
                Main.EntitySpriteDraw(texture, drawPos, null, colorA, upLeft, origin, scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorA, upRight, origin, scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorB, upLeft, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorB, upRight, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorA, up, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorA, left, origin, scale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorB, up, origin, scale * 0.36f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, colorB, left, origin, scale * 0.36f, SpriteEffects.None, 0);
            }

            // Draw shields while defender is alive
            if (NPC.localAI[3] > 0f && NPC.localAI[3] < TimeForShieldDespawn)
            {
                float maxOscillation = 60f;
                float minScale = 0.9f;
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
                Texture2D shieldTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleOpenCircleButBigger").Value;
                Rectangle shieldFrame = shieldTexture.Frame();
                Vector2 origin = shieldFrame.Size() * 0.5f;
                Vector2 shieldDrawPos = NPC.Center - screenPos;
                shieldDrawPos -= new Vector2(shieldTexture.Width, shieldTexture.Height) * NPC.scale / 2f;
                shieldDrawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                float minHue = 0.06f;
                float maxHue = 0.18f;
                float opacityScaleDuringShieldDespawn = (TimeForShieldDespawn - NPC.localAI[3]) / TimeForShieldDespawn;
                float scaleDuringShieldDespawnScale = 1.8f;
                float scaleDuringShieldDespawn = (1f - opacityScaleDuringShieldDespawn) * scaleDuringShieldDespawnScale;
                float colorScale = MathHelper.Lerp(0f, shieldOpacity, opacityScaleDuringShieldDespawn);
                Color color = Main.hslToRgb(MathHelper.Lerp(maxHue - minHue, maxHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.TwoPi) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                Color color2 = Main.hslToRgb(MathHelper.Lerp(minHue, maxHue - minHue, ((float)Math.Sin(Main.GlobalTimeWrappedHourly * MathHelper.Pi * 3f) + 1f) * 0.5f), 1f, 0.5f) * colorScale;
                color2.A = 0;
                color *= 0.6f;
                color2 *= 0.6f;
                float scaleMult = 2.75f + scaleDuringShieldDespawn;
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale2 * scaleMult * 0.45f, SpriteEffects.None, 0f);
                spriteBatch.Draw(shieldTexture, shieldDrawPos, shieldFrame, color2, NPC.rotation, origin, shieldScale2 * scaleMult * 0.5f, SpriteEffects.None, 0f);
                
                // The shield for the border MUST be drawn before the main shield, it becomes incredibly visually obnoxious otherwise.
                
                // The scale used for the noise overlay polygons also grows and shrinks
                // This is intentionally out of sync with the shield, and intentionally desynced per player
                // Don't put this anywhere less than 0.25f or higher than 1f. The higher it is, the denser / more zoomed out the noise overlay is.
                float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

                // Define shader parameters
                Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
                shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.058f); // Scrolling speed of polygonal overlay
                shieldEffect.Parameters["blowUpPower"].SetValue(2.8f);
                shieldEffect.Parameters["blowUpSize"].SetValue(0.4f);
                shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);
                
                shieldEffect.Parameters["shieldOpacity"].SetValue(opacityScaleDuringShieldDespawn);
                shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);
                
                Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, color, color2);
                
                // Define shader parameters for shield color
                shieldEffect.Parameters["shieldColor"].SetValue(color.ToVector3());
                shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());
                
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);
                
                // Fetch shield heat overlay texture (this is the neutrons fed to the shader)
                Texture2D heatTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Neurons2").Value;
                Vector2 pos = NPC.Center + NPC.gfxOffY * Vector2.UnitY - Main.screenPosition;
                Main.spriteBatch.Draw(heatTex, shieldDrawPos, null, Color.White, 0, heatTex.Size() / 2f, shieldScale * scaleMult * 0.5f, 0, 0);
            }
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            int totalFrames = 3;
            if (AIState == (int)Phase.FlameCocoon || AIState == (int)Phase.SpearCocoon)
            {
                if (!useDefenseFrames)
                {
                    NPC.frameCounter += Dying ? 0.25 : 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y >= frameHeight * totalFrames)
                    {
                        NPC.frame.Y = 0;
                        useDefenseFrames = true;
                    }
                }
                else
                {
                    NPC.frameCounter += Dying ? 0.25 : 1D;
                    if (NPC.frameCounter > 10D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }

                    if (NPC.frame.Y >= frameHeight * 2)
                        NPC.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (useDefenseFrames)
                    useDefenseFrames = false;

                NPC.frameCounter += Dying ? 0.25 : (NPC.Calamity().newAI[3] < 180f) ? 0.625 : 1D;
                if (NPC.frameCounter > 5D)
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * totalFrames)
                {
                    NPC.frame.Y = 0;
                    frameUsed++;
                }

                int totalSheets = 4;
                if (frameUsed >= totalSheets)
                    frameUsed = 0;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!hasTakenDaytimeDamage)
            {
                if (NPC.localAI[1] == (float)BossMode.Day)
                {
                    hasTakenDaytimeDamage = true;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
                        netMessage.Write((byte)NPC.whoAmI);
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
                    ModContent.ProjectileType<MiniGuardianRock>(),
                    ModContent.ProjectileType<MiniGuardianSpear>(),
                    ModContent.ProjectileType<SilvaCrystalExplosion>(),
                    ModContent.ProjectileType<GhostlyMine>(),
                    ModContent.ProjectileType<EnergyOrb>(),
                    ModContent.ProjectileType<IrradiatedAura>(),
                    ModContent.ProjectileType<SummonAstralExplosion>(),
                    ModContent.ProjectileType<ApparatusExplosion>(),
                    ModContent.ProjectileType<TarragonAura>()
                };

                bool allowedClass = projectile.CountsAsClass<SummonDamageClass>() || (!projectile.CountsAsClass<MeleeDamageClass>() && !projectile.CountsAsClass<RangedDamageClass>() && 
                    !projectile.CountsAsClass<MagicDamageClass>() && !projectile.CountsAsClass<ThrowingDamageClass>() && !projectile.CountsAsClass<SummonMeleeSpeedDamageClass>());
                bool allowedDamage = allowedClass && hit.Damage <= 75; //Flat 75 regardless of difficulty.
                //Absorber on-hit effects likely won't proc this but Deific Amulet and Astral Bulwark stars will proc this.
                bool allowedBabs = Main.player[projectile.owner].Calamity().pSoulArtifact && !Main.player[projectile.owner].Calamity().profanedCrystalBuffs;

                if ((exceptionList.TrueForAll(x => projectile.type != x) && !allowedDamage) || !allowedBabs)
                {
                    challenge = false;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
                        netMessage.Write((byte)NPC.whoAmI);
                        netMessage.Write(challenge);
                        netMessage.Send();
                    }
                }
            }
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (!hasTakenDaytimeDamage)
            {
                if (NPC.localAI[1] == (float)BossMode.Day)
                {
                    hasTakenDaytimeDamage = true;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.ProvidenceDyeConditionSync);
                        netMessage.Write((byte)NPC.whoAmI);
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
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.PSCChallengeSync);
                    netMessage.Write((byte)NPC.whoAmI);
                    netMessage.Write(challenge);
                    netMessage.Send();
                }
            }
        }

        // This will always put the boss to 1 health before dying, which makes external checks work.
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(NPC.life - 1);

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0 && !Dying)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(HurtSound, NPC.Center);
            }

            int dustType = ProvUtils.GetDustID(NPC.localAI[1]);
            for (int k = 0; k < 15; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, hit.HitDirection, -1f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread * Main.rand.NextFloat(), Mod.Find<ModGore>("Providence4").Type, NPC.scale);
                }
                NPC.position = NPC.Center;
                NPC.width = (int)(400 * NPC.scale);
                NPC.height = (int)(350 * NPC.scale);
                NPC.position -= NPC.Size * 0.5f;
                for (int d = 0; d < 60; d++)
                {
                    int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    Main.dust[fire].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 90; d++)
                {
                    int fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    fire = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                    Main.dust[fire].noGravity = true;
                }
            }
        }
    }

    //These will be used for almost every single one of her projectiles, so it's useful to have.
    public static class ProvUtils
    {
        public static Color GetProjectileColor(int Mode, int Alpha, bool Outline = false)
        {
            Color FinalColor = new Color(250, Outline ? 0 : 150, 0, Alpha); //Default to day
            switch (Mode)
            {
                case (int)Providence.BossMode.Red:
                    FinalColor = new Color(250, 100, Outline ? 200 : 100, Alpha);
                    break;
                case (int)Providence.BossMode.Orange:
                    FinalColor = new Color(250, 150, Outline ? 150 : 100, Alpha);
                    break;
                case (int)Providence.BossMode.Yellow: //Same as day
                    break;
                case (int)Providence.BossMode.Green:
                    FinalColor = new Color(Outline ? 200 : 100, 250, 100, Alpha);
                    break;
                case (int)Providence.BossMode.Blue: //Same as night
                case (int)Providence.BossMode.Night:
                    FinalColor = new Color(100, Outline ? 250 : 200, Outline ? 200 : 250, Alpha);
                    break;
                case (int)Providence.BossMode.Violet:
                    FinalColor = new Color(Outline ? 100 : 150, Outline ? 150 : 100, 250, Alpha);
                    break;
                default:
                    break;
            }

            if (Outline)
                FinalColor *= 0.1f;

            return FinalColor;
        }

        public static int GetDustID(float Mode)
        {
            int DustType = (int)CalamityDusts.ProfanedFire; //Default to day
            switch (Mode)
            {
                case (float)Providence.BossMode.Red:
                    DustType = DustID.RedTorch;
                    break;
                case (float)Providence.BossMode.Orange:
                    DustType = DustID.OrangeTorch;
                    break;
                case (float)Providence.BossMode.Yellow: //Same as day
                    break;
                case (float)Providence.BossMode.Green:
                    DustType = DustID.GreenTorch;
                    break;
                case (float)Providence.BossMode.Blue: //Same as night
                case (float)Providence.BossMode.Night:
                    DustType = (int)CalamityDusts.Nightwither;
                    break;
                case (float)Providence.BossMode.Violet:
                    DustType = DustID.PurpleTorch;
                    break;
                default:
                    break;
            }
            return DustType;
        }

        //Include debuffs inflicted by Providence's projectiles for all her forms
        //In the GFB seed, also includes negative healing
        public static void ApplyHitEffects(Player Target, int Mode, int BaseDuration, int NegativeHealValue)
        {
            int BuffType = ModContent.BuffType<HolyFlames>(); //Default to day
            float Multiplier = 1f; //Used to counterbalance Cursed Inferno and Shadowflame

            //Day and Night Providence inflicts 16-80 damage of debuffs depending on attacks
            //GFB Providence inflicts 24-120 damage (+50%) for half the colors, 26-130 (+62.5%) for another half
            switch (Mode)
            {
                case (int)Providence.BossMode.Red:
                    BuffType = ModContent.BuffType<BrimstoneFlames>();
                    break;
                case (int)Providence.BossMode.Orange:
                    BuffType = ModContent.BuffType<Dragonfire>();
                    Multiplier = 0.5f;
                    break;
                case (int)Providence.BossMode.Yellow: //Same as day
                    break;
                case (int)Providence.BossMode.Green:
                    BuffType = BuffID.CursedInferno;
                    Multiplier = 0.75f;
                    break;
                case (int)Providence.BossMode.Blue: //Same as night
                case (int)Providence.BossMode.Night:
                    BuffType = ModContent.BuffType<Nightwither>();
                    break;
                case (int)Providence.BossMode.Violet:
                    BuffType = ModContent.BuffType<Shadowflame>();
                    Multiplier = 0.60f; 
                    break;
                default:
                    break;
            }
            Target.AddBuff(BuffType, (int)(BaseDuration * Multiplier));
            
            //A. Specifically inflicts Vaporfied in quirky RGB Mode because it's a colorful debuff
            //B. Apply the negative healing
            if (Mode >= (int)Providence.BossMode.Red)
            {
                //Obligatory offensive guardian boosting negative heals
                if (CalamityGlobalNPC.holyBossAttacker != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.holyBossAttacker].active)
                        NegativeHealValue *= 2;
                }

                Target.HealEffect(-1 * NegativeHealValue, false);
                Target.statLife -= NegativeHealValue;
                if (Target.statLife < 0)
                {
                    PlayerDeathReason CustomSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ProvidenceAntiHealing").Format(Target.name));
                    Target.KillMe(CustomSource, NegativeHealValue, 0);
                }
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, Target.whoAmI, NegativeHealValue);

                Target.AddBuff(ModContent.BuffType<Vaporfied>(), (int)(BaseDuration * Multiplier));   
            }
        }
    }
}
