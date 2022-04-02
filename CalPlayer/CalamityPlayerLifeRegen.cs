using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
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
            if (player.ownedProjectileCounts[ModContent.ProjectileType<BloodBoilerFire>()] > 0)
                noLifeRegen = true;

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            double lifeRegenMult = death ? 1.5 : 1D;
            if (reaverDefense)
                lifeRegenMult *= 0.8;
            int lifeRegenLost = 0;

            // Initial Debuffs

            // Get fucked, Nebula Armor
            player.nebulaLevelLife = 0;

            // Vanilla
            if (death)
            {
                if (player.poisoned)
                    lifeRegenLost += 4;

                if (player.onFire)
                    lifeRegenLost += 8;

                if (player.tongued)
                    lifeRegenLost += 100;

                if (player.venom)
                    lifeRegenLost += 12;

                if (player.onFrostBurn)
                    lifeRegenLost += 12;

                if (player.onFire2)
                    lifeRegenLost += 12;

                if (player.burned)
                    lifeRegenLost += 60;

                if (player.suffocating)
                    lifeRegenLost += 40;

                if (player.electrified)
                {
                    lifeRegenLost += 8;
                    if (player.controlLeft || player.controlRight)
                        lifeRegenLost += 32;
                }
            }

            // Calamity
            if (shadowflame)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 30;
            }

            if (wDeath)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
            }

            if (aFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (weakBrimstoneFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 7;
            }

            if (bFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += abaddon ? 8 : 16;
            }

            if (nightwither)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (vaporfied)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 8;
            }

            if (cragsLava)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 30;
            }

            if (gsInferno)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += profanedCrystalBuffs ? 35 : 30;
            }

            if (astralInfection)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 20;
            }

            if (ZoneSulphur && player.IsUnderwater() && !decayEffigy && !player.lavaWet && !player.honeyWet)
            {
                player.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 2, true);
                pissWaterBoost++;

                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
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
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
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
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (banishingFire)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 60;
            }

            if (waterLeechBleeding)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 6;
                if (CalamityWorld.downedAquaticScourge)
                    lifeRegenLost += 6;
                if (CalamityWorld.downedPolterghast)
                    lifeRegenLost += 12;
            }

            if (pFlames)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += reducedPlagueDmg ? 10 : 20;
            }

            if (bBlood)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 8;
            }

            if (vHex)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 16;
            }

            if (dragonFire)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 18;
            }

            if (cDepth)
            {
                if (player.statDefense > 0)
                {
                    int depthDamage = depthCharm ? 9 : 18;
                    int subtractDefense = (int)(player.statDefense * 0.05); // 240 defense = 0 damage taken with depth charm
                    int calcDepthDamage = depthDamage - subtractDefense;

                    if (calcDepthDamage < 0)
                        calcDepthDamage = 0;

                    if (player.lifeRegen > 0)
                        player.lifeRegen = 0;

                    player.lifeRegenTime = 0;
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

            if (cirrusDress)
                alcoholPoisonLevel = 0;

            if (alcoholPoisonLevel > 3)
            {
                player.nebulaLevelLife = 0;

                if (player.whoAmI == Main.myPlayer)
                    player.AddBuff(ModContent.BuffType<AlcoholPoisoning>(), 2, false);

                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                lifeRegenLost += 3 * alcoholPoisonLevel;
            }

            if (manaOverloader)
            {
                if (player.statMana > (int)(player.statManaMax2 * 0.5))
                    lifeRegenLost += 3;
            }
            if (brimflameFrenzy)
            {
                player.manaRegen = 0;
                player.manaRegenBonus = 0;
                player.manaRegenDelay = (int) player.maxRegenDelay;
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
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
                    if (player.lifeRegen > 0)
                        player.lifeRegen = 0;
                }
            }
            else
                witheredWeaponHoldTime = 0;

            if (ManaBurn)
            {
                int debuffIndex = player.FindBuffIndex(ModContent.BuffType<ManaBurn>());
                float debuffIntensity = debuffIndex == -1 ? 0f : player.buffTime[debuffIndex] / (float)Player.manaSickTimeMax;

                lifeRegenLost += (int)(Math.Sqrt(debuffIntensity) * Math.Pow(6D, debuffIntensity + 1f));
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;
            }

            player.lifeRegen -= (int)(lifeRegenLost * lifeRegenMult);

            // Buffs

            if (divineBless)
            {
                if (player.whoAmI == Main.myPlayer && player.miscCounter % 15 == 0) // Flat 4 health per second
                {
                    if (!noLifeRegen)
                        player.statLife += 1;
                }
            }

            if (bloodfinBoost)
            {
                if (player.lifeRegen < 0)
                {
                    if (player.lifeRegenTime < 1800)
                        player.lifeRegenTime = 1800;

                    player.lifeRegen += 10;
                }
                else
                {
                    player.lifeRegen += 5;
                    player.lifeRegenTime += 10;
                }

                if (bloodfinTimer > 0)
                    bloodfinTimer--;

                if (player.whoAmI == Main.myPlayer && bloodfinTimer <= 0)
                {
                    bloodfinTimer = 30;

                    if (player.statLife < (int)(player.statLifeMax2 * 0.75) && !noLifeRegen)
                        player.statLife += 1;
                }
            }

            if (celestialJewel || astralArcanum)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                int defenseBoost = astralArcanum ? 15 : 11;
                if (lesserEffect)
                {
                    player.lifeRegen += astralArcanum ? 2 : 1;
                    player.statDefense += defenseBoost;
                }
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                            player.lifeRegenTime = 1800;

                        player.lifeRegen += astralArcanum ? 6 : 4;
                        player.statDefense += defenseBoost;
                    }
                    else
                        player.lifeRegen += astralArcanum ? 3 : 2;
                }
            }
            else if (crownJewel)
            {
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }

                if (lesserEffect)
                    player.statDefense += 8;
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                            player.lifeRegenTime = 1800;

                        player.lifeRegen += 4;
                        player.statDefense += 8;
                    }
                    else
                        player.lifeRegen += 2;
                }
            }

            // Last Debuffs

            if (noLifeRegen)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;
            }

            if (hInferno)
            {
                player.nebulaLevelLife = 0;

                hInfernoBoost++;

                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;
                player.lifeRegen -= (int)(hInfernoBoost * lifeRegenMult);

                if (player.lifeRegen < -200)
                    player.lifeRegen = -200;
            }
            else
                hInfernoBoost = 0;

            if (ZoneAbyss)
            {
                if (!player.IsUnderwater())
                {
                    if (player.statLife > 100)
                    {
                        player.nebulaLevelLife = 0;

                        if (player.lifeRegen > 0)
                            player.lifeRegen = 0;

                        player.lifeRegenTime = 0;
                        player.lifeRegen -= (int)(160D * lifeRegenMult);
                    }
                }
            }

            if (weakPetrification)
            {
                if (player.mount.Active)
                    player.mount.Dismount(player);
            }

            if (lol || (silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (dashMod == 9 && player.dashDelay < 0))
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }
        #endregion

        #region Update Life Regen
        public override void UpdateLifeRegen()
        {
            if (rum)
                player.lifeRegen += 2;

            if (caribbeanRum)
                player.lifeRegen += 2;

            if (aChicken)
                player.lifeRegen += 1;

            if (cadence)
                player.lifeRegen += 5;

            if (mushy)
                player.lifeRegen += 1;

            if (permafrostsConcoction)
            {
                if (player.statLife < actualMaxLife / 2)
                    player.lifeRegen++;
                if (player.statLife < actualMaxLife / 4)
                    player.lifeRegen++;
                if (player.statLife < actualMaxLife / 10)
                    player.lifeRegen += 2;

                if (player.poisoned || player.onFire || bFlames)
                    player.lifeRegen += 4;
            }

            if (tRegen)
                player.lifeRegen += 3;

            if (sRegen)
                player.lifeRegen += 2;

            if (hallowedRegen)
                player.lifeRegen += 3;

            if (affliction || afflicted)
                player.lifeRegen += 1;

            if (absorber)
            {
                if (player.StandingStill() && player.itemAnimation == 0)
                    player.lifeRegen += 4;
            }

            if (aAmpoule)
            {
                player.lifeRegen += 4;
            }
            else if (rOoze)
            {
                if (!Main.dayTime)
                    player.lifeRegen += 4;
            }

            if (ursaSergeant)
            {
                if (player.statLife <= (int)(actualMaxLife * 0.15))
                {
                    player.lifeRegen += 3;
                    player.lifeRegenTime += 3;
                }
                else if (player.statLife <= (int)(actualMaxLife * 0.25))
                {
                    player.lifeRegen += 2;
                    player.lifeRegenTime += 2;
                }
                else if (player.statLife <= (int)(actualMaxLife * 0.5))
                {
                    player.lifeRegen += 1;
                    player.lifeRegenTime += 1;
                }
            }

            if (polarisBoost)
            {
                player.lifeRegen += 1;
                player.lifeRegenTime += 1;
            }

            if (projRefRareLifeRegenCounter > 0)
            {
                player.lifeRegenTime += 2;
                player.lifeRegen += 2;
            }

            if (darkSunRing)
            {
                if (Main.eclipse || Main.dayTime)
                    player.lifeRegen += 3;
            }

            if (phantomicHeartRegen <= 720 && phantomicHeartRegen >= 600)
            {
                player.lifeRegen += 2;
                if (Main.rand.NextBool(2))
                {
                    int regen = Dust.NewDust(player.position, player.width, player.height, 5, 0f, 0f, 200, new Color(99, 54, 84), 2f);
                    Main.dust[regen].noGravity = true;
                    Main.dust[regen].fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    Main.dust[regen].velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    Main.dust[regen].position = player.Center - velocity;
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
                    (CalamityWorld.downedProvidence ? 0.01f : 0f) +
                    (CalamityWorld.downedDoG ? 0.01f : 0f) +
                    (CalamityWorld.downedYharon ? 0.01f : 0f); // 0.2
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                int regenBoost = 1 + (integerTypeBoost / 5);
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = CalamityLists.alcoholList.Contains(hasBuff);
                    if (shouldAffect)
                        lesserEffect = true;
                }
                if (player.lifeRegen < 0)
                    player.lifeRegen += lesserEffect ? 1 : regenBoost;
            }

            if (regenator)
            {
                player.lifeRegenTime += 6;
                player.lifeRegen += 12;
            }
            if (handWarmer && eskimoSet)
            {
                player.lifeRegen += 2;
            }
            if (bloodPactBoost)
            {
                player.lifeRegen += 2;
            }
            if (avertorBonus)
            {
                player.lifeRegen += 4;
            }

            if (bloodflareSummon)
            {
                if (player.statLife <= (int)(actualMaxLife * 0.5))
                    player.lifeRegen += 2;
            }

            if (fearmongerSet && fearmongerRegenFrames > 0)
            {
                player.lifeRegen += 7;

                if (player.lifeRegenTime < 1800)
                    player.lifeRegenTime = 1800;

                player.lifeRegenTime += 4;
            }

            if (pinkCandle && !noLifeRegen)
            {
                // Every frame, add up 1/60th of the healing value (0.4% max HP per second)
                pinkCandleHealFraction += player.statLifeMax2 * 0.004 / 60;

                if (pinkCandleHealFraction >= 1D)
                {
                    pinkCandleHealFraction = 0D;

                    if (player.statLife < player.statLifeMax2)
                        player.statLife++;
                }
            }
            else
                pinkCandleHealFraction = 0D;

            if (reaverRegen && reaverRegenCooldown >= 60)
            {
                reaverRegenCooldown = 0;

                if (player.statLife != player.statLifeMax2 && !noLifeRegen)
                    player.statLife += 1;
            }

            if (BloomStoneRegen)
            {
                float dayTimeCompletion = !Main.dayTime ? 1f : (float)(Main.time / Main.dayLength);
                float regenBenefitFactor = MathHelper.SmoothStep(0.25f, 1f, Utils.InverseLerp(0f, 0.24f, dayTimeCompletion, true) * Utils.InverseLerp(1f, 0.76f, dayTimeCompletion, true));

                player.lifeRegen += (int)MathHelper.Lerp(2f, 6f, regenBenefitFactor);
                player.lifeRegenTime += (int)MathHelper.Lerp(1f, 3f, regenBenefitFactor);
            }

            // Standing still healing bonuses (all exclusive with vanilla Shiny Stone)
            if (!player.shinyStone)
            {
                int lifeRegenTimeMaxBoost = areThereAnyDamnBosses ? 450 : 1800;
                int lifeRegenMaxBoost = areThereAnyDamnBosses ? 1 : 4;
                float lifeRegenLifeRegenTimeMaxBoost = areThereAnyDamnBosses ? 8f : 30f;

                if (player.StandingStill() && player.itemAnimation == 0)
                {
                    bool boostedRegen = false;
                    bool noSunlight = false;
                    if (shadeRegen)
                    {
                        boostedRegen = true;
                        if (player.lifeRegen > 0 && player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30))
                            {
                                int regen = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = player.Center - velocity;
                            }
                        }
                    }
                    else if (cFreeze)
                    {
                        boostedRegen = true;
                        if (player.lifeRegen > 0 && player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30))
                            {
                                int regen = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 200, new Color(150, Main.DiscoG, 255), 0.75f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = player.Center - velocity;
                            }
                        }
                    }
                    else if (draedonsHeart)
                    {
                        boostedRegen = true;
                        if (player.lifeRegen > 0 && player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
                            {
                                int regen = Dust.NewDust(player.position, player.width, player.height, 107, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = player.Center - velocity;
                            }
                        }
                    }
                    else if (photosynthesis)
                    {
                        boostedRegen = true;
                        if (!Main.dayTime)
                            noSunlight = true;
                        if (player.lifeRegen > 0 && player.statLife < actualMaxLife)
                        {
                            if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
                            {
                                int regen = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 200, default, 1f);
                                Main.dust[regen].noGravity = true;
                                Main.dust[regen].fadeIn = 1.3f;
                                Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                                Main.dust[regen].velocity = velocity;
                                velocity.Normalize();
                                velocity *= 34f;
                                Main.dust[regen].position = player.Center - velocity;
                            }
                        }
                    }
                    if (boostedRegen)
                    {
                        int lifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenTimeMaxBoost : (lifeRegenTimeMaxBoost / 5);
                        int lifeRegenMaxBoost2 = !noSunlight ? lifeRegenMaxBoost : (lifeRegenMaxBoost / 5);
                        float lifeRegenLifeRegenTimeMaxBoost2 = !noSunlight ? lifeRegenLifeRegenTimeMaxBoost : (lifeRegenLifeRegenTimeMaxBoost / 5);

                        if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost2)
                            player.lifeRegenTime = lifeRegenTimeMaxBoost2;

                        player.lifeRegenTime += lifeRegenMaxBoost2;
                        player.lifeRegen += lifeRegenMaxBoost2;

                        float num3 = player.lifeRegenTime * 2.5f; // lifeRegenTime max is 3600
                        num3 /= 300f;
                        if (num3 > 0f)
                        {
                            if (num3 > lifeRegenLifeRegenTimeMaxBoost2)
                                num3 = lifeRegenLifeRegenTimeMaxBoost2;

                            player.lifeRegen += (int)num3;
                        }
                        if (player.lifeRegen > 0 && player.statLife < actualMaxLife)
                        {
                            player.lifeRegenCount++;
                        }
                    }
                }
            }

            // The Camper regen boost activates while moving so it can stack with Shiny Stone like effects
            if (camper && player.statLife < actualMaxLife && !player.StandingStill())
            {
                float camperRegenMult = areThereAnyDamnBosses ? 1.25f : 2f;
                int camperRegenCount = areThereAnyDamnBosses ? 1 : 4;
                player.lifeRegen = (int)(player.lifeRegen * camperRegenMult);
                player.lifeRegenCount += camperRegenCount;
                if (Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2))
                {
                    int regen = Dust.NewDust(player.position, player.width, player.height, 12, 0f, 0f, 200, Color.OrangeRed, 1f);
                    Main.dust[regen].noGravity = true;
                    Main.dust[regen].fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    Main.dust[regen].velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    Main.dust[regen].position = player.Center - velocity;
                }
            }

            if (player.statLife < actualMaxLife)
            {
                bool noLifeRegenCap = (player.shinyStone || draedonsHeart || cFreeze || shadeRegen || photosynthesis || camper) &&
                    player.StandingStill() && player.itemAnimation == 0;

                if (!noLifeRegenCap)
                {
                    // Max HP = 400
                    // 350 HP = 1 - 0.875 * 10 = 1.25 = 1
                    // 100 HP = 1 - 0.25 * 10 = 7.5 = 7
                    // 200 HP = 1 - 0.5 * 10 = 5
                    int lifeRegenScale = (int)((1f - (player.statLife / actualMaxLife)) * 10f); // 9 to 0 (1% HP to 100%)
                    if (player.lifeRegen > lifeRegenScale)
                    {
                        float lifeRegenScalar = 1f + (player.statLife / actualMaxLife); // 1 to 2 (1% HP to 100%)
                        int defLifeRegen = (int)(player.lifeRegen / lifeRegenScalar);
                        player.lifeRegen = defLifeRegen;
                    }
                }
            }

            if (BossRushEvent.BossRushActive)
            {
                if (CalamityConfig.Instance.BossRushHealthCurse)
                {
                    if (player.lifeRegen > 0)
                        player.lifeRegen = 0;

                    player.lifeRegenTime = 0;

                    if (player.lifeRegenCount > 0)
                        player.lifeRegenCount = 0;
                }
            }

            // For the stat meter
            lifeRegenStat = player.lifeRegen;
        }
        #endregion
    }
}
