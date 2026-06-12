using System;
using System.Linq;
using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.Potions;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.DataStructures;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Fearmonger;
using CalamityMod.Items.Armor.Reaver;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
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

                if (Player.poisoned)
                    totalVanillaDoT += 4;

                if (Player.onFire)
                    totalVanillaDoT += 8;

                if (Player.tongued)
                    totalVanillaDoT += 100;

                if (Player.venom)
                    totalVanillaDoT += 12;

                if (Player.onFrostBurn)
                    totalVanillaDoT += 12;

                if (Player.onFire2)
                    totalVanillaDoT += 12;

                if (Player.burned)
                    totalVanillaDoT += 60;

                if (Player.suffocating)
                    totalVanillaDoT += 40;

                if (Player.electrified)
                {
                    totalVanillaDoT += eleResist ? 4 : 8;
                    if (Player.controlLeft || Player.controlRight)
                        totalVanillaDoT += eleResist ? 16 : 32;
                }

                // Tally up total current vanilla DoT so it can be added as extra DoT from Death Mode
                totalNegativeLifeRegen += totalVanillaDoT * deathNegativeRegenBonus;
            }

            //
            // Calamity debuffs (Vanilla Shadowflame and Daybroken are added here)
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
            ApplyDoTDebuff(whisperingDeath, 0, laudanum);

            ApplyDoTDebuff(irradiated, 4);
            ApplyDoTDebuff(windChilled, 4);
            int sulphurDoT = 6 - (sulphurSet ? 2 : 0) - (sulphurskin ? 2 : 0) - (corrosiveSpine ? 2 : 0);
            ApplyDoTDebuff(sulphurPoison, sulphurDoT);
            ApplyDoTDebuff(riptide, 6);
            ApplyDoTDebuff(weakBrimstoneFlames, 7);
            ApplyDoTDebuff(burningBlood, 8);
            ApplyDoTDebuff(brainRot, 8);
            ApplyDoTDebuff(vaporfied, 8);
            int staticDoT = ((Player.controlLeft || Player.controlRight) ? 12 : 3) / (eleResist ? 2 : 1);
            ApplyDoTDebuff(staticDischarge, staticDoT);
            ApplyDoTDebuff(heavybleeding, 16);
            ApplyDoTDebuff(crushDepth, 18);
            ApplyDoTDebuff(astralInfection, 24, hideOfDeus);
            ApplyDoTDebuff(shadowflame, 30);
            ApplyDoTDebuff(brimstoneFlames, 30);
            ApplyDoTDebuff(plague, (int)MathF.Round(30 * (alchFlask ? (1f - AlchemicalDecanter.PlagueReduction) : 1f)));
            ApplyDoTDebuff(vHex, 30); // Has other effects
            ApplyDoTDebuff(searingLava, 30);
            ApplyDoTDebuff(demonicFlames, 33); // Never inflicted on the player
            ApplyDoTDebuff(laceration, 36);
            ApplyDoTDebuff(daybroken, 40);
            ApplyDoTDebuff(bane, 40);
            ApplyDoTDebuff(nightwither, 40);
            ApplyDoTDebuff(holyFlames, 40);
            ApplyDoTDebuff(voidfrost, 40);
            ApplyDoTDebuff(hadopelagicPressure, 40);

            // Profaned Soul Crystal turns you into Providence, a God, and you take more damage from God Slayer Inferno
            ApplyDoTDebuff(godSlayerInferno, profanedCrystalBuffs ? 50 : 40);
            int fluxDoT = ((Player.controlLeft || Player.controlRight) ? 50 : 10) / (eleResist ? 2 : 1);
            ApplyDoTDebuff(vermillionFlux, fluxDoT);
            ApplyDoTDebuff(elementalMix, 50); // Never inflicted on the player
            ApplyDoTDebuff(trueVHex, 50);
            int dragonfireDoT = ((Player.name == "JFL" || Player.name == "MrJFL") ? 200 : 50) / (dynamoStemCells ? 2 : 1);
            ApplyDoTDebuff(dragonFire, dragonfireDoT);
            ApplyDoTDebuff(miracleBlight, 60);
            ApplyDoTDebuff(banishingFire, 60); // Never inflicted on the player
            int rebukeDoT = ((Player.controlLeft || Player.controlRight) ? 80 : 16) / (eleResist ? 2 : 1);
            ApplyDoTDebuff(auricRebuke, rebukeDoT);

            // Slowly increase the sulphuric water poisoning effect. Once it's high enough, the player takes damage and the meter resets.
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
                float scoogDistance = Vector2.Distance(Player.Center, AS.Center);
                // GFB Aquatic Scourge poisons you if:
                // 1. You are over 50 blocks away from the head
                // 2. You are under 250 blocks away from the head (so that people halfway across the world aren't getting killed for no reason)
                // 3. Aquatic Scourge has been damaged
                if (AS.life < AS.lifeMax && scoogDistance < 4000f)
                    ASPoisonLevel = Utils.GetLerpValue(800f, 1600f, scoogDistance, true);
            }

            bool ASPoisoning = ASPoisonLevel > 0f;
            if (ASPoisoning || ((ZoneSulphur || ZoneAbyssLayer1) && !Player.creativeGodMode && Player.IsUnderwater() && !decayEffigy && !abyssalDivingSuit && !Player.lavaWet && !Player.honeyWet && !nearSafeZone))
            {
                float increment = 1f / SulphSeaWaterSafetyTime;
                //No way to mitigate AS Poisoning
                if (ASPoisoning)
                    increment *= 3f + (6f * ASPoisonLevel);
                if (sulphurskin && !ASPoisoning)
                    increment *= 0.5f;
                if (sulphurSet && !ASPoisoning)
                    increment *= 0.5f;
                if (corrosiveSpine && !ASPoisoning)
                    increment *= 0.5f;
                if (ZoneAbyssLayer1 && !ASPoisoning)
                    increment *= 0.33f;

                SulphWaterPoisoningLevel = MathHelper.Clamp(SulphWaterPoisoningLevel + increment, 0f, 1f);
                if (SulphWaterPoisoningLevel >= 1f)
                {
                    SulphWaterPoisoningLevel = 0f;
                    Player.Hurt(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.SulphurMeter").ToNetworkText(Player.name)), Math.Min(Player.statLifeMax2 / 4, 150), 0);
                }
            }
            else
                SulphWaterPoisoningLevel = MathHelper.Clamp(SulphWaterPoisoningLevel - 1f / SulphSeaWaterRecoveryTime, 0f, 1f);
            #endregion

            #region Alcohol
            var dripPlayer = Player.GetModPlayer<IVDripPlayer>();

            for (int l = 0; l < Player.MaxBuffs; l++)
            {
                int buff = Player.buffType[l];
                if (buff <= 0) continue;

                var data = CalamityBuffSets.DebuffDataset[buff];
                if (data?.AlcoholLevel > 0)
                    alcoholPoisonLevel += data.AlcoholLevel;
            }

            if (dripPlayer.ivDripEquipped && dripPlayer.currentAlcohol != AlcoholType.None)
            {
                int ivBuffID = CalamityBuffSets.GetBuffIDFromAlcoholType(dripPlayer.currentAlcohol);

                if (ivBuffID != -1)
                {
                    var ivData = CalamityBuffSets.DebuffDataset[ivBuffID];
                    if (ivData?.AlcoholLevel > 0)
                        alcoholPoisonLevel += ivData.AlcoholLevel;
                }
            }

            if (everclear)
                totalNegativeLifeRegen += Everclear.RegenLoss;
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.Everclear))
                totalNegativeLifeRegen += Everclear.RegenLoss;
            // Blanket effect for all alcohols
            if (alcoholPoisonLevel > 0)
            {
                // This applies the tipsy eyes effect
                Player.tipsy = true;

                // This one is checked through a buff so we have to counter that
                if (!Player.HasBuff(BuffID.Tipsy))
                    Player.fishingSkill += 5;

            }
            if (alcoholPoisonLevel > alcoholPoisonMax)
            {
                // Independently of Calamity's nerfs to Nebula life regen, it is disabled entirely by alcohol poisoning.
                Player.nebulaLevelLife = 0;

                if (Player.whoAmI == Main.myPlayer)
                    Player.AddBuff(ModContent.BuffType<AlcoholPoisoning>(), 2, false);

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;
                totalNegativeLifeRegen += 3 * alcoholPoisonLevel;
            }
            #endregion

            if (brimflameFrenzy)
            {
                Player.manaRegen = 0;
                Player.manaRegenBonus = 0;
                Player.manaRegenDelay = (int)Player.maxRegenDelay;
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

            if (Player.statMana < 0 && Player.Calamity().ChaosStone)
            {
                totalNegativeLifeRegen -= Player.statMana/100f * Items.Accessories.ChaosStone.LostRegenPer100Mana;
            }

            //
            // ACTUALLY APPLY NEGATIVE LIFE REGEN
            //

            // At the last second, Reaver defense helm reduces DoT debuffs by 20%
            if (reaverDefense)
                totalNegativeLifeRegen -= (int)(totalNegativeLifeRegen * ReaverHeadTank.SetBonusDebuffDamageReduction);

            if (tequilaSunrise)
                totalNegativeLifeRegen = (int)(totalNegativeLifeRegen * TequilaSunrise.DoTMultiplier);
            if (Player.GetModPlayer<IVDripPlayer>().HasAlcohol(AlcoholType.TequilaSunrise))
                totalNegativeLifeRegen = (int)(totalNegativeLifeRegen * TequilaSunrise.DoTMultiplier);

            Player.lifeRegen -= (int)totalNegativeLifeRegen;

            bool hasLifeRegenHinderingDebuff = Player.lifeRegenTime == 0;


            #region Life Regen That Works Even During DoT Debuffs

            //Negation of debuffs. These all will not take life regen above 0
            if (Player.lifeRegen < 0)
            {
                if (crownJewel)
                    Player.lifeRegen += CrownJewel.ReducedDoTAmount;

                if (infectedJewel)
                    Player.lifeRegen += InfectedJewel.ReducedDoTAmount;

                if (purity)
                    Player.lifeRegen += Radiance.ReducedDoTAmount;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
            }

            if (divineBless)
            {
                if (Player.whoAmI == Main.myPlayer && Player.miscCounter % AngelicAlliance.DivineBlessFramesPerHeal == 0) // Flat 4 health per second
                {
                    if (!noLifeRegen)
                        Player.HealPlayer(1, HealTextType.None);
                }
            }

            if (bloodfinBoost)
            {
                if (bloodfinTimer > 0)
                    bloodfinTimer--;

                if (Player.whoAmI == Main.myPlayer && bloodfinTimer <= 0)
                {
                    bloodfinTimer = Bloodfin.FramesForExtraRegen;

                    if (!noLifeRegen)
                        Player.HealPlayer(1, HealTextType.None);
                }
            }

            // Permafrost's Concoction increases life regen while afflicted with a fire debuff
            if (permafrostsConcoction && Player.buffType.Any(l => CalamityBuffSets.DebuffDataset[l] is not null && CalamityBuffSets.DebuffDataset[l].HeatDebuffScaling > 0))
                Player.lifeRegen += 6;

            if (grandDadHealTimer == 0 && grandDadHealPool >= 7)
            {
                Player.HealPlayer(7, HealTextType.Broadcast);
                grandDadHealPool -= 7;
                grandDadHealTimer = (int)(30 * Utils.GetLerpValue(260, 60, grandDadHealPool, true));
            }
            else if (grandDadHealTimer > 0)
                grandDadHealTimer--;

            //Raising natural regen post-damaging-debuff
            if (hadLifeRegenHinderingDebuff && !hasLifeRegenHinderingDebuff)
            {
                if (infectedJewel)
                    Player.lifeRegenTime += InfectedJewel.PostDebuffRegenTimeBoost;

                if (purity)
                    Player.lifeRegenTime += Radiance.PostDebuffRegenTimeBoost;

                if (tequilaSunrise)
                    Player.lifeRegenTime += 1800;

            }

            if (dripPlayer.HasAlcohol(AlcoholType.TequilaSunrise))
            {
                if (hadLifeRegenHinderingDebuff && !hasLifeRegenHinderingDebuff)
                {
                    Player.lifeRegenTime += 1800;
                }
            }


            #endregion

            // During Silva revive or God Slayer dash, all negative life regen is canceled
            if ((silvaCountdown > 0 && hasSilvaEffect && silvaSet) || (LastUsedDashID == GodslayerArmorDash.ID && Player.dashDelay < 0))
            {
                if (Player.lifeRegen < 0)
                    Player.lifeRegen = 0;
            }

            #region Things That Disable Even That Life Regen
            //
            // Yes, really, there's a list of conditions under which life regen doesn't work
            // even if it's life regen that normally works during a damage over time debuff.
            //
            // 1. No life regen bool (Omega Blue armor)
            // 2. Being too far from Providence cocoon ("Holy Inferno")
            // 3. Air drowning in the Abyss
            //

            // All forms of overtly disabling life regeneration disable Nebula Life boosters as well.

            if (noLifeRegen)
            {
                Player.nebulaLevelLife = 0;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;

                Player.lifeRegenTime = 0;

                if (Player.lifeRegenCount > 0)
                    Player.lifeRegenCount = 0;
            }

            if (holyInferno)
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
            #endregion

            // Chalice of the Blood God bleedout
            // The bleedout is applied by directly reducing the player's health. It is not canceled by anything.
            ChaliceOfTheBloodGod.HandleBleedout(Player);

            //Finally, update the state of hadLifeRegenHinderingDebuff for next frame. Needs to check for 2 frames of 0 regen time to be sure they had a debuff
            hadLifeRegenHinderingDebuff = hasLifeRegenHinderingDebuff && _hasNoNaturalRegen;
            _hasNoNaturalRegen = hasLifeRegenHinderingDebuff;
        }
        #endregion

        #region Update Life Regen
        //This method runs BEFORE UpdateBadLifeRegen. Put most effects here. Ran at the end of PostUpdateEquips
        public void GeneralLifeRegen()
        {
            // Apply partial life regeneration
            // Regenerator applies partial life regen as a part of its damage conversion straight up, so it will avoid this function
            if (regenerator)
                partialLifeRegenCounter = 0f;
            else
            {
                partialLifeRegenCounter += partialLifeRegen;
                if (partialLifeRegenCounter >= 1f || partialLifeRegenCounter <= -1f)
                {
                    int flooredRegenAdded = (int)MathF.Floor(partialLifeRegenCounter);
                    Player.lifeRegen += flooredRegenAdded;
                    partialLifeRegenCounter -= flooredRegenAdded;
                }
            }

            float lifeRatio = (Player.statLife) / (float)Player.statLifeMax2;

            if (mushy)
                Player.lifeRegen += Mushy.RegenBoost;

            if (permafrostsConcoction)
            {
                if (lifeRatio <= 0.5f)
                    Player.lifeRegen++;
                if (lifeRatio <= 0.25f)
                    Player.lifeRegen++;
                if (lifeRatio <= 0.1f)
                    Player.lifeRegen += 2;
            }

            if (tRegen)
                Player.lifeRegen += TarragonHeadMelee.TarraLifeRegenBoost;

            if (sRegen)
                Player.lifeRegen += SpiritGlyph.RegenBoost;

            if (PinkJellyRegen)
                Player.lifeRegen += LifeJelly.AuraRegenBoost;

            if (rOoze)
                Player.lifeRegen += (int)Math.Round(MathHelper.Lerp(RadiantOoze.MaxRegenBoost, RadiantOoze.MinRegenBoost, lifeRatio));

            if (livingDew)
                Player.lifeRegenTime += LivingDew.RegenTimeBoost;

            if (aAmpoule)
            {
                Player.lifeRegen += (int)Math.Round(MathHelper.Lerp(AmbrosialAmpoule.MaxRegenBoost, AmbrosialAmpoule.MinRegenBoost, lifeRatio));
                Player.lifeRegenTime += AmbrosialAmpoule.RegenTimeBoost;
            }

            if (purity)
            {
                Player.lifeRegen += (int)Math.Round(MathHelper.Lerp(Radiance.MaxRegenBoost, Radiance.MinRegenBoost, lifeRatio));
                Player.lifeRegenTime += Radiance.RegenTimeBoost;
            }


            if (GreenJellyRegen)
                Player.lifeRegen += Items.Accessories.GrandGelatin.AuraRegenBoost;

            if (AbsorberRegen)
                Player.lifeRegen += TheAbsorber.AuraRegenBoost;

            if (hallowedRegen)
                Player.lifeRegen += HallowedRune.RegenBoost;

            if (affliction || afflicted)
                Player.lifeRegen += Affliction.RegenBoost;

            if (trinketOfChi || chiRegen)
                Player.lifeRegen += TrinketofChi.RegenBoost;

            if (darkSunRing)
            {
                if (Main.eclipse || Main.dayTime)
                    Player.lifeRegen += Main.eclipse ? 2 : 4;
            }

            if (silvaSet)
                Player.lifeRegen += SilvaArmor.SetBonusRegenBoost;

            if (angelicAlliance)
            {
                if (Player.wingTime < Player.wingTimeMax)
                    Player.lifeRegen += AngelicAlliance.RegenBoostDuringFlight;
            }

            if (phantomicHeartRegen > 0 && phantomicHeartRegen < 1000)
            {
                Player.lifeRegen += PhantomicArtifact.RegenBoost;
                if (Main.rand.NextBool())
                {
                    Dust regen = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.Blood, 0f, 0f, 200, new Color(99, 54, 84), 2f);
                    regen.noGravity = true;
                    regen.fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    regen.velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    regen.position = Player.Center - velocity;
                }
            }
            if (fearmongerSet && fearmongerRegenFrames > 0)
            {
                if (Player.lifeRegenTime < FearmongerGreathelm.MinionRegenTimeFloor)
                    Player.lifeRegenTime = FearmongerGreathelm.MinionRegenTimeFloor;

                Player.lifeRegen += FearmongerGreathelm.MinionRegenBoost;
                Player.lifeRegenTime += FearmongerGreathelm.MinionRegenTimeBoost;
            }
            if (handWarmer && eskimoSet)
            {
                Player.lifeRegen += 2;
            }
        }

        //This method runs AFTER UpdateBadLifeRegen. Only put effects that should bypass debuff DoT canceling here.
        public override void UpdateLifeRegen()
        {
            if (avertorBonus)
            {
                Player.lifeRegen += 4;
            }

            if (community)
            {
                int regenBoost = 1 + (int)(TheCommunity.CalculatePower() * TheCommunity.RegenMultiplier);
                if (Player.lifeRegen < 0)
                    Player.lifeRegen += regenBoost;
            }

            if (manaOverloader)
            {
                float manaRatio = Player.statMana / (float)Player.statManaMax2;
                Player.lifeRegen += (int)(MathF.Round(MathHelper.Lerp(4f, -4f, manaRatio)) * (Player.HasBuff(BuffID.ManaSickness) ? 0.5f : 1f));
            }

            if (pinkCandle && !noLifeRegen)
            {
                // Every frame, add up 1/60th of the healing value (0.4% max HP per second)
                pinkCandleHealFraction += Player.statLifeMax2 * VigorousCandle.PercentHealthPerSecond / 60;

                if (pinkCandleHealFraction >= 1D)
                {
                    pinkCandleHealFraction = 0D;
                    Player.HealPlayer(1, HealTextType.None);
                }
            }
            else
                pinkCandleHealFraction = 0D;

            #region Standing Still Life Regen
            // Standing still healing bonuses (all are exclusive with vanilla Shiny Stone, but all function similarly)
            if (!Player.shinyStone && Player.StandingStill() && Player.velocity.Y == 0 && Player.itemAnimation == 0 && (shadeRegen || cFreeze))
            {
                // Divides all negative life regen by two before applying any other effects.
                if (Player.lifeRegen < 0)
                    Player.lifeRegen /= 2;

                // Spawn dust of some flavor while actually regenerating
                if (Player.lifeRegen > 0 && Player.statLife < actualMaxLife)
                {
                    int dustType = shadeRegen ? 173 : 67;
                    bool dustSpawnRolled = Main.rand.Next(30000) < Player.lifeRegenTime ? Main.rand.NextBool() : Main.rand.NextBool(30);
                    if (dustType != -1 && dustSpawnRolled)
                    {
                        Dust regen = Dust.NewDustDirect(Player.position, Player.width, Player.height, dustType, 0f, 0f, 200, default, 1f);
                        regen.noGravity = true;
                        regen.fadeIn = 1.3f;
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                        regen.velocity = velocity;
                        velocity.Normalize();
                        regen.position = Player.Center - velocity;
                    }
                }

                // Actually apply "standing still" regeneration (the stats are granted even at full health)
                float regenTimeNeededForTurboRegen = shadeRegen ? 40f : 60f;

                // After a brief delay determined by your form of standing still regen, min-cap life regen time at 900 / 3600.
                if (Player.lifeRegenTime > regenTimeNeededForTurboRegen && Player.lifeRegenTime < 900f)
                    Player.lifeRegenTime = 900f;

                Player.lifeRegen += 4;
                Player.lifeRegenTime += 4;

            }
            #endregion

            if (regenerator) // Gives special regen of it's own, but disables all regular life regen
            {
                if (Player.miscCounter % Regenerator.FramesPerHeal == 0 && Player.statLife < (int)(Player.statLifeMax2 * 0.5f))
                    Player.HealPlayer(1, HealTextType.None);

                // Boost life regen time quite a bit.
                // This is so that in events and such where small hits are common, your damage boost isn't completley negated
                if (Player.lifeRegenTime < 3600)
                    Player.lifeRegenTime += Regenerator.RegenTimeBoost;
            }
            else
                regeneratorDamage = 0;

            if (toxicHeart) // Since it needs to know your life regen, it must be placed here
            {
                float minLifeRegen = -20; // Fastest rate
                float maxLifeRegen = 15; // Slowest rate
                int auraDamage = (int)Player.GetBestClassDamage().ApplyTo(200);
                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<ToxicHeart>()));
                float lifeRegenRate = Utils.Remap(Player.lifeRegen, minLifeRegen, maxLifeRegen, 20, 1, true);

                if (pulseRate < lifeRegenRate) // Jump to fastest pulse rate and slowly slow down if life regen increases
                    pulseRate = lifeRegenRate;
                else
                    pulseRate = MathHelper.Lerp(pulseRate, lifeRegenRate, 0.002f);

                if (pulseCounter >= 420)
                {
                    Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<PlaguePulse>(), auraDamage, 0f, Player.whoAmI, 0, 0, 0);
                    pulseCounter = 0;
                    if (toxicHeartVisuals)
                    {
                        float soundVolume = Utils.Remap(Player.lifeRegen, minLifeRegen, maxLifeRegen, 1f, 0.3f, true);
                        SoundStyle heartbeat = new("CalamityMod/Sounds/Item/Heartbeat");
                        SoundEngine.PlaySound(heartbeat with { Volume = soundVolume, PitchVariance = 0.2f }, Player.Center);
                    }
                }
                else
                {
                    pulseCounter += MathHelper.Clamp(pulseRate, 1, 20);
                }
            }
        }
        #endregion

        public override void NaturalLifeRegen(ref float regen)
        {
            // Honey Dew and its upgrades make natural regen more powerful
            if (purity)
                regen *= Radiance.NaturalRegenPower;
            else if (aAmpoule)
                regen *= AmbrosialAmpoule.NaturalRegenPower;
            else if (livingDew)
                regen *= LivingDew.NaturalRegenPower;
            else if (honeyDew)
                regen *= HoneyDew.NaturalRegenPower;

            // The Camper counteracts the regen loss while moving horizontally
            if (camper && (Player.velocity.X != 0 && Player.grappling[0] <= 0))
            {
                // Normally 1.25 while resting and 0.5 while not so we apply this cancelling multiplier
                regen *= 2.5f;

                if (Main.rand.Next(30000) < Player.lifeRegenTime || Main.rand.NextBool())
                {
                    Dust heart = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.HeartCrystal, 0f, 0f, 200, Color.OrangeRed, 1f);
                    heart.noGravity = true;
                    heart.fadeIn = 1.3f;
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 50f, 100f, 0.04f);
                    heart.velocity = velocity;
                    velocity.Normalize();
                    velocity *= 34f;
                    heart.position = Player.Center - velocity;
                }
            }

            // Regenerator trades all positive regen for damage, and caps your health gain at 50%
            if (regenerator)
            {
                float finalRegen = Player.lifeRegen + partialLifeRegen + MathF.Round(regen * (Player.statLifeMax2 / 400f * 0.85f + 0.15f));
                finalRegen = MathF.Max(finalRegen, 0f);

                // Rapid Healing increments RegenCount directly so it needs to be manually added
                // It also works while debuffs are active so the same logic applies here
                if (Player.palladiumRegen)
                    finalRegen += 4f;

                regeneratorDamage = finalRegen * Regenerator.RegenToDamageRatio;
                Player.GetDamage<GenericDamageClass>() += regeneratorDamage;

                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                if (regen > 0f)
                    regen = 0f;
                if (Player.lifeRegenCount > 0)
                    Player.lifeRegenCount = 0;

                //Hard-lock the player's health to a certain ratio.
                //No lifesteal, no regen, no healing pots
                if (Player.statLife >= (int)(Player.statLifeMax2 * Regenerator.HealthRatioCap))
                {
                    Player.statLife = (int)(Player.statLifeMax2 * Regenerator.HealthRatioCap);
                    Player.moonLeech = true;
                    healingPotionMultiplier = 0;
                }
            }
        }
    }
}
