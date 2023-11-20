using System;
using System.Linq;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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

            // Universal +25% increase to DoT debuff damage in Death Mode
            float deathNegativeRegenBonus = 0.25f;
            float calamityDebuffMultiplier = 1f + (CalamityWorld.death ? deathNegativeRegenBonus : 0f);

            // Cumulative amount of DoT debuff negative life regen from Calamity debuffs (or changes to vanilla debuffs)
            float totalNegativeLifeRegen = 0;

            #region Damage over Time Debuffs (Negative Life Regen)

            // Vanilla debuffs (+25% damage over time in Death Mode is applied here)
            if (CalamityWorld.death)
            {
                int totalVanillaDoT = 0;

                if (Player.poisoned && !purity)
                    totalVanillaDoT += 4;

                if (Player.onFire && !purity)
                    totalVanillaDoT += 8;

                if (Player.tongued)
                    totalVanillaDoT += 100;

                if (Player.venom && !purity)
                    totalVanillaDoT += 12;

                if (Player.onFrostBurn && !purity)
                    totalVanillaDoT += 12;

                if (Player.onFire2 && !purity)
                    totalVanillaDoT += 12;

                if (Player.burned)
                    totalVanillaDoT += 60;

                if (Player.suffocating)
                    totalVanillaDoT += 40;

                if (Player.electrified && !purity)
                {
                    totalVanillaDoT += 8;
                    if (Player.controlLeft || Player.controlRight)
                        totalVanillaDoT += 32;
                }

                // Tally up total current vanilla DoT so it can be added as extra DoT from Death Mode
                totalNegativeLifeRegen += totalVanillaDoT * deathNegativeRegenBonus;
            }

            //
            // Calamity debuffs (Vanilla Shadowflame is added here)
            //
            void ApplyDoTDebuff(bool hasDebuff, int negativeLifeRegenToApply, bool immuneCondition = false)
            {
                if (!hasDebuff || immuneCondition)
                    return;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                totalNegativeLifeRegen += negativeLifeRegenToApply * calamityDebuffMultiplier;
            }

            // Whispering Death sets positive regen to zero but doesn't actually deal any damage
            ApplyDoTDebuff(wDeath, 0);

            ApplyDoTDebuff(irradiated, 4, purity);
            int sulphurDoT = 6 - (sulfurSet ? 2 : 0) - (sulphurskin ? 2 : 0);
            ApplyDoTDebuff(sulphurPoison, sulphurDoT, purity);
            ApplyDoTDebuff(rTide, 6, purity);
            ApplyDoTDebuff(weakBrimstoneFlames, 7);
            ApplyDoTDebuff(bBlood, 8, purity);
            ApplyDoTDebuff(brainRot, 8, purity);
            ApplyDoTDebuff(elementalMix, 50, purity);
            ApplyDoTDebuff(vaporfied, 8, purity);
            ApplyDoTDebuff(bFlames, abaddon ? 10 : 30, purity);
            ApplyDoTDebuff(nightwither, reducedNightwitherDamage ? 20 : 40, purity);
            ApplyDoTDebuff(hFlames, reducedHolyFlamesDamage ? 20 : 40, purity);
            ApplyDoTDebuff(vHex, 30);
            ApplyDoTDebuff(cDepth, 18, purity);
            ApplyDoTDebuff(astralInfection, 24, infectedJewel || purity);
            ApplyDoTDebuff(pFlames, 30, purity);
            ApplyDoTDebuff(cragsLava, 30);
            ApplyDoTDebuff(shadowflame, 30, purity);
            // Profaned Soul Crystal turns you into Providence, a God, and you take more damage from God Slayer Inferno
            ApplyDoTDebuff(gsInferno, profanedCrystalBuffs ? 60 : 50);
            ApplyDoTDebuff(dragonFire, 60);
            ApplyDoTDebuff(miracleBlight, 80);
            ApplyDoTDebuff(banishingFire, 60);

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
            #endregion

            #region Alcohol
            if (vodka)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
            }
            if (redWine)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
                if (baguette)
                    totalNegativeLifeRegen += 3;
            }
            if (grapeBeer)
            {
                alcoholPoisonLevel++;
            }
            if (moonshine)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
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
                totalNegativeLifeRegen += 1;
            }
            if (whiskey)
            {
                alcoholPoisonLevel++;
            }
            if (everclear)
            {
                alcoholPoisonLevel += 2;
                totalNegativeLifeRegen += 10;
            }
            if (bloodyMary)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 4;
            }
            if (tequila)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
            }
            if (tequilaSunrise)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 2;
            }
            if (screwdriver)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
            }
            if (caribbeanRum)
            {
                alcoholPoisonLevel++;
            }
            if (cinnamonRoll)
            {
                alcoholPoisonLevel++;
            }
            if (oldFashioned)
            {
                alcoholPoisonLevel++;
            }
            if (margarita)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
            }
            if (starBeamRye)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 2;
            }
            if (moscowMule)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 4;
            }
            if (whiteWine)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
            }
            if (evergreenGin)
            {
                alcoholPoisonLevel++;
                totalNegativeLifeRegen += 1;
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
                totalNegativeLifeRegen += 3 * alcoholPoisonLevel;
            }
            #endregion

            if (manaOverloader)
            {
                if (Player.statMana > (int)(Player.statManaMax2 * 0.5))
                    totalNegativeLifeRegen += 3;
            }

            if (brimflameFrenzy)
            {
                Player.manaRegen = 0;
                Player.manaRegenBonus = 0;
                Player.manaRegenDelay = (int) Player.maxRegenDelay;
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                totalNegativeLifeRegen += 42; //the meaning of death
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
                    totalNegativeLifeRegen += (int)(5D * Math.Pow(1.5D, witheredWeaponHoldTime / 87D));
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

                totalNegativeLifeRegen += (int)(Math.Sqrt(debuffIntensity) * Math.Pow(6D, debuffIntensity + 1f));
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
            }

            //
            // ACTUALLY APPLY NEGATIVE LIFE REGEN
            //

            // At the last second, Reaver defense helm reduces DoT debuffs by 20%
            if (reaverDefense)
                totalNegativeLifeRegen = (int)(0.8f * totalNegativeLifeRegen);

            Player.lifeRegen -= (int)totalNegativeLifeRegen;

            #region Life Regen That Works Even During DoT Debuffs

            // Honey Dew (and upgrades)
            if (alwaysHoneyRegen)
            {
                // Exact copy of vanilla Honey behavior, but does not stack with actually standing in Honey
                if (!Player.honey)
                {
                    Player.lifeRegen += 2;
                    Player.lifeRegenTime += 1;

                    // Grants +2 life regen if negative life regen would otherwise occur.
                    // However, this can't bring regen into the positives.
                    if (Player.lifeRegen < 0)
                    {
                        Player.lifeRegen += 2;
                        if (Player.lifeRegen > 0)
                            Player.lifeRegen = 0;
                    }
                }
            }

            if (honeyDewHalveDebuffs)
            {
                // Tick down all sickness debuffs; this makes them expire 2x faster
                // Upgrades increase the sets of debuffs which expire faster
                for (int l = 0; l < Player.MaxBuffs; ++l)
                {
                    int buffID = Player.buffType[l];
                    if (Player.buffTime[l] <= 2)
                        continue;
                    bool shouldHalveDuration = CalamityLists.sicknessDebuffList.Contains(buffID);
                    if (livingDewHalveDebuffs)
                        shouldHalveDuration |= CalamityLists.fireDebuffList.Contains(buffID);
                    if (purity)
                        shouldHalveDuration |= CalamityLists.debuffList.Contains(buffID);

                    if (shouldHalveDuration)
                        --Player.buffTime[l];
                }
            }

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
            if (permafrostsConcoction && Player.buffType.Any(CalamityLists.fireDebuffList.Contains))
            {
                if (Player.lifeRegenTime < 1800)
                    Player.lifeRegenTime = 1800;

                Player.lifeRegen += 6;
            }

            // Grant life regen based on missing health for Radiant Ooze, Ambrosial Ampule, and purity
            if (rOoze || aAmpoule || purity)
            {
                float missingLifeRatio = (Player.statLifeMax2 - Player.statLife) / Player.statLifeMax2;
                //Ambrosial Ampule and ooze give between 2 and 6 hp/s, Purity gives between 3 and 7 hp/s
                float lifeRegenToGive = MathHelper.Lerp( purity ? 6f : 4f, purity ? 14f : 12f, missingLifeRatio);
                Player.lifeRegen += (int)lifeRegenToGive;
            }

            if (purity)
            {
                int intendedPurityDefense = 0;
                int currentDebuffs = Player.buffType.Count(CalamityLists.debuffList.Contains);
                if (currentDebuffs > 0)
                {
                    // Healing rate is normally 5 HP/s (+1 every 12 frames)
                    // However, that 12 frames can and will slowly increase if you try to abuse this accessory
                    int healFrameCadence = 12;

                    // Healing slows down after 5 seconds (300 frames) debuffed. For every 15 frames thereafter the cadence slows
                    // There is no upper limit to how slow it can get and it can take a very long time to reset to normal
                    int punishmentFrames = PurityHealSlowdownFrames - 300;
                    //lowest punishment is three full seconds between the one health heal
                    if (healFrameCadence < 180)
                        healFrameCadence += (punishmentFrames < 0) ? 0 : punishmentFrames / 15;

                    if (Player.miscCounter % healFrameCadence == healFrameCadence - 1)
                        Player.Heal(1);

                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;

                    intendedPurityDefense = 20 + (currentDebuffs - 1) * 8;
                    if (jewelBonusDefense < intendedPurityDefense)
                        jewelBonusDefense = intendedPurityDefense;

                    // Count up total frames spent healing for slowdown.
                    ++PurityHealSlowdownFrames;
                }

                // If the defense should be ticking down to some lower value, do that.
                // Purity loses 1 point of defense every second.
                if (Player.miscCounter % 60 == 0 && jewelBonusDefense > intendedPurityDefense)
                    --jewelBonusDefense;

                // If the player is clear of all debuffs then gradually reduce the slowdown frames
                if (currentDebuffs <= 0)
                {
                    --PurityHealSlowdownFrames;
                    if (PurityHealSlowdownFrames < 0)
                        PurityHealSlowdownFrames = 0;
                }

                // Actually apply defense bonus
                Player.statDefense += jewelBonusDefense;
            }

            // Infected Jewel does not stack with Purity
            else if (infectedJewel)
            {
                Player.lifeRegen += 2;

                // If the player has any debuffs, give the extra life regen and defense
                // More defense is given for each additional debuff
                int intendedJewelDefense = 0;
                int currentDebuffs = Player.buffType.Count(CalamityLists.debuffList.Contains);
                if (currentDebuffs > 0)
                {
                    Player.lifeRegen += 4;
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;

                    intendedJewelDefense = 16 + (currentDebuffs - 1) * 5;
                    if (jewelBonusDefense < intendedJewelDefense)
                        jewelBonusDefense = intendedJewelDefense;
                }

                // If the defense should be ticking down to some lower value, do that.
                // Infected Jewel loses 1 point of defense every 20 frames.
                if (Player.miscCounter % 60 == 0 && jewelBonusDefense > intendedJewelDefense)
                    --jewelBonusDefense;

                // Actually apply defense bonus
                Player.statDefense += jewelBonusDefense;
            }

            // Crown Jewel does not stack with Purity or Infected Jewel
            else if (crownJewel)
            {
                Player.lifeRegen += 2;

                // If any debuff is detected, provide even more life regen and massively accelerate it
                if (Player.buffType.Any(CalamityLists.debuffList.Contains))
                {
                    Player.lifeRegen += 3;
                    if (Player.lifeRegenTime < 1800)
                        Player.lifeRegenTime = 1800;
                }
            }
            #endregion

            // During Silva revive or God Slayer dash, all negative life regen is canceled
            if ((silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (DashID == GodSlayerDash.ID && Player.dashDelay < 0))
            {
                if (Player.lifeRegen < 0)
                    Player.lifeRegen = 0;
            }

            #region Things That Disable Even That Life Regen
            //
            // Yes, really, there's a list of conditions under which life regen doesn't work
            // even if it's life regen that normally works during a damage over time debuff.
            //
            // 1. No life regen bool (Blood Boiler usage or wearing Omega Blue armor)
            // 2. Being too far from Providence cocoon ("Holy Inferno")
            // 3. Air drowning in the Abyss
            //

            if (noLifeRegen)
            {
                Player.nebulaLevelLife = 0;

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
                Player.lifeRegen -= (int)(hInfernoBoost * calamityDebuffMultiplier);

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
                        Player.lifeRegen -= (int)(160D * calamityDebuffMultiplier);
                    }
                }
            }

            // TODO -- Why is this here?
            if (weakPetrification)
            {
                if (Player.mount.Active)
                    Player.mount.Dismount(Player);
            }
            #endregion

            // Chalice of the Blood God bleedout
            // The bleedout is applied by directly reducing the player's health. It is not canceled by anything.
            ChaliceOfTheBloodGod.HandleBleedout(Player);
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
                Player.lifeRegen += 5;

            if (AbsorberRegen)
                Player.lifeRegen += 6;

            if (hallowedRegen)
                Player.lifeRegen += 3;

            if (affliction || afflicted)
                Player.lifeRegen += 1;

            if (trinketOfChi || chiRegen)
                Player.lifeRegen += 2;


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
                if (Main.rand.NextBool())
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

            #region Standing Still Life Regen
            // Standing still healing bonuses (all are exclusive with vanilla Shiny Stone, but all function similarly)
            if (!Player.shinyStone && Player.StandingStill() && Player.velocity.Y == 0 && Player.itemAnimation == 0)
            { 
                bool honeyDewWorking = honeyTurboRegen && Player.honeyWet;
                bool anyStandingStillLifeRegen = shadeRegen || cFreeze || honeyDewWorking || photosynthesis || aAmpoule || purity;
                bool onlyPhotosynthesisAtNight = !shadeRegen && !cFreeze && !honeyDewWorking && photosynthesis && !Main.dayTime;

                // Divides all negative life regen by two before applying any other effects.
                if (anyStandingStillLifeRegen && Player.lifeRegen < 0)
                    Player.lifeRegen /= 2;
                
                // Spawn dust of some flavor while actually regenerating, aAmpule and purity have a slightly different looking style
                if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                {
                    int dustType = shadeRegen ? 173 : cFreeze ? 67 : honeyDewWorking ? DustID.Honey2 : photosynthesis ? 244 : aAmpoule ? 228 : purity ? 187 : -1;
                    bool dustSpawnRolled = Main.rand.Next(30000) < Player.lifeRegenTime || purity ? Main.rand.NextBool() : aAmpoule ? Main.rand.NextBool(4) : Main.rand.NextBool(30);
                    if (dustType != -1 && dustSpawnRolled)
                    {
                        int regen = Dust.NewDust(Player.position, Player.width, Player.height, dustType, 0f, 0f, purity || aAmpoule ? 80 : 200, default, purity || aAmpoule ? 0.5f : 1f);
                        Main.dust[regen].noGravity = true;
                        Main.dust[regen].fadeIn = 1.3f;
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                        Main.dust[regen].velocity = velocity;
                        velocity.Normalize();
                        velocity *= purity || aAmpoule ? 55f : 34f;
                        Main.dust[regen].position = Player.Center - velocity;
                    }
                }

                // Actually apply "standing still" regeneration (the stats are granted even at full health)
                float regenTimeNeededForTurboRegen = shadeRegen ? 40f : cFreeze ? 60f : honeyDewWorking ? 90f : photosynthesis ? 90f : aAmpoule ? 90f : purity ? 60f : -1f;

                // 4 = vanilla Shiny Stone
                int turboRegenPower = shadeRegen || cFreeze || purity ? 4 : honeyDewWorking || aAmpoule ? 3 : photosynthesis ? 1 : -1;

                if (turboRegenPower > 0)
                {
                    // After a brief delay determined by your form of standing still regen, min-cap life regen time at 1800 / 3600.
                    // Photosynthesis Potion does not do this at night.
                    if (Player.lifeRegenTime > regenTimeNeededForTurboRegen && Player.lifeRegenTime < 1800f && !onlyPhotosynthesisAtNight)
                        Player.lifeRegenTime = 1800f;

                    Player.lifeRegen += turboRegenPower;
                    Player.lifeRegenTime += turboRegenPower;
                }

            }
            #endregion

            // The Camper counteracts the regen loss while moving horizontally
            if (camper && (Player.velocity.X != 0 && Player.grappling[0] <= 0))
            {
                // Vanilla base regen rate which gets boosted when resting
                // The first 6 boosts increment every 300 frames, up to 6 at 1800
                // Then, the last 3 boosts increment every 600 frames, up to 9 at 3600 which is the cap
                int baseRegenRate = (int)(Math.Clamp(Player.lifeRegenTime / 300f, 0f, 6f) + Math.Clamp((Player.lifeRegenTime - 1800f) / 600f, 0f, 3f));
                // Normally 1.25 while resting and 0.5 while not
                Player.lifeRegen += (int)(baseRegenRate * 0.75f);

                if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool())
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
