using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Events;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Update Bad Life Regen
        public override void UpdateBadLifeRegen()
        {
            if (Player.ownedProjectileCounts[ModContent.ProjectileType<BloodBoilerFire>()] > 0)
                noLifeRegen = true;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            double lifeRegenMult = death ? 1.5 : 1D;
            if (reaverDefense)
                lifeRegenMult *= 0.8;
            int lifeRegenLost = 0;

            // Initial Debuffs

            // Get fucked, Nebula Armor
            Player.nebulaLevelLife = 0;

            // Vanilla
            if (death)
            {
                if (Player.poisoned)
                    lifeRegenLost += 4;

                if (Player.onFire)
                    lifeRegenLost += 8;

                if (Player.tongued)
                    lifeRegenLost += 100;

                if (Player.venom)
                    lifeRegenLost += 12;

                if (Player.onFrostBurn)
                    lifeRegenLost += 12;

                if (Player.onFire2)
                    lifeRegenLost += 12;

                if (Player.burned)
                    lifeRegenLost += 60;

                if (Player.suffocating)
                    lifeRegenLost += 40;

                if (Player.electrified)
                {
                    lifeRegenLost += 8;
                    if (Player.controlLeft || Player.controlRight)
                        lifeRegenLost += 32;
                }
            }

            // Calamity
            if (shadowflame)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 30;
            }

            if (wDeath)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
            }

            if (aFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (weakBrimstoneFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 7;
            }

            if (bFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (nightwither)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (vaporfied)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 8;
            }

            if (cragsLava)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 30;
            }

            if (gsInferno)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += profanedCrystalBuffs ? 35 : 30;
            }

            if (astralInfection)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 20;
            }

            if (ZoneSulphur && Player.IsUnderwater() && !decayEffigy && !abyssalDivingSuit && !Player.lavaWet && !Player.honeyWet)
            {
                Player.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 2, true);
                pissWaterBoost++;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                int waterDivisor = 250;
                int minimumRegenLost = 8;
                if (sulfurSet && sulphurskin)
                {
                    waterDivisor = 500;
                    minimumRegenLost = 1;
                }
                else if (sulfurSet)
                {
                    waterDivisor = 400;
                    minimumRegenLost = 3;
                }
                else if (sulphurskin)
                {
                    waterDivisor = 350;
                    minimumRegenLost = 2;
                }
                int sulphurWater = pissWaterBoost / waterDivisor;
                if (sulphurWater < minimumRegenLost)
                    sulphurWater = minimumRegenLost;
                lifeRegenLost += sulphurWater;
            }
            else
                pissWaterBoost = 0;

            if (sulphurPoison)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 6;
                if (sulfurSet)
                {
                    lifeRegenLost -= 2;
                }
                if (sulphurskin)
                {
                    lifeRegenLost -= 2;
                }
            }

            if (hFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (banishingFire)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 60;
            }

            if (waterLeechBleeding)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 6;
                if (DownedBossSystem.downedAquaticScourge)
                    lifeRegenLost += 6;
                if (DownedBossSystem.downedPolterghast)
                    lifeRegenLost += 12;
            }

            if (pFlames)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 20;
            }

            if (bBlood)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 8;
            }

            if (vHex)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (dragonFire)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 18;
            }

            if (cDepth)
            {
                if (Player.statDefense > 0)
                {
                    int depthDamage = depthCharm ? 9 : 18;
                    int subtractDefense = (int)(Player.statDefense * 0.05); // 240 defense = 0 damage taken with depth charm
                    int calcDepthDamage = depthDamage - subtractDefense;

                    if (calcDepthDamage < 0)
                        calcDepthDamage = 0;

                    if (Player.lifeRegen > 0)
                        Player.lifeRegen = 0;

                    Player.lifeRegenTime = 0;
                    lifeRegenLost += calcDepthDamage;
                }
            }

            if (vodka)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (redWine)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
                if (baguette)
                    lifeRegenLost += 3;
            }
            if (grapeBeer)
            {
                alcoholPoisonLevel++;
            }
            if (moonshine)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (rum)
            {
                alcoholPoisonLevel++;
            }
            if (fabsolVodka)
            {
                alcoholPoisonLevel++;
            }
            if (fireball)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (whiskey)
            {
                alcoholPoisonLevel++;
            }
            if (everclear)
            {
                alcoholPoisonLevel += 2;
                lifeRegenLost += 10;
            }
            if (bloodyMary)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 2;
            }
            if (tequila)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (tequilaSunrise)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (screwdriver)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (caribbeanRum)
            {
                alcoholPoisonLevel++;
            }
            if (cinnamonRoll)
            {
                alcoholPoisonLevel++;
            }
            if (margarita)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (starBeamRye)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (moscowMule)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 2;
            }
            if (whiteWine)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (evergreenGin)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (Player.tipsy)
            {
                alcoholPoisonLevel++;
            }

            if (cirrusDress)
                alcoholPoisonLevel = 0;

            if (alcoholPoisonLevel > 3)
            {
                Player.nebulaLevelLife = 0;

                if (Player.whoAmI == Main.myPlayer)
                    Player.AddBuff(ModContent.BuffType<AlcoholPoisoning>(), 2, false);

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 3 * alcoholPoisonLevel;
            }

            if (manaOverloader)
            {
                if (Player.statMana > (int)(Player.statManaMax2 * 0.5))
                    lifeRegenLost += 3;
            }
            if (brimflameFrenzy)
            {
                Player.manaRegen = 0;
                Player.manaRegenBonus = 0;
                Player.manaRegenDelay = (int) Player.maxRegenDelay;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                lifeRegenLost += 42; //the meaning of death
            }

            if (witheredDebuff)
            {
                witheredWeaponHoldTime += witheringWeaponEnchant.ToDirectionInt();
                if (witheredWeaponHoldTime < 0)
                {
                    witheredWeaponHoldTime = 0;
                }
                else
                {
                    lifeRegenLost += (int)(5D * Math.Pow(1.5D, witheredWeaponHoldTime / 87D));
                    if (Player.lifeRegen > 0)
                        Player.lifeRegen = 0;
                }
            }
            else
                witheredWeaponHoldTime = 0;

            if (ManaBurn)
            {
                int debuffIndex = Player.FindBuffIndex(ModContent.BuffType<ManaBurn>());
                float debuffIntensity = debuffIndex == -1 ? 0f : Player.buffTime[debuffIndex] / (float)Player.manaSickTimeMax;

                lifeRegenLost += (int)(Math.Sqrt(debuffIntensity) * Math.Pow(6D, debuffIntensity + 1f));
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
            }

            Player.lifeRegen -= (int)(lifeRegenLost * lifeRegenMult);

            // Buffs

            if (divineBless)
            {
                if (Player.whoAmI == Main.myPlayer && Player.miscCounter % 15 == 0) // Flat 4 health per second
                {
                    if (!noLifeRegen)
                        Player.statLife += 1;
                }
            }

            if (bloodfinBoost)
            {
                if (Player.lifeRegen < 0)
                {
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;

                    Player.lifeRegen += 10;
                }
                else
                {
                    Player.lifeRegen += 5;
                    Player.lifeRegenTime += 10;
                }

                if (bloodfinTimer > 0)
                    bloodfinTimer--;

                if (Player.whoAmI == Main.myPlayer && bloodfinTimer <= 0)
                {
                    bloodfinTimer = 30;

                    if (Player.statLife < (int)(Player.statLifeMax2 * 0.75) && !noLifeRegen)
                        Player.statLife += 1;
                }
            }

            if (celestialJewel || astralArcanum)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                int defenseBoost = astralArcanum ? 15 : 11;
                if (lesserEffect)
                {
                    Player.lifeRegen += astralArcanum ? 2 : 1;
                    Player.statDefense += defenseBoost;
                }
                else
                {
                    if (Player.lifeRegen < 0)
                    {
                        if (Player.lifeRegenTime < 1800)
                            Player.lifeRegenTime = 1800;

                        Player.lifeRegen += astralArcanum ? 6 : 4;
                        Player.statDefense += defenseBoost;
                    }
                    else
                        Player.lifeRegen += astralArcanum ? 3 : 2;
                }
            }
            else if (crownJewel)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                if (lesserEffect)
                    Player.statDefense += 8;
                else
                {
                    if (Player.lifeRegen < 0)
                    {
                        if (Player.lifeRegenTime < 1800)
                            Player.lifeRegenTime = 1800;

                        Player.lifeRegen += 4;
                        Player.statDefense += 8;
                    }
                    else
                        Player.lifeRegen += 2;
                }
            }

            // Last Debuffs

            if (noLifeRegen)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;

                if (Player.lifeRegenCount > 0)
                    Player.lifeRegenCount = 0;
            }

            if (hInferno)
            {
                Player.nebulaLevelLife = 0;

                hInfernoBoost++;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                Player.lifeRegen -= (int)(hInfernoBoost * lifeRegenMult);

                if (Player.lifeRegen < -200)
                    Player.lifeRegen = -200;
            }
            else
                hInfernoBoost = 0;

            if (ZoneAbyss)
            {
                if (!Player.IsUnderwater())
                {
                    if (Player.statLife > 100)
                    {
                        Player.nebulaLevelLife = 0;

                        if (Player.lifeRegen > 0)
                            Player.lifeRegen = 0;

                        Player.lifeRegenTime = 0;
                        Player.lifeRegen -= (int)(160D * lifeRegenMult);
                    }
                }
            }

            if (weakPetrification)
            {
                if (Player.mount.Active)
                    Player.mount.Dismount(Player);
            }

            if (lol || (silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (DashID == GodSlayerDash.ID && Player.dashDelay < 0))
            {
                if (Player.lifeRegen < 0)
                    Player.lifeRegen = 0;
            }
        }
        #endregion

        #region Update Life Regen
        public override void UpdateLifeRegen()
        {
            // Fuck the 'sitting in a chair' meta.
            if (Player.sitting.isSitting)
                Player.lifeRegenTime -= 10;

            if (rum)
                Player.lifeRegen += 2;

            if (caribbeanRum)
                Player.lifeRegen += 2;

            if (aChicken)
                Player.lifeRegen += 1;

            if (cadence)
                Player.lifeRegen += 5;

            if (mushy)
                Player.lifeRegen += 1;

            if (permafrostsConcoction)
            {
                if (Player.statLife < actualMaxLife / 2)
                    Player.lifeRegen++;
                if (Player.statLife < actualMaxLife / 4)
                    Player.lifeRegen++;
                if (Player.statLife < actualMaxLife / 10)
                    Player.lifeRegen += 2;

                if (Player.poisoned || Player.onFire || bFlames)
                    Player.lifeRegen += 4;
            }

            if (tRegen)
                Player.lifeRegen += 3;

            if (sRegen)
                Player.lifeRegen += 2;

            if (hallowedRegen)
                Player.lifeRegen += 3;

            if (affliction || afflicted)
                Player.lifeRegen += 1;

            if (absorber)
            {
                if (Player.StandingStill() && Player.itemAnimation == 0)
                    Player.lifeRegen += 4;
            }

            if (aAmpoule)
            {
                Player.lifeRegen += 4;
            }
            else if (rOoze)
            {
                if (!Main.dayTime)
                    Player.lifeRegen += 4;
            }

            if (ursaSergeant)
            {
                if (Player.statLife <= (int)(actualMaxLife * 0.15))
                {
                    Player.lifeRegen += 3;
                    Player.lifeRegenTime += 3;
                }
                else if (Player.statLife <= (int)(actualMaxLife * 0.25))
                {
                    Player.lifeRegen += 2;
                    Player.lifeRegenTime += 2;
                }
                else if (Player.statLife <= (int)(actualMaxLife * 0.5))
                {
                    Player.lifeRegen += 1;
                    Player.lifeRegenTime += 1;
                }
            }

            if (polarisBoost)
            {
                Player.lifeRegen += 1;
                Player.lifeRegenTime += 1;
            }

            if (projRefRareLifeRegenCounter > 0)
            {
                Player.lifeRegenTime += 2;
                Player.lifeRegen += 2;
            }

            if (darkSunRing)
            {
                if (Main.eclipse || Main.dayTime)
                    Player.lifeRegen += 3;
            }

            if (phantomicHeartRegen <= 720 && phantomicHeartRegen >= 600)
            {
                Player.lifeRegen += 2;
                if (Main.rand.NextBool(2))
                {
                    int regen = Dust.NewDust(Player.position, Player.width, Player.height, 5, 0f, 0f, 200, new Color(99, 54, 84), 2f);
                    Main.dust[regen].noGravity = true;
                    Main.dust[regen].fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    Main.dust[regen].velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    Main.dust[regen].position = Player.Center - velocity;
                }
            }

            if (community)
            {
                float floatTypeBoost = 0.05f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) +
                    (NPC.downedBoss3 ? 0.01f : 0f) + // 0.1
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) +
                    (NPC.downedFishron ? 0.01f : 0f) + // 0.15
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (DownedBossSystem.downedProvidence ? 0.01f : 0f) +
                    (DownedBossSystem.downedDoG ? 0.01f : 0f) +
                    (DownedBossSystem.downedYharon ? 0.01f : 0f); // 0.2
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                int regenBoost = 1 + (integerTypeBoost / 5);
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    bool shouldAffect = CalamityLists.alcoholList.Contains(hasBuff);
                    if (shouldAffect)
                        lesserEffect = true;
                }
                if (Player.lifeRegen < 0)
                    Player.lifeRegen += lesserEffect ? 1 : regenBoost;
            }

            if (regenator)
            {
                Player.lifeRegenTime += 6;
                Player.lifeRegen += 12;
            }
            if (handWarmer && eskimoSet)
            {
                Player.lifeRegen += 2;
            }
            if (bloodPactBoost)
            {
                Player.lifeRegen += 2;
            }
            if (avertorBonus)
            {
                Player.lifeRegen += 4;
            }

            if (bloodflareSummon)
            {
                if (Player.statLife <= (int)(actualMaxLife * 0.5))
                    Player.lifeRegen += 2;
            }

            if (fearmongerSet && fearmongerRegenFrames > 0)
            {
                Player.lifeRegen += 7;

                if (Player.lifeRegenTime < 1800)
                    Player.lifeRegenTime = 1800;

                Player.lifeRegenTime += 4;
            }

            if (pinkCandle && !noLifeRegen)
            {
                // Every frame, add up 1/60th of the healing value (0.4% max HP per second)
                pinkCandleHealFraction += Player.statLifeMax2 * 0.004 / 60;

                if (pinkCandleHealFraction >= 1D)
                {
                    pinkCandleHealFraction = 0D;

                    if (Player.statLife < Player.statLifeMax2)
                        Player.statLife++;
                }
            }
            else
                pinkCandleHealFraction = 0D;

            if (reaverRegen && reaverRegenCooldown >= 60)
            {
                reaverRegenCooldown = 0;

                if (Player.statLife != Player.statLifeMax2 && !noLifeRegen)
                    Player.statLife += 1;
            }

            if (BloomStoneRegen)
            {
                float dayTimeCompletion = !Main.dayTime ? 1f : (float)(Main.time / Main.dayLength);
                float regenBenefitFactor = MathHelper.SmoothStep(0.25f, 1f, Utils.GetLerpValue(0f, 0.24f, dayTimeCompletion, true) * Utils.GetLerpValue(1f, 0.76f, dayTimeCompletion, true));

                Player.lifeRegen += (int)MathHelper.Lerp(2f, 6f, regenBenefitFactor);
                Player.lifeRegenTime += (int)MathHelper.Lerp(1f, 3f, regenBenefitFactor);
            }

            // Standing still healing bonuses (all exclusive with vanilla Shiny Stone)
            if (!Player.shinyStone)
            {
                int lifeRegenTimeMaxBoost = areThereAnyDamnBosses ? 450 : 1800;
                int lifeRegenMaxBoost = areThereAnyDamnBosses ? 1 : 4;
                float lifeRegenLifeRegenTimeMaxBoost = areThereAnyDamnBosses ? 8f : 30f;

                if (Player.StandingStill() && Player.itemAnimation == 0)
                {
                    bool boostedRegen = false;
                    bool noSunlight = false;
                    if (shadeRegen)
                    {
                        boostedRegen = true;
                        if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool(30))
                            {
                                int regen = Dust.NewDust(Player.position, Player.width, Player.height, 173, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = Player.Center - velocity;
                            }
                        }
                    }
                    else if (cFreeze)
                    {
                        boostedRegen = true;
                        if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool(30))
                            {
                                int regen = Dust.NewDust(Player.position, Player.width, Player.height, 67, 0f, 0f, 200, new Color(150, Main.DiscoG, 255), 0.75f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = Player.Center - velocity;
                            }
                        }
                    }
                    else if (draedonsHeart)
                    {
                        boostedRegen = true;
                        if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool(2))
                            {
                                int regen = Dust.NewDust(Player.position, Player.width, Player.height, 107, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = Player.Center - velocity;
                            }
                        }
                    }
                    else if (photosynthesis)
                    {
                        boostedRegen = true;
                        if (!Main.dayTime)
                            noSunlight = true;
                        if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool(2))
                            {
                                int regen = Dust.NewDust(Player.position, Player.width, Player.height, 244, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = Player.Center - velocity;
                            }
                        }
                    }
                    if (boostedRegen)
                    {
                        int lifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenTimeMaxBoost : (lifeRegenTimeMaxBoost / 5);
                        int lifeRegenMaxBoost2 = !noSunlight ? lifeRegenMaxBoost : (lifeRegenMaxBoost / 5);
                        float lifeRegenLifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenLifeRegenTimeMaxBoost : (lifeRegenLifeRegenTimeMaxBoost / 5);

                        if (Player.lifeRegenTime > 90 && Player.lifeRegenTime < lifeRegenTimeMaxBoost2)
                            Player.lifeRegenTime = lifeRegenTimeMaxBoost2;

                        Player.lifeRegenTime += lifeRegenMaxBoost2;
                        Player.lifeRegen += lifeRegenMaxBoost2;

                        float num3 = Player.lifeRegenTime * 2.5f; // lifeRegenTime max is 3600
                        num3 /= 300f;
                        if (num3 > 0f)
                        {
                            if (num3 > lifeRegenLifeRegenTimeMaxBoost2)
                                num3 = lifeRegenLifeRegenTimeMaxBoost2;

                            Player.lifeRegen += (int)num3;
                        }
                        if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                        {
                            Player.lifeRegenCount++;
                        }
                    }
                }
            }

            // The Camper regen boost activates while moving so it can stack with Shiny Stone like effects
            if (camper && Player.statLife < actualMaxLife && !Player.StandingStill())
            {
                float camperRegenMult = areThereAnyDamnBosses ? 1.25f : 2f;
                int camperRegenCount = areThereAnyDamnBosses ? 1 : 4;
                Player.lifeRegen = (int)(Player.lifeRegen * camperRegenMult);
                Player.lifeRegenCount += camperRegenCount;
                if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool(2))
                {
                    int regen = Dust.NewDust(Player.position, Player.width, Player.height, 12, 0f, 0f, 200, Color.OrangeRed, 1f);
                    Main.dust[regen].noGravity = true;
                    Main.dust[regen].fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    Main.dust[regen].velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    Main.dust[regen].position = Player.Center - velocity;
                }
            }

            if (Player.statLife < actualMaxLife)
            {
                bool noLifeRegenCap = (Player.shinyStone || draedonsHeart || cFreeze || shadeRegen || photosynthesis || camper) &&
                    Player.StandingStill() && Player.itemAnimation == 0;

                if (!noLifeRegenCap)
                {
                    // Max HP = 400
                    // 350 HP = 1 - 0.875 * 10 = 1.25 = 1
                    // 100 HP = 1 - 0.25 * 10 = 7.5 = 7
                    // 200 HP = 1 - 0.5 * 10 = 5
                    int lifeRegenScale = (int)((1f - (Player.statLife / actualMaxLife)) * 10f); // 9 to 0 (1% HP to 100%)
                    if (Player.lifeRegen > lifeRegenScale)
                    {
                        float lifeRegenScalar = 1f + (Player.statLife / actualMaxLife); // 1 to 2 (1% HP to 100%)
                        int defLifeRegen = (int)(Player.lifeRegen / lifeRegenScalar);
                        Player.lifeRegen = defLifeRegen;
                    }
                }
            }

            if (BossRushEvent.BossRushActive)
            {
                if (CalamityConfig.Instance.BossRushHealthCurse)
                {
                    if (Player.lifeRegen > 0)
                        Player.lifeRegen = 0;

                    Player.lifeRegenTime = 0;

                    if (Player.lifeRegenCount > 0)
                        Player.lifeRegenCount = 0;
                }
            }

            // For the stat meter
            lifeRegenStat = Player.lifeRegen;
        }
        #endregion
    }
}
