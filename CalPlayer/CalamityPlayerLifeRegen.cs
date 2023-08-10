using CalamityMod.Items.Accessories;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityMod.Systems;
using System.Linq;

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

            if (irradiated)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 4;
            }

            // Slowly increase the sulphuric water poisoning effect. Once it's high enough, the player starts taking damage over time.
            bool nearSafeZone = false;
            if (SulphuricWaterSafeZoneSystem.NearbySafeTiles.Count >= 1)
            {
                Point closestSafeZone = SulphuricWaterSafeZoneSystem.NearbySafeTiles.Keys.OrderBy(t => t.ToVector2().DistanceSQ(Player.Center / 16f)).First();
                if (Vector2.Distance(Player.Center.ToTileCoordinates().ToVector2(), closestSafeZone.ToVector2()) < SulphuricWaterSafeZoneSystem.NearbySafeTiles[closestSafeZone] * 17f)
                    nearSafeZone = true;
            }
            
            float ASPoisonLevel = 0f;
            if (CalamityGlobalNPC.aquaticScourge >= 0 && Main.zenithWorld)
            {
                NPC AS = Main.npc[CalamityGlobalNPC.aquaticScourge];
                //if the player is 50 blocks or more away from the head
                if (AS.life < AS.lifeMax) //Only poison when damaged
                    ASPoisonLevel = Utils.GetLerpValue(800f, 1600f, Vector2.Distance(Player.Center, AS.Center), true);
            }

            bool ASPoisoning = ASPoisonLevel > 0f;
            if (ASPoisoning || ((ZoneSulphur || Player.Calamity().ZoneAbyssLayer1) && !Player.creativeGodMode && Player.IsUnderwater() && !decayEffigy && !abyssalDivingSuit && !Player.lavaWet && !Player.honeyWet && !nearSafeZone))
            {
                float increment = 1f / SulphSeaWaterSafetyTime;
                //No way to mitigate AS Poisoning
                if (ASPoisoning)
                    increment *= 4f + (8f * ASPoisonLevel);
                if (sulphurskin && !ASPoisoning)
                    increment *= 0.5f;
                if (sulfurSet && !ASPoisoning)
                    increment *= 0.5f;

                SulphWaterPoisoningLevel = MathHelper.Clamp(SulphWaterPoisoningLevel + increment, 0f, 1f);
                if (SulphWaterPoisoningLevel >= 1f)
                {
                    SulphWaterPoisoningLevel = 0f;
                    Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.SulphurMeter").Format(Player.name)), Math.Min(Player.statLifeMax2 / 4, 150), 0);
                }
            }
            else
                SulphWaterPoisoningLevel = MathHelper.Clamp(SulphWaterPoisoningLevel - 1f / SulphSeaWaterRecoveryTime, 0f, 1f);

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
                lifeRegenLost += 36;
            }

            if (miracleBlight)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                lifeRegenLost += 40;
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
                lifeRegenLost += 4;
            }
            if (tequila)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 1;
            }
            if (tequilaSunrise)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 2;
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
                lifeRegenLost += 2;
            }
            if (moscowMule)
            {
                alcoholPoisonLevel++;
                lifeRegenLost += 4;
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

            if (alcoholPoisonLevel > (cirrusDress ? 5 : 3))
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

            // Permafrost's Concoction increases life regen while afflicted with a fire debuff
            if (permafrostsConcoction)
            {
                if (Player.onFire || Player.onFire2 || Player.onFire3 || Player.burned || shadowflame || weakBrimstoneFlames || bFlames || cragsLava || gsInferno || hFlames || banishingFire || dragonFire)
                {
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;

                    Player.lifeRegen += 6;
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

            if ((silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (DashID == GodSlayerDash.ID && Player.dashDelay < 0))
            {
                if (Player.lifeRegen < 0)
                    Player.lifeRegen = 0;
            }
        }
        #endregion

        #region Update Life Regen
        public override void UpdateLifeRegen()
        {
            if (rum)
                Player.lifeRegen += 2;

            if (caribbeanRum)
                Player.lifeRegen += 2;

            if (aChicken)
                Player.lifeRegen += 1;

            if (mushy)
                Player.lifeRegen += 2;

            if (permafrostsConcoction)
            {
                if (Player.statLife < actualMaxLife / 2)
                    Player.lifeRegen++;
                if (Player.statLife < actualMaxLife / 4)
                    Player.lifeRegen++;
                if (Player.statLife < actualMaxLife / 10)
                    Player.lifeRegen += 2;
            }

            if (tRegen)
                Player.lifeRegen += 3;

            if (sRegen)
                Player.lifeRegen += 2;

            if (PinkJellyRegen)
                Player.lifeRegen += 4;

            if (GreenJellyRegen)
                Player.lifeRegen += 6;

            if (AbsorberRegen)
                Player.lifeRegen += 7;

            if (hallowedRegen)
                Player.lifeRegen += 3;

            if (affliction || afflicted)
                Player.lifeRegen += 1;

            if (trinketOfChi || chiRegen)
                Player.lifeRegen += 2;

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

            if (evolutionLifeRegenCounter > 0)
            {
                Player.lifeRegenTime += 2;
                Player.lifeRegen += 2;
            }

            if (darkSunRing)
            {
                if (Main.eclipse || Main.dayTime)
                    Player.lifeRegen += Main.eclipse ? 2 : 4;
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
                int regenBoost = 1 + (int)(TheCommunity.CalculatePower() * TheCommunity.RegenMultiplier);
                bool lesserEffect = false;
                for (int l = 0; l < Player.MaxBuffs; l++)
                {
                    int hasBuff = Player.buffType[l];
                    lesserEffect = CalamityLists.alcoholList.Contains(hasBuff);
                }
                if (Player.lifeRegen < 0)
                    Player.lifeRegen += lesserEffect ? 1 : regenBoost;
            }

            if (regenator)
            {
                Player.lifeRegenTime += 3;
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

            // The Camper counteracts the regen loss while moving horizontally
            if (camper && (Player.velocity.X != 0 && Player.grappling[0] <= 0))
            {
                // Vanilla base regen rate which gets boosted when resting
                // The first 6 boosts increment every 300 frames, up to 6 at 1800
                // Then, the last 3 boosts increment every 600 frames, up to 9 at 3600 which is the cap
                int baseRegenRate = (int)(Math.Clamp(Player.lifeRegenTime / 300f, 0f, 6f) + Math.Clamp((Player.lifeRegenTime - 1800f) / 600f, 0f, 3f));
                // Normally 1.25 while resting and 0.5 while not
                Player.lifeRegen += (int)(baseRegenRate * 0.75f);

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

            // Life regen soft cap.
            if (Player.statLife < actualMaxLife)
            {
                // The soft cap doesn't apply if the player is not moving and not using a weapon while having any of the following:
                // Shiny Stone, Cosmic Freeze buff from the Cosmic Discharge, Demonshade Armor, Photosynthesis Potion buff or The Camper.
                int baseLifeRegenBoost = 4;
                bool noLifeRegenCap = (Player.shinyStone || cFreeze || shadeRegen || photosynthesis || camper) &&
                    Player.StandingStill() && Player.itemAnimation == 0;

                if (!noLifeRegenCap)
                {
                    // Calculate the % of HP the player has left.
                    float maxLifeRatio = Player.statLife / (float)actualMaxLife;

                    // Calculate the ratio of the player's current max life relative to the starting HP of 100.
                    // This makes the soft cap far less harsh at lower amounts of max life.
                    // Ranges from 20 (at 100 max life) to 4 (at 500 max life) to 2 (at 1000 max life) to 1 (at greater than 2000 max life).
                    int lifeRegenSoftCapMax = 20;
                    int lifeRegenSoftCapMin = (int)Math.Round(100f / actualMaxLife * lifeRegenSoftCapMax);

                    // The soft cap for life regen which ranges from 20 (at less than 5% HP) to 1 (at greater than or equal to 95% HP).
                    // This value is capped at a min and max amount.
                    int lifeRegenSoftCap = (int)MathHelper.Clamp((int)Math.Round((1f - maxLifeRatio) * lifeRegenSoftCapMax), lifeRegenSoftCapMin, lifeRegenSoftCapMax);

                    // If life regen is greater than the calculated soft cap, reduce it.
                    if (Player.lifeRegen - baseLifeRegenBoost > lifeRegenSoftCap)
                    {
                        // The scalar used to calculate how much the life regen stat should be reduced by.
                        // Ranges from 1 (at 0% HP) to 2 (at 100% HP).
                        float lifeRegenScalar = 1f + maxLifeRatio;

                        // Calculate the amount of life regen the player should get according to the soft cap and their current % HP remaining.
                        // The higher the player's % HP remaining the less life regen they get and vice versa.
                        int defLifeRegen = (int)((Player.lifeRegen - baseLifeRegenBoost) / lifeRegenScalar);

                        // Set the player's life regen to the scaled amount.
                        Player.lifeRegen = baseLifeRegenBoost + defLifeRegen;
                    }
                }
            }
        }
        #endregion
    }
}
