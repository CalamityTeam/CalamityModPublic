using System;
using System.Linq;
using CalamityMod.Balancing;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Placeables;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Enums;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor.Aerospec;
using CalamityMod.Items.Armor.Demonshade;
using CalamityMod.Items.Armor.LunicCorps;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Wulfrum;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Other;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Dodges
        private void SpectralVeilDodge()
        {
            // 17APR2024: Ozzatron: Spectral Veil counts as a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
            int spectralVeilIFrames = spectralVeilImmunity + (Player.longInvince ? BalancingConstants.CrossNecklaceIFrameBoost : 0);
            Player.GiveUniversalIFrames(spectralVeilIFrames, true);
            rogueStealth = rogueStealthMax;
            spectralVeilImmunity = 0;

            Vector2 sVeilDustDir = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            sVeilDustDir.Normalize();
            sVeilDustDir *= 0.5f;

            for (int j = 0; j < 20; j++)
            {
                int sVeilDustIndex1 = Dust.NewDust(Player.Center, 1, 1, DustID.VilePowder, sVeilDustDir.X * j, sVeilDustDir.Y * j);
                int sVeilDustIndex2 = Dust.NewDust(Player.Center, 1, 1, DustID.VilePowder, -sVeilDustDir.X * j, -sVeilDustDir.Y * j);
                Main.dust[sVeilDustIndex1].noGravity = false;
                Main.dust[sVeilDustIndex1].noLight = false;
                Main.dust[sVeilDustIndex2].noGravity = false;
                Main.dust[sVeilDustIndex2].noLight = false;
            }

            SoundEngine.PlaySound(SilvaHeadSummon.DispelSound, Player.Center);

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        private void GodSlayerDodge()
        {
            // 17APR2024: Ozzatron: God Slayer Dodge is a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
            int godSlayerDodgeIFrames = Player.ComputeDodgeIFrames();
            Player.GiveUniversalIFrames(godSlayerDodgeIFrames, true);

            SoundEngine.PlaySound(SoundID.Item67, Player.Center);

            for (int j = 0; j < 30; j++)
            {
                int num = Dust.NewDust(Player.position, Player.width, Player.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[num];
                dust.position.X += Main.rand.Next(-20, 21);
                dust.position.Y += Main.rand.Next(-20, 21);
                dust.velocity *= 0.4f;
                dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(Player.ArmorSetDye(), Player);
                if (Main.rand.NextBool())
                {
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.noGravity = true;
                }
            }

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        private void CounterScarfDodge()
        {
            if (evasionScarf)
            {
                int duration = CalamityUtils.SecondsToFrames(30);
                Player.AddCooldown(Cooldowns.EvasionScarf.ID, duration);
            }
            else
            {
                int duration = CalamityUtils.SecondsToFrames(30);
                Player.AddCooldown(Cooldowns.CounterScarf.ID, duration);
            }

            // 17APR2024: Ozzatron: Counter Scarf is a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
            int counterScarfIFrames = Player.ComputeDodgeIFrames();
            Player.GiveUniversalIFrames(counterScarfIFrames, true);

            for (int j = 0; j < 100; j++)
            {
                int scarfDodgeDust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain, 0f, 0f, 100, default, 2f);
                Dust dust = Main.dust[scarfDodgeDust];
                dust.position.X += Main.rand.Next(-20, 21);
                dust.position.Y += Main.rand.Next(-20, 21);
                dust.velocity *= 0.4f;
                dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cNeck, Player);
                if (Main.rand.NextBool())
                {
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dust.noGravity = true;
                }
            }

            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
        }

        public void AbyssMirrorDodge(double dodgeDamageGateValuePercent, int dodgeDamageGateValue, int hitDamage)
        {
            double maxCooldownDurationDamagePercent = 0.5;
            int maxCooldownDurationDamageValue = (int)Math.Round(Player.statLifeMax2 * (maxCooldownDurationDamagePercent - dodgeDamageGateValuePercent));

            // Just in case...
            if (maxCooldownDurationDamageValue <= 0)
                maxCooldownDurationDamageValue = 1;

            float cooldownDurationScalar = MathHelper.Clamp((hitDamage - dodgeDamageGateValue) / (float)maxCooldownDurationDamageValue, 0f, 1f);

            if (Player.whoAmI == Main.myPlayer && abyssalMirror && !eclipseMirror)
            {
                int cooldownDuration = (int)MathHelper.Lerp(BalancingConstants.MirrorDodgeCooldownMin, BalancingConstants.MirrorDodgeCooldownMax, cooldownDurationScalar);
                Player.AddCooldown(GlobalDodge.ID, cooldownDuration, true, "abyssmirror");

                // 17APR2024: Ozzatron: Abyssal Mirror is a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
                int abyssalMirrorDodgeIFrames = Player.ComputeDodgeIFrames();
                Player.GiveUniversalIFrames(abyssalMirrorDodgeIFrames, true);

                rogueStealth += 0.5f;
                SoundEngine.PlaySound(SilvaHeadSummon.ActivationSound, Player.Center);

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<AbyssalMirror>()));
                for (int i = 0; i < 10; i++)
                {
                    int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(55);
                    damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                    int lumenyl = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), ModContent.ProjectileType<AbyssalMirrorProjectile>(), damage, 0, Player.whoAmI);
                    Main.projectile[lumenyl].rotation = Main.rand.NextFloat(0, 360);
                    Main.projectile[lumenyl].frame = Main.rand.Next(0, 4);
                    if (lumenyl.WithinBounds(Main.maxProjectiles))
                        Main.projectile[lumenyl].DamageType = DamageClass.Generic;
                }

                // TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
                if (Player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }

        public void EclipseMirrorDodge(double dodgeDamageGateValuePercent, int dodgeDamageGateValue, int hitDamage)
        {
            double maxCooldownDurationDamagePercent = 0.5;
            int maxCooldownDurationDamageValue = (int)Math.Round(Player.statLifeMax2 * (maxCooldownDurationDamagePercent - dodgeDamageGateValuePercent));

            // Just in case...
            if (maxCooldownDurationDamageValue <= 0)
                maxCooldownDurationDamageValue = 1;

            float cooldownDurationScalar = MathHelper.Clamp((hitDamage - dodgeDamageGateValue) / (float)maxCooldownDurationDamageValue, 0f, 1f);

            if (Player.whoAmI == Main.myPlayer && eclipseMirror)
            {
                int cooldownDuration = (int)MathHelper.Lerp(BalancingConstants.MirrorDodgeCooldownMin, BalancingConstants.MirrorDodgeCooldownMax, cooldownDurationScalar);
                Player.AddCooldown(GlobalDodge.ID, cooldownDuration, true, "eclipsemirror");

                // 17APR2024: Ozzatron: Eclipse Mirror is a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
                int eclipseMirrorDodgeIFrames = Player.ComputeDodgeIFrames();
                Player.GiveUniversalIFrames(eclipseMirrorDodgeIFrames, true);

                rogueStealth += 0.5f;
                SoundEngine.PlaySound(SoundID.Item68, Player.Center);

                var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<EclipseMirror>()));
                int damage = (int)Player.GetTotalDamage<RogueDamageClass>().ApplyTo(2750);
                damage = Player.ApplyArmorAccDamageBonusesTo(damage);

                int eclipse = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<EclipseMirrorBurst>(), damage, 0, Player.whoAmI);
                if (eclipse.WithinBounds(Main.maxProjectiles))
                    Main.projectile[eclipse].DamageType = DamageClass.Generic;

                // TODO -- Calamity dodges should probably not send a vanilla dodge packet considering that causes Tabi dust
                if (Player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }
        #endregion

        #region Pre Kill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            PopupGUIManager.SuspendAll();

            if (andromedaState == AndromedaPlayerState.LargeRobot)
            {
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Player.Center + Utils.NextVector2Circular(Main.rand, 60f, 90f), 133);
                        dust.velocity = Utils.NextVector2Circular(Main.rand, 4f, 4f);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(1.2f, 1.35f);
                    }

                    for (int i = 0; i < 3; i++)
                        Utils.PoofOfSmoke(Player.Center + Utils.NextVector2Circular(Main.rand, 20f, 30f));
                }
            }

            if (hInferno)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == ModContent.NPCType<Providence>())
                        n.active = false;
                }
            }

            if (nCore && !Player.HasCooldown(Cooldowns.NebulousCore.ID))
            {
                SoundEngine.PlaySound(SoundID.Item67, Player.Center);

                for (int j = 0; j < 50; j++)
                {
                    int nebulousReviveDust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.ShadowbeamStaff, 0f, 0f, 100, default, 2f);
                    Dust dust = Main.dust[nebulousReviveDust];
                    dust.position.X += Main.rand.Next(-20, 21);
                    dust.position.Y += Main.rand.Next(-20, 21);
                    dust.velocity *= 0.9f;
                    dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    // Change this accordingly if we have a proper equipped sprite.
                    dust.shader = GameShaders.Armor.GetSecondaryShader(Player.cBody, Player);
                    if (Main.rand.NextBool())
                        dust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                }

                // Nebulous Core clears Chalice of the Blood God's bleedout buffer
                if (chaliceOfTheBloodGod)
                {
                    chaliceBleedoutBuffer = 0D;
                    chaliceDamagePointPartialProgress = 0D;
                }
                Player.HealPlayer(100);

                Player.AddCooldown(Cooldowns.NebulousCore.ID, CalamityUtils.SecondsToFrames(90));
                return false;
            }

            if (DashID == GodslayerArmorDash.ID && Player.dashDelay < 0)
            {
                if (Player.statLife < 1)
                    Player.statLife = 1;

                return false;
            }

            if (silvaSet && silvaCountdown > 0)
            {
                if (silvaCountdown == silvaReviveDuration && !hasSilvaEffect)
                {
                    SoundEngine.PlaySound(SilvaHeadSummon.ActivationSound, Player.Center);

                    Player.AddBuff(ModContent.BuffType<SilvaRevival>(), silvaReviveDuration);

                    if (silvaWings)
                    {
                        Player.HealPlayer(Player.statLifeMax2 / 2);
                    }
                }

                hasSilvaEffect = true;

                if (Player.statLife < 1)
                    Player.statLife = 1;

                // Silva revive clears Chalice of the Blood God's bleedout buffer every frame while active
                // Can we please remove this from the game
                if (chaliceOfTheBloodGod)
                {
                    chaliceBleedoutBuffer = 0D;
                    chaliceDamagePointPartialProgress = 0D;
                }

                return false;
            }

            if (necroSet && necroReviveCounter == -1)
            {
                SoundEngine.PlaySound(SoundID.DD2_SkeletonDeath, Player.Center);

                necroReviveCounter = 0; // Start ticking the timer of death
                Player.statLife = Player.statLifeMax2;

                if (Player.statLife < 1)
                    Player.statLife = 1;
                return false;
            }

            if (permafrostsConcoction && !Player.HasCooldown(PermafrostConcoction.ID))
            {
                Player.AddCooldown(PermafrostConcoction.ID, CalamityUtils.SecondsToFrames(180));
                Player.AddBuff(ModContent.BuffType<Encased>(), CalamityUtils.SecondsToFrames(3f));

                Player.statLife = Player.statLifeMax2 * 3 / 10;

                SoundEngine.PlaySound(SoundID.Item92, Player.Center);

                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(Player.position, Player.width, Player.height, DustID.GemSapphire, 0f, 0f, 0, default, 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 5f;
                }

                return false;
            }

            // Custom Death Messages

            if (damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
            {
                if (alcoholPoisoning)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AlcoholBig" + Main.rand.Next(1, 2 + 1)).Format(Player.name));
                }
                if (vHex)
                {
                    // Unique messages appear half the time during each individual stage of SCal's fight
                    string vHexKeyToUse = "Status.Death.VulnerabilityHex" + Main.rand.Next(1, 3 + 1);
                    if (Main.rand.NextBool() && CalamityGlobalNPC.SCal != -1)
                    {
                        if (CalamityGlobalNPC.SCalGrief != -1)
                            vHexKeyToUse = "Status.Death.VulnerabilityHexGrief";
                        else if (CalamityGlobalNPC.SCalLament != -1)
                            vHexKeyToUse = "Status.Death.VulnerabilityHexLament";
                        else if (CalamityGlobalNPC.SCalEpiphany != -1)
                            vHexKeyToUse = "Status.Death.VulnerabilityHexEpiphany";
                        // good luck dying to SCal in Acceptance to see this
                        else if (CalamityGlobalNPC.SCalAcceptance != -1)
                            vHexKeyToUse = "Status.Death.VulnerabilityHexAcceptance";

                        // if none of SCal's phases are detected somehow then it just uses the normal messages all the time
                    }
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText(vHexKeyToUse).Format(Player.name));
                }
                if (ZoneCalamity && Player.lavaWet)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.SearingLava" + Main.rand.Next(1, 2 + 1)).Format(Player.name));
                }
                if (gsInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.GodSlayerInferno" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (sulphurPoison)
                {
                    if (!Main.rand.NextBool(4)) // 75% custom
                        damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.SulphuricPoisoning" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                    else
                        damageSource = PlayerDeathReason.ByOther(9); // 25% generic Poisoned death text
                }
                if (dragonFire)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Dragonfire" + Main.rand.Next(1, 4 + 1)).Format(Player.name));
                }
                if (miracleBlight)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.MiracleBlight" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (hInferno)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.HolyInferno").Format(Player.name));
                }
                if (hFlames || banishingFire)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.HolyFlames" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (shadowflame)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Shadowflame").Format(Player.name));
                }
                if (bBlood)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.BurningBlood" + Main.rand.Next(1, 2 + 1)).Format(Player.name));
                }
                if (brainRot)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.BrainRot" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (elementalMix)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ElementalMix" + Main.rand.Next(1, 2 + 1)).Format(Player.name));
                }
                if (cDepth)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.CrushDepth" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (rTide)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Riptide" + Main.rand.Next(1, 2 + 1)).Format(Player.name));
                }
                if (bFlames || weakBrimstoneFlames)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.BrimstoneFlames" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (pFlames)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Plague" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (astralInfection)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AstralInfection" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
                }
                if (nightwither)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Nightwither").Format(Player.name));
                }
                if (vaporfied)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Vaporfied").Format(Player.name));
                }
                if (manaOverloader || ManaBurn)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ManaBurn").Format(Player.name));
                }
                if (bloodyMary || everclear || evergreenGin || fireball || margarita || moonshine || moscowMule || redWine || screwdriver || starBeamRye || tequila || tequilaSunrise || vodka || whiteWine || Player.tipsy)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AlcoholSmall").Format(Player.name));
                }
                if (witheredDebuff)
                {
                    damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Withered").Format(Player.name));
                }
            }
            if (profanedCrystalBuffs && !profanedCrystalHide)
            {
                damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ProfanedSoulCrystal").Format(Player.name));
            }

            // Leon Death Noise RE4
            if (Main.zenithWorld)
                SoundEngine.PlaySound(LeonDeathNoiseRE4_ForGFB, Player.Center);

            if (NorfleetCounter >= 3 && NorfleetCounter < 1000)
                damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Norfleet").Format(Player.name));
            NorfleetCounter = 0;

            if (NPC.AnyNPCs(ModContent.NPCType<SupremeCalamitas>()))
            {
                if (sCalDeathCount < 51)
                    sCalDeathCount++;
            }

            return true;
        }
        #endregion

        #region Modify Hit NPC
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            // All Calamity multipliers are added together to prevent insane exponential stacking
            float totalDamageMult = 1f;

            // Rippers are always checked for application, because there are ways to get rippers outside of Rev now
            CalamityUtils.ApplyRippersToDamage(this, item.IsTrueMelee(), ref totalDamageMult);

            // Demonshade enrage
            if (enraged)
                totalDamageMult += 1.25f;
            // Withering enchantment when it's draining your HP
            if (witheredDebuff && witheringWeaponEnchant)
                totalDamageMult += 0.6f;

            // Apply all Calamity multipliers as a sum total to TML New Damage in a single step
            modifiers.SourceDamage *= totalDamageMult;

            // 01JUN2024: Ozzatron: apply Yellow Candle "chip damage" as a dirty modifier
            // The registration of the dirty modifier is conditional to ensure it doesn't apply to "near invincible" targets
            //
            // FinalDamage cannot be used for the intended effect because there is no way to access the actual damage of the hit
            CalamityGlobalNPC cgn = target.Calamity();
            if (yellowCandle && cgn.DR < 0.99f && target.takenDamageMultiplier > 0.05f)
                modifiers.ModifyHitInfo += CirrusYellowCandleBuff.ModifyHitInfo_Spite;

            // Excalibur and True Excalibur deal +100% damage to targets above 75% HP.
            if (item.type == ItemID.Excalibur || item.type == ItemID.TrueExcalibur)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    modifiers.ScalingBonusDamage += 1f;
            }

            // Titanium Sword deals up to +15% damage based on the target's knockback resistance.
            if (item.type == ItemID.TitaniumSword)
            {
                float knockbackResistBonus = 0.15f * (1f - target.knockBackResist);
                modifiers.ScalingBonusDamage += knockbackResistBonus;
            }

            // Antlion Claw, Bone Sword and Breaker Blade ignore 50% of the enemy's defense.
            if (item.type == ItemID.AntlionClaw || item.type == ItemID.BoneSword || item.type == ItemID.BreakerBlade)
            {
                modifiers.ScalingArmorPenetration += 0.5f;
            }

            // Stylish Scissors, all Phaseblades and all Phasesabers ignore 100% of the enemy's defense.
            if (item.type == ItemID.StylistKilLaKillScissorsIWish || (item.type >= ItemID.BluePhaseblade && item.type <= ItemID.YellowPhaseblade) || (item.type >= ItemID.BluePhasesaber && item.type <= ItemID.YellowPhasesaber) || item.type == ItemID.OrangePhaseblade || item.type == ItemID.OrangePhasesaber)
            {
                modifiers.ScalingArmorPenetration += 1f;
            }

            // Frost Armor's rework gives +X% melee damage and +Y% ranged damage based on distance, where X+Y = 15.
            if (frostSet)
            {
                // 0f = point blank, 1f = max range or further
                float DistanceInterpolant = Utils.GetLerpValue(FrostArmorSetChange.MinDistance, FrostArmorSetChange.MaxDistance, target.Distance(Main.player[Main.myPlayer].Center), true);

                if (item.CountsAsClass<MeleeDamageClass>())
                {
                    float meleeBoost = MathHelper.Lerp(0f, FrostArmorSetChange.ProximityBoost, 1 - DistanceInterpolant);
                    modifiers.SourceDamage += meleeBoost;
                }
                else if (item.CountsAsClass<RangedDamageClass>())
                {
                    float rangedBoost = MathHelper.Lerp(0f, FrostArmorSetChange.ProximityBoost, DistanceInterpolant);
                    modifiers.SourceDamage += rangedBoost;
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            if (proj.npcProj || proj.trap)
                return;

            // All Calamity multipliers are added together to prevent insane exponential stacking
            float totalDamageMult = 1f;

            // Rippers are always checked for application, because there are ways to get rippers outside of Rev now
            CalamityUtils.ApplyRippersToDamage(this, proj.IsTrueMelee(), ref totalDamageMult);

            // Demonshade enrage
            if (enraged)
                totalDamageMult += 1.25f;
            // Withering enchantment when it's draining your HP
            if (witheredDebuff && witheringWeaponEnchant)
                totalDamageMult += 0.6f;

            // Apply all Calamity multipliers as a sum total to TML New Damage in a single step
            modifiers.SourceDamage *= totalDamageMult;

            // 01JUN2024: Ozzatron: apply Yellow Candle "chip damage" as a dirty modifier
            // The registration of the dirty modifier is conditional to ensure it doesn't apply to "near invincible" targets
            //
            // FinalDamage cannot be used for the intended effect because there is no way to access the actual damage of the hit
            CalamityGlobalNPC cgn = target.Calamity();
            if (yellowCandle && cgn.DR < 0.99f && target.takenDamageMultiplier > 0.05f)
                modifiers.ModifyHitInfo += CirrusYellowCandleBuff.ModifyHitInfo_Spite;

            // Stealth strike damage multipliers are applied here.
            // TODO -- stealth should be its own damage class and this should be applied as player StealthDamage *= XYZ
            if (proj.Calamity().stealthStrike && proj.CountsAsClass<RogueDamageClass>())
                modifiers.SourceDamage *= (float)bonusStealthDamage + 1; // Default bonusStealthDamage is 0, a 1 has to be added to take the damage of the weapon.

            // Screwdriver adds 5% bonus damage to all piercing projectiles.
            if (screwdriver)
            {
                if (proj.penetrate > 1 || proj.penetrate == -1)
                    modifiers.ScalingBonusDamage += 0.05f;
            }

            // Excalibur and True Excalibur deal +100% damage to targets above 75% HP.
            if (proj.type == ProjectileID.Excalibur || proj.type == ProjectileID.TrueExcalibur)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    modifiers.ScalingBonusDamage += 1f;
            }

            // Calamity buffs Inferno Fork by 20%. This is multiplicative because it's supposed to be a buff to the weapon's base damage.
            // However, because the weapon is coded like spaghetti, you have to multiply the explosion's damage too.
            if (proj.type == ProjectileID.InfernoFriendlyBlast)
                modifiers.SourceDamage *= 1.2f;

            // Gungnir deals +100% damage to targets above 75% HP.
            if (proj.type == ProjectileID.Gungnir)
            {
                if (target.life > (int)(target.lifeMax * 0.75))
                    modifiers.ScalingBonusDamage += 1f;
            }

            // Titanium Trident deals up to +15% damage based on the target's knockback resistance.
            if (proj.type == ProjectileID.TitaniumTrident)
            {
                float knockbackResistBonus = 0.15f * (1f - target.knockBackResist);
                modifiers.ScalingBonusDamage += knockbackResistBonus;
            }

            // Frost Armor's rework gives +X% melee damage and +Y% ranged damage based on distance, where X+Y = 15.
            if (frostSet)
            {
                // 0f = point blank, 1f = max range or further
                float DistanceInterpolant = Utils.GetLerpValue(FrostArmorSetChange.MinDistance, FrostArmorSetChange.MaxDistance, target.Distance(Main.player[Main.myPlayer].Center), true);

                if (proj.CountsAsClass<MeleeDamageClass>())
                {
                    float meleeBoost = MathHelper.Lerp(0f, FrostArmorSetChange.ProximityBoost, 1 - DistanceInterpolant);
                    modifiers.SourceDamage += meleeBoost;
                }
                else if (proj.CountsAsClass<RangedDamageClass>())
                {
                    float rangedBoost = MathHelper.Lerp(0f, FrostArmorSetChange.ProximityBoost, DistanceInterpolant);
                    modifiers.SourceDamage += rangedBoost;
                }
            }

            // SUMMONER CROSS CLASS NERF IS APPLIED HERE
            //
            // There are several ways to negate the summoner cross class nerf:
            // - Wearing Forbidden armor and using a magic weapon
            // - Wearing Fearmonger armor
            // - Wearing Gem Tech armor and having the Blue Gem active
            // - Using Profaned Soul Crystal
            // - During the Old One's Army event it's disabled by default
            bool isSummon = proj.CountsAsClass<SummonDamageClass>();
            if (isSummon)
            {
                Item heldItem = Player.ActiveItem();

                bool wearingForbiddenSet = Player.armor[0].type == ItemID.AncientBattleArmorHat && Player.armor[1].type == ItemID.AncientBattleArmorShirt && Player.armor[2].type == ItemID.AncientBattleArmorPants;

                bool forbiddenWithMagicWeapon = wearingForbiddenSet && heldItem.CountsAsClass<MagicDamageClass>();
                bool gemTechBlueGem = GemTechSet && GemTechState.IsBlueGemActive;

                bool crossClassNerfDisabled = forbiddenWithMagicWeapon || fearmongerSet || gemTechBlueGem || profanedCrystalBuffs || DD2Event.Ongoing;
                crossClassNerfDisabled |= CalamityLists.DisabledSummonerNerfMinions.Contains(proj.type);

                // If this projectile is a summon, its owner is holding an item, and the cross class nerf isn't disabled from equipment:
                if (isSummon && heldItem.type > ItemID.None && !crossClassNerfDisabled)
                {
                    bool heldItemIsClassedWeapon = !heldItem.CountsAsClass<SummonDamageClass>() && (
                        heldItem.CountsAsClass<MeleeDamageClass>() ||
                        heldItem.CountsAsClass<RangedDamageClass>() ||
                        heldItem.CountsAsClass<MagicDamageClass>() ||
                        heldItem.CountsAsClass<ThrowingDamageClass>()
                    );

                    bool heldItemIsTool = heldItem.pick > 0 || heldItem.axe > 0 || heldItem.hammer > 0;
                    bool heldItemCanBeUsed = heldItem.useStyle != ItemUseStyleID.None;
                    bool heldItemIsAccessoryOrAmmo = heldItem.accessory || heldItem.ammo != AmmoID.None;
                    bool heldItemIsExcludedByModCall = CalamityLists.DisabledSummonerNerfItems.Contains(heldItem.type);

                    if (heldItemIsClassedWeapon && heldItemCanBeUsed && !heldItemIsTool && !heldItemIsAccessoryOrAmmo && !heldItemIsExcludedByModCall)
                        modifiers.FinalDamage *= BalancingConstants.SummonerCrossClassNerf;
                }
            }
        }
        #endregion

        #region Modify Hit By NPC
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            // Enemies deal less contact damage while sick, due to being weakened.
            if (npc.poisoned)
            {
                float damageReductionFromPoison = npc.Calamity().irradiated > 0 ? 0.075f : 0.05f;
                if (npc.Calamity().VulnerableToSickness.HasValue)
                {
                    if (npc.Calamity().VulnerableToSickness.Value)
                        damageReductionFromPoison *= 2f;
                    else
                        damageReductionFromPoison /= 2f;
                }
                damageReductionFromPoison = 1f - damageReductionFromPoison;

                modifiers.SourceDamage *= damageReductionFromPoison;
            }

            if (npc.venom)
            {
                float damageReductionFromVenom = npc.Calamity().irradiated > 0 ? 0.075f : 0.05f;
                if (npc.Calamity().VulnerableToSickness.HasValue)
                {
                    if (npc.Calamity().VulnerableToSickness.Value)
                        damageReductionFromVenom *= 2f;
                    else
                        damageReductionFromVenom /= 2f;
                }
                damageReductionFromVenom = 1f - damageReductionFromVenom;

                modifiers.SourceDamage *= damageReductionFromVenom;
            }

            if (npc.Calamity().astralInfection > 0)
            {
                float damageReductionFromAstralInfection = npc.Calamity().irradiated > 0 ? 0.075f : 0.05f;
                if (npc.Calamity().VulnerableToSickness.HasValue)
                {
                    if (npc.Calamity().VulnerableToSickness.Value)
                        damageReductionFromAstralInfection *= 2f;
                    else
                        damageReductionFromAstralInfection /= 2f;
                }
                damageReductionFromAstralInfection = 1f - damageReductionFromAstralInfection;

                modifiers.SourceDamage *= damageReductionFromAstralInfection;
            }

            if (npc.Calamity().pFlames > 0)
            {
                float damageReductionFromPlague = npc.Calamity().irradiated > 0 ? 0.075f : 0.05f;
                if (npc.Calamity().VulnerableToSickness.HasValue)
                {
                    if (npc.Calamity().VulnerableToSickness.Value)
                        damageReductionFromPlague *= 2f;
                    else
                        damageReductionFromPlague /= 2f;
                }
                damageReductionFromPlague = 1f - damageReductionFromPlague;

                modifiers.SourceDamage *= damageReductionFromPlague;
            }

            if (npc.Calamity().wDeath > 0)
            {
                float damageReductionFromWhisperingDeath = npc.Calamity().irradiated > 0 ? 0.15f : 0.1f;
                if (npc.Calamity().VulnerableToSickness.HasValue)
                {
                    if (npc.Calamity().VulnerableToSickness.Value)
                        damageReductionFromWhisperingDeath *= 2f;
                    else
                        damageReductionFromWhisperingDeath /= 2f;
                }
                damageReductionFromWhisperingDeath = 1f - damageReductionFromWhisperingDeath;

                modifiers.SourceDamage *= damageReductionFromWhisperingDeath;
            }

            //
            // At this point, the player is guaranteed to be hit if there is no dodge.
            // The amount of damage that will be dealt is yet to be determined.
            //

            if (transformer)
            {
                if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.PinkJellyfish || npc.type == NPCID.GreenJellyfish || npc.type == NPCID.FungoFish ||
                    npc.type == NPCID.BloodJelly || npc.type == ModContent.NPCType<Stormlion>() || npc.type == ModContent.NPCType<GhostBell>() || npc.type == ModContent.NPCType<BoxJellyfish>())
                    contactDamageReduction += 0.5;
            }

            // Can't have any cooldowns here because dodges grrrrr....
            if (fleshTotem && !Player.HasCooldown(Cooldowns.FleshTotem.ID) && TotalEnergyShielding <= 0)
                contactDamageReduction += 0.5;

            if (tarragonCloak && tarraMelee && !Player.HasCooldown(Cooldowns.TarragonCloak.ID))
                contactDamageReduction += 0.5;

            if (bloodflareMelee && bloodflareFrenzy && !Player.HasCooldown(BloodflareFrenzy.ID))
                contactDamageReduction += 0.5;

            if (npc.Calamity().tSad > 0)
                contactDamageReduction += 0.5;

            if (npc.Calamity().relicOfResilienceWeakness > 0)
            {
                contactDamageReduction += Items.Weapons.Typeless.RelicOfResilience.WeaknessDR;
                npc.Calamity().relicOfResilienceWeakness = 0;
            }

            if (eskimoSet)
            {
                if (npc.coldDamage)
                    contactDamageReduction += 0.1;
            }

            if (trinketOfChiBuff)
                contactDamageReduction += 0.04;

            // Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
            // To prevent abuse, this effect does not work if there are any bosses alive.
            if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
                contactDamageReduction += 0.15;

            if (abyssalDivingSuitPlates)
                contactDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

            if (aquaticHeartIce)
                contactDamageReduction += 0.2;

            if (encased)
                contactDamageReduction += 0.3;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && Player.ActiveItem().type == ModContent.ItemType<LionHeart>())
                contactDamageReduction += 0.5;

            bool lifeAndShieldCondition = Player.statLife >= Player.statLifeMax2 && (!HasAnyEnergyShield || TotalEnergyShielding >= TotalMaxShieldDurability);
            if (theBee && theBeeCooldown <= 0 && lifeAndShieldCondition)
            {
                contactDamageReduction += 0.5;
                theBeeCooldown = 600;
            }

            // Apply Adrenaline DR if available
            if (AdrenalineEnabled)
            {
                bool fullAdrenWithoutDH = !draedonsHeart && (adrenaline == adrenalineMax) && !adrenalineModeActive;
                bool usingNanomachinesWithDH = draedonsHeart && adrenalineModeActive;

                // 18AUG2023: Ozzatron: Adrenaline DR does not apply if you have energy shields active.
                // Otherwise, it becomes almost impossible to break energy shields due to Adrenaline DR.
                // If the shield never breaks, you never lose full Adrenaline, meaning you keep the DR forever and are functionally immortal.
                // This intentionally gives Adrenaline much-needed anti-synergy with energy shields, because they make gaining Adrenaline much safer.
                if ((fullAdrenWithoutDH || usingNanomachinesWithDH) && TotalEnergyShielding <= 0)
                    contactDamageReduction += this.GetAdrenalineDR();
            }

            if (Player.mount.Active && (Player.mount.Type == ModContent.MountType<RimehoundMount>() || Player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
                contactDamageReduction += 0.1;

            if (vHex)
                contactDamageReduction -= 0.1;

            if (irradiated)
                contactDamageReduction -= 0.1;

            if (corrEffigy)
                contactDamageReduction -= 0.05;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (contactDamageReduction > 0D)
            {
                if (aCrunch)
                    contactDamageReduction *= (double)ArmorCrunch.MultiplicativeDamageReductionPlayer;
                if (crumble)
                    contactDamageReduction *= (double)Crumbling.MultiplicativeDamageReductionPlayer;

                // Contact damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                // In GFB, as defense damage is uncapped, DR damage is also uncapped.
                int currentDefense = Player.GetCurrentDefense(false);
                if (totalDefenseDamage > 0 && currentDefense > 0)
                {
                    double drDamageRatio = CurrentDefenseDamage / (double)currentDefense;
                    if (!Main.getGoodWorld && drDamageRatio > 1D)
                        drDamageRatio = 1D;

                    contactDamageReduction *= 1D - drDamageRatio;
                    if (!Main.getGoodWorld && contactDamageReduction < 0D)
                        contactDamageReduction = 0D;
                }

                // Scale with base damage reduction
                if (Player.endurance > 0)
                    contactDamageReduction *= 1f - Player.endurance;

                contactDamageReduction = 1D / (1D + contactDamageReduction);
                modifiers.SourceDamage *= (float)contactDamageReduction;
            }

            if (Main.hardMode && Main.expertMode)
            {
                bool reduceChaosBallDamage = npc.type == NPCID.ChaosBall && !NPC.AnyNPCs(NPCID.GoblinSummoner);
                if (reduceChaosBallDamage || npc.type == NPCID.ChaosBallTim || npc.type == NPCID.BurningSphere || npc.type == NPCID.WaterSphere)
                    modifiers.SourceDamage *= 0.6f;
            }
        }
        #endregion

        #region Modify Hit By Proj
        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            // TODO -- Evolution dodge isn't actually a dodge and you'll still get hit for 1.
            // This should probably be changed so that when the evolution reflects it gives you 1 frame of guaranteed free dodging everything.
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && proj.damage > 0)
            {
                double dodgeDamageGateValuePercent = 0.05;
                int dodgeDamageGateValue = (int)Math.Round(Player.statLifeMax2 * dodgeDamageGateValuePercent);

                // Reflects count as dodges. They share the timer and can be disabled by Armageddon right click.
                if (!disableAllDodges && !Player.HasCooldown(GlobalDodge.ID) && proj.damage >= dodgeDamageGateValue)
                {
                    double maxCooldownDurationDamagePercent = 0.5;
                    int maxCooldownDurationDamageValue = (int)Math.Round(Player.statLifeMax2 * (maxCooldownDurationDamagePercent - dodgeDamageGateValuePercent));

                    // Just in case...
                    if (maxCooldownDurationDamageValue <= 0)
                        maxCooldownDurationDamageValue = 1;

                    float cooldownDurationScalar = MathHelper.Clamp((proj.damage - dodgeDamageGateValue) / (float)maxCooldownDurationDamageValue, 0f, 1f);

                    // The Evolution
                    if (evolution)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.velocity *= -2f;
                        proj.extraUpdates += 1;
                        proj.penetrate = 1;

                        // 17APR2024: Ozzatron: The Evolution is a reflect which also functions as a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
                        int evolutionIFrames = Player.ComputeReflectIFrames();
                        Player.GiveUniversalIFrames(evolutionIFrames, true);

                        modifiers.SetMaxDamage(1);
                        evolutionLifeRegenCounter = 300;
                        projTypeJustHitBy = proj.type;

                        int cooldownDuration = (int)MathHelper.Lerp(BalancingConstants.EvolutionReflectCooldownMin, BalancingConstants.EvolutionReflectCooldownMax, cooldownDurationScalar);
                        Player.AddCooldown(GlobalDodge.ID, cooldownDuration);

                        return;
                    }
                }
            }

            if (phantomicArtifact && Player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summon.PhantomicShield>()] != 0)
            {
                Projectile pro = Main.projectile.AsEnumerable().Where(projectile => projectile.friendly && projectile.owner == Player.whoAmI && projectile.type == ModContent.ProjectileType<Projectiles.Summon.PhantomicShield>()).First();
                phantomicBulwarkCooldown = 1800; // 30 second cooldown
                pro.Kill();
                projectileDamageReduction += 0.2;
            }

            if (auralisAuroraCounter >= 300)
            {
                modifiers.SourceDamage.Flat -= 100;

                auralisAuroraCounter = 0;
                auralisAuroraCooldown = CalamityUtils.SecondsToFrames(30f);
            }

            // Torch God does 1 damage but inflicts a random fire debuff
            if (proj.type == ProjectileID.TorchGod)
                modifiers.SetMaxDamage(1);

            // Reduce damage from vanilla traps

            // Explosives
            // 350 in normal, 450 in expert
            if (proj.type == ProjectileID.Explosives)
                modifiers.SourceDamage *= (Main.expertMode ? 0.225f : 0.35f);

            // Rolling Cacti
            // 45 in normal, 65 in expert for cactus
            // 30 in normal, 36 in expert for spikes
            else if (proj.type == ProjectileID.RollingCactus || proj.type == ProjectileID.RollingCactusSpike)
                modifiers.SourceDamage *= (Main.expertMode ? 0.3f : 0.5f);

            // Normal Boulders and Temple traps
            if (Main.expertMode)
            {
                // 140 in normal, 182 in expert, 273 in master
                if (proj.type == ProjectileID.Boulder || proj.type == ProjectileID.MiniBoulder)
                    modifiers.SourceDamage *= 0.65f;

                // 80 in normal, 100 in expert, 150 in master
                else if (proj.type == ProjectileID.SpikyBallTrap || proj.type == ProjectileID.FlamethrowerTrap || proj.type == ProjectileID.PoisonDartTrap)
                    modifiers.SourceDamage *= 0.625f;

                // 120 in normal, 144 in expert, 216 in master
                else if (proj.type == ProjectileID.SpearTrap)
                    modifiers.SourceDamage *= 0.6f;
            }

            // Reduce damage dealt by rainbow trails depending on how faded they are.
            if (proj.type == ProjectileID.HallowBossLastingRainbow)
            {
                // Find the oldPos of the projectile that is intersecting the player hitbox.
                Rectangle hitbox = proj.Hitbox;
                int trailLength = 80;
                int startOfDamageFalloff = 20;
                for (int k = 0; k < trailLength; k += 2)
                {
                    Vector2 trailHitbox = proj.oldPos[k];
                    if (!(trailHitbox == Vector2.Zero))
                    {
                        hitbox.X = (int)trailHitbox.X;
                        hitbox.Y = (int)trailHitbox.Y;

                        // Adjust damage based on what part of the trail intersected the player hitbox.
                        if (hitbox.Intersects(Player.Hitbox))
                        {
                            if (k > startOfDamageFalloff)
                                modifiers.SourceDamage *= MathHelper.Lerp(0.4f, 1f, 1f - (k - startOfDamageFalloff) / (float)(trailLength - startOfDamageFalloff));

                            break;
                        }
                    }
                }
            }

            //
            // At this point, the player is guaranteed to be hit if there is no dodge.
            // The amount of damage that will be dealt is yet to be determined.
            //

            if (evolution)
            {
                if (proj.type == projTypeJustHitBy)
                    projectileDamageReduction += 0.15;
            }

            if (transformer)
            {
                if (proj.type == ProjectileID.MartianTurretBolt || proj.type == ProjectileID.GigaZapperSpear || proj.type == ProjectileID.CultistBossLightningOrbArc || proj.type == ProjectileID.VortexLightning || proj.type == ModContent.ProjectileType<DestroyerElectricLaser>() ||
                    proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
                    projectileDamageReduction += 0.5;
            }

            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && proj.damage > 0)
            {
                // Daedalus Reflect counts as a reflect but doesn't actually stop you from taking damage
                if (daedalusReflect && !disableAllDodges && !evolution && !Player.HasCooldown(GlobalDodge.ID))
                    projectileDamageReduction += 0.5;
            }

            if (trinketOfChiBuff)
                projectileDamageReduction += 0.04;

            // Fearmonger set provides 15% multiplicative DR that ignores caps during the Holiday Moons.
            // To prevent abuse, this effect does not work if there are any bosses alive.
            if (fearmongerSet && !areThereAnyDamnBosses && (Main.pumpkinMoon || Main.snowMoon))
                projectileDamageReduction += 0.15;

            if (abyssalDivingSuitPlates)
                projectileDamageReduction += 0.15 - abyssalDivingSuitPlateHits * 0.03;

            if (aquaticHeartIce)
                projectileDamageReduction += 0.2;

            if (encased)
                projectileDamageReduction += 0.3;

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<EnergyShell>()] > 0 && Player.ActiveItem().type == ModContent.ItemType<LionHeart>())
                projectileDamageReduction += 0.5;

            bool lifeAndShieldCondition = Player.statLife >= Player.statLifeMax2 && (!HasAnyEnergyShield || TotalEnergyShielding >= TotalMaxShieldDurability);
            if (theBee && theBeeCooldown <= 0 && lifeAndShieldCondition)
            {
                projectileDamageReduction += 0.5;
                theBeeCooldown = 600;
            }

            // Apply Adrenaline DR if available
            if (AdrenalineEnabled)
            {
                bool fullAdrenWithoutDH = !draedonsHeart && (adrenaline == adrenalineMax) && !adrenalineModeActive;
                bool usingNanomachinesWithDH = draedonsHeart && adrenalineModeActive;

                // 18AUG2023: Ozzatron: Adrenaline DR does not apply if you have energy shields active.
                // Otherwise, it becomes almost impossible to break energy shields due to Adrenaline DR.
                // If the shield never breaks, you never lose full Adrenaline, meaning you keep the DR forever and are functionally immortal.
                // This intentionally gives Adrenaline much-needed anti-synergy with energy shields, because they make gaining Adrenaline much safer.
                if ((fullAdrenWithoutDH || usingNanomachinesWithDH) && TotalEnergyShielding <= 0)
                    projectileDamageReduction += this.GetAdrenalineDR();
            }

            if (Player.mount.Active && (Player.mount.Type == ModContent.MountType<RimehoundMount>() || Player.mount.Type == ModContent.MountType<OnyxExcavator>()) && Math.Abs(Player.velocity.X) > Player.mount.RunSpeed / 2f)
                projectileDamageReduction += 0.1;

            // Damage reduction from Shield of the High Ruler if facing the projectile that just hit.
            // If the projectile is in the exact center of the player on the X axis YOU GET NOTHING, GOOD DAY, SIR!
            if (copyrightInfringementShield)
            {
                bool projectileRight = (Player.Center.X - proj.Center.X) < 0f;
                bool projectileLeft = (Player.Center.X - proj.Center.X) > 0f;
                if (Player.direction == 1)
                {
                    if (projectileRight)
                        projectileDamageReduction += 0.15;
                }
                else
                {
                    if (projectileLeft)
                        projectileDamageReduction += 0.15;
                }
            }

            if (vHex)
                projectileDamageReduction -= 0.1;

            if (irradiated)
                projectileDamageReduction -= 0.1;

            if (corrEffigy)
                projectileDamageReduction -= 0.05;

            // 10% is converted to 9%, 25% is converted to 20%, 50% is converted to 33%, 75% is converted to 43%, 100% is converted to 50%
            if (projectileDamageReduction > 0D)
            {
                if (aCrunch)
                    projectileDamageReduction *= (double)ArmorCrunch.MultiplicativeDamageReductionPlayer;
                if (crumble)
                    projectileDamageReduction *= (double)Crumbling.MultiplicativeDamageReductionPlayer;

                // Projectile damage reduction is reduced by DR Damage, which itself is proportional to defense damage
                int currentDefense = Player.GetCurrentDefense(false);
                if (totalDefenseDamage > 0 && currentDefense > 0)
                {
                    double drDamageRatio = CurrentDefenseDamage / (double)currentDefense;
                    if (drDamageRatio > 1D)
                        drDamageRatio = 1D;

                    projectileDamageReduction *= 1D - drDamageRatio;

                    if (projectileDamageReduction < 0D)
                        projectileDamageReduction = 0D;
                }

                // Scale with base damage reduction
                if (Player.endurance > 0)
                    projectileDamageReduction *= 1f - Player.endurance;

                projectileDamageReduction = 1D / (1D + projectileDamageReduction);
                modifiers.SourceDamage *= (float)projectileDamageReduction;
            }
        }
        #endregion

        #region On Hit By NPC / Projectile
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = false;
            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                if (Player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this NPC deals defense damage with contact damage, then mark the player to take defense damage.
            // Defense damage is not applied if the player has iframes, or is in Journey god mode.
            if (!hasIFrames && !Player.creativeGodMode)
                nextHitDealsDefenseDamage |= npc.Calamity().canBreakPlayerDefense;

            // ModifyHit (Flesh Totem effect happens here) -> Hurt (includes dodges) -> OnHit
            // As such, to avoid cooldowns proccing from dodge hits, do it here
            if (fleshTotem && !Player.HasCooldown(Cooldowns.FleshTotem.ID) && hurtInfo.Damage > 0)
                Player.AddCooldown(Cooldowns.FleshTotem.ID, CalamityUtils.SecondsToFrames(20), true, "default");

            if (NPC.AnyNPCs(ModContent.NPCType<THELORDE>()))
                Player.AddBuff(ModContent.BuffType<NOU>(), 15, true);

            if (crawCarapace)
            {
                npc.AddBuff(ModContent.BuffType<Crumbling>(), 900);
                int onHitDamage = Player.CalcIntDamage<GenericDamageClass>(40);
                Player.ApplyDamageToNPC(npc, onHitDamage, 0f, 0, false);
                SoundEngine.PlaySound(SoundID.NPCHit33 with { Volume = 0.5f }, Player.Center);
            }

            if (baroclaw)
            {
                npc.AddBuff(ModContent.BuffType<ArmorCrunch>(), 900);
                npc.AddBuff(ModContent.BuffType<CrushDepth>(), 900);
                int onHitDamage = Player.CalcIntDamage<GenericDamageClass>(250);
                Player.ApplyDamageToNPC(npc, onHitDamage, 0f, 0, false);
                SoundEngine.PlaySound(BaroclawHit, Player.Center);
                Vector2 bloodSpawnPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height) * 0.04f;
                Vector2 splatterDirection = (Player.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
                for (int i = 0; i < 9; i++)
                {
                    int sparkLifetime = Main.rand.Next(12, 18);
                    float sparkScale = Main.rand.NextFloat(0.8f, 1f) * 0.955f;
                    Color sparkColor = Color.Lerp(Color.RoyalBlue, Color.DarkBlue, Main.rand.NextFloat(0.7f));
                    sparkColor = Color.Lerp(sparkColor, Color.RoyalBlue, Main.rand.NextFloat());
                    Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 25f);
                    sparkVelocity.Y -= 5.5f;
                    SparkParticle spark = new SparkParticle(Player.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            if (absorber)
            {
                npc.AddBuff(ModContent.BuffType<AbsorberAffliction>(), 900);
                int onHitDamage = Player.CalcIntDamage<GenericDamageClass>(350);
                Player.ApplyDamageToNPC(npc, onHitDamage, 0f, 0, false);
                SoundEngine.PlaySound(AbsorberHit, Player.Center);
                Vector2 bloodSpawnPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height) * 0.04f;
                Vector2 splatterDirection = (Player.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
                for (int i = 0; i < 12; i++)
                {
                    int sparkLifetime = Main.rand.Next(11, 16);
                    float sparkScale = Main.rand.NextFloat(1.8f, 2.8f) * 0.955f;
                    Color sparkColor = Color.Lerp(Color.DarkSeaGreen, Color.MediumSeaGreen, Main.rand.NextFloat(0.7f));
                    sparkColor = Color.Lerp(sparkColor, Color.DarkSeaGreen, Main.rand.NextFloat());
                    Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(12f, 25f);
                    sparkVelocity.Y -= 4.7f;
                    SparkParticle spark = new SparkParticle(Player.Center, sparkVelocity, false, sparkLifetime, sparkScale, sparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            if (alchFlask)
            {
                for (int i = 0; i < 9; i++)
                {
                    int seekerDamage = (int)Player.GetBestClassDamage().ApplyTo(15);
                    seekerDamage = Player.ApplyArmorAccDamageBonusesTo(seekerDamage);

                    Projectile bee = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.2f), ModContent.ProjectileType<BasicPlagueBee>(), seekerDamage, 0f, Player.whoAmI, -20, 30, 2);
                    bee.ArmorPenetration = 35;
                    bee.penetrate = 6;
                    bee.extraUpdates = 2;
                    bee.timeLeft = 600;
                }
                Player.AddBuff(BuffID.Honey, 900);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = false;
            for (int i = 0; i < Player.hurtCooldowns.Length; i++)
                if (Player.hurtCooldowns[i] > 0)
                    hasIFrames = true;

            // If this projectile is capable of dealing defense damage, then mark the player to take defense damage.
            // Defense damage is not applied if the player has iframes, or is in Journey god mode.
            if (!hasIFrames && !Player.creativeGodMode)
                nextHitDealsDefenseDamage |= proj.Calamity().DealsDefenseDamage;

            if (sulphurSet && !proj.friendly && hurtInfo.Damage > 0)
            {
                if (Main.player[proj.owner] is null)
                {
                    if (!Main.npc[proj.owner].friendly)
                        Main.npc[proj.owner].AddBuff(BuffID.Poisoned, 60);
                }
                else
                {
                    Player p = Main.player[proj.owner];
                    if (p.hostile && Player.hostile && (Player.team != p.team || p.team == 0))
                        p.AddBuff(BuffID.Poisoned, 60);
                }
            }

            if (proj.hostile && hurtInfo.Damage > 0)
            {
                if (proj.type == ProjectileID.TorchGod)
                {
                    int fireDebuffTypes = CalamityWorld.death ? 9 : CalamityWorld.revenge ? 7 : Main.expertMode ? 5 : 3;
                    int choice = Main.zenithWorld ? 9 : Main.rand.Next(fireDebuffTypes);
                    switch (choice)
                    {
                        case 0:
                            Player.AddBuff(BuffID.OnFire, 600);
                            break;

                        case 1:
                            Player.AddBuff(BuffID.Frostburn, 300);
                            break;

                        case 2:
                            Player.AddBuff(BuffID.CursedInferno, 300);
                            break;

                        case 3:
                            Player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
                            break;

                        case 4:
                            Player.AddBuff(ModContent.BuffType<Shadowflame>(), 150);
                            break;

                        case 5:
                            Player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 100);
                            break;

                        case 6:
                            Player.AddBuff(ModContent.BuffType<HolyFlames>(), 200);
                            break;

                        case 7:
                            Player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 300);
                            break;

                        case 8:
                            Player.AddBuff(ModContent.BuffType<Dragonfire>(), 150);
                            break;

                        case 9:
                            Player.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
                            break;
                    }
                }
                else if (proj.type == ProjectileID.Explosives)
                {
                    Player.AddBuff(BuffID.OnFire, 600);
                }
                else if (proj.type == ProjectileID.Boulder)
                {
                    Player.AddBuff(BuffID.BrokenArmor, 600);
                }
                else if (proj.type == ProjectileID.DeathLaser || proj.type == ProjectileID.RocketSkeleton || proj.type == ProjectileID.BombSkeletronPrime)
                {
                    Player.AddBuff(BuffID.OnFire, 180);
                }
                else if (proj.type == ProjectileID.Skull)
                {
                    Player.AddBuff(BuffID.Weak, 180);
                }
                else if (proj.type == ProjectileID.CursedFlameHostile || proj.type == ProjectileID.EyeFire)
                {
                    // Guaranteed Cursed Inferno for 1 second (vanilla also has a 68.75% chance of Cursed Inferno for 2 to 3 seconds)
                    Player.AddBuff(BuffID.CursedInferno, 60);
                }
                else if (proj.type == ProjectileID.Stinger || proj.type == ProjectileID.QueenBeeStinger)
                {
                    // 66.6% chance of Poison for 3 seconds, 1 guaranteed second of Poison otherwise (vanilla also has a 33.3% chance of Poison for 10 seconds)
                    if (Main.rand.Next(3) > 0)
                        Player.AddBuff(BuffID.Poisoned, 180);
                    else
                        Player.AddBuff(BuffID.Poisoned, 60);
                }
                else if (proj.type == ProjectileID.PoisonSeedPlantera)
                {
                    // 75% chance of Poison for 3 to 5 seconds, guaranteed Poison for 2 seconds (vanilla also has a 50% chance of Poison for 3 to 7 seconds)
                    if (Main.rand.Next(4) > 0)
                        Player.AddBuff(BuffID.Poisoned, Main.rand.Next(180, 301));
                    else
                        Player.AddBuff(BuffID.Poisoned, 120);
                }
                else if (proj.type == ProjectileID.ThornBall)
                {
                    Player.AddBuff(BuffID.Poisoned, 300);
                }
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    if (!Player.frozen)
                        Player.AddBuff(BuffID.Frozen, 60);

                    Player.AddBuff(BuffID.Chilled, 180);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    Player.AddBuff(BuffID.Electrified, 120);
                }
                else if (proj.type == ProjectileID.AncientDoomProjectile)
                {
                    Player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.CultistBossFireBallClone)
                {
                    Player.AddBuff(ModContent.BuffType<Shadowflame>(), 120);
                }
                else if (proj.type == ProjectileID.PhantasmalBolt || proj.type == ProjectileID.PhantasmalEye)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 60);
                }
                else if (proj.type == ProjectileID.PhantasmalSphere)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 120);
                }
                else if (proj.type == ProjectileID.PhantasmalDeathray)
                {
                    Player.AddBuff(ModContent.BuffType<Nightwither>(), 300);
                }
                else if (proj.type == ProjectileID.FairyQueenLance || proj.type == ProjectileID.HallowBossRainbowStreak || proj.type == ProjectileID.HallowBossSplitShotCore)
                {
                    Player.AddBuff(NPC.ShouldEmpressBeEnraged() ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 60);
                }
                else if (proj.type == ProjectileID.HallowBossLastingRainbow)
                {
                    Player.AddBuff(NPC.ShouldEmpressBeEnraged() ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 120);
                }
                else if (proj.type == ProjectileID.FairyQueenSunDance)
                {
                    Player.AddBuff(NPC.ShouldEmpressBeEnraged() ? ModContent.BuffType<HolyFlames>() : ModContent.BuffType<Nightwither>(), 180);
                }
                else if (proj.type == ProjectileID.BloodNautilusShot)
                {
                    Player.AddBuff(ModContent.BuffType<BurningBlood>(), 120);
                }
                else if (proj.type == ProjectileID.BloodShot)
                {
                    Player.AddBuff(ModContent.BuffType<BurningBlood>(), 60);
                }
                else if (proj.type == ProjectileID.RuneBlast && Main.zenithWorld)
                {
                    Player.AddBuff(ModContent.BuffType<MiracleBlight>(), 600);
                }
            }

            // As these reflects do not cancel damage, they need to be in OnHit rather than ModifyHit
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => proj.type != x) && proj.active && !proj.friendly && proj.hostile && hurtInfo.Damage > 0)
            {
                // The Transformer can reflect bullets
                if (transformer)
                {
                    if (proj.type == ProjectileID.BulletSnowman || proj.type == ProjectileID.BulletDeadeye || proj.type == ProjectileID.SniperBullet || proj.type == ProjectileID.VortexLaser)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.velocity *= -1f;
                        proj.damage = (int)Player.GetBestClassDamage().ApplyTo(proj.damage * 8);
                        proj.penetrate = 1;

                        // 17APR2024: Ozzatron: The Transformer is a reflect which also functions as a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
                        int transformerIFrames = Player.ComputeReflectIFrames();
                        Player.GiveUniversalIFrames(transformerIFrames, true);
                    }
                }

                double dodgeDamageGateValuePercent = 0.05;
                int dodgeDamageGateValue = (int)Math.Round(Player.statLifeMax2 * dodgeDamageGateValuePercent);

                // Reflects count as dodges. They share the timer and can be disabled by global dodge disabling effects.
                if (!disableAllDodges && !Player.HasCooldown(GlobalDodge.ID) && proj.damage >= dodgeDamageGateValue)
                {
                    double maxCooldownDurationDamagePercent = 0.5;
                    int maxCooldownDurationDamageValue = (int)Math.Round(Player.statLifeMax2 * (maxCooldownDurationDamagePercent - dodgeDamageGateValuePercent));

                    // Just in case...
                    if (maxCooldownDurationDamageValue <= 0)
                        maxCooldownDurationDamageValue = 1;

                    float cooldownDurationScalar = MathHelper.Clamp((proj.damage - dodgeDamageGateValue) / (float)maxCooldownDurationDamageValue, 0f, 1f);

                    if (daedalusReflect && !evolution)
                    {
                        proj.hostile = false;
                        proj.friendly = true;
                        proj.velocity *= -1f;
                        proj.penetrate = 1;

                        // 17APR2024: Ozzatron: The Daedalus Reflect set bonus also functions as a dodge. It uses vanilla dodge iframes and benefits from Cross Necklace.
                        int daedalusReflectIFrames = Player.ComputeReflectIFrames();
                        Player.GiveUniversalIFrames(daedalusReflectIFrames, true);

                        int cooldownDuration = (int)MathHelper.Lerp(BalancingConstants.DaedalusReflectCooldownMin, BalancingConstants.DaedalusReflectCooldownMax, cooldownDurationScalar);
                        Player.AddCooldown(GlobalDodge.ID, cooldownDuration);
                    }
                }
            }
            if (NPC.AnyNPCs(ModContent.NPCType<THELORDE>()))
            {
                Player.AddBuff(ModContent.BuffType<NOU>(), 15, true);
            }
        }
        #endregion

        #region Free and Consumable Dodge Hooks
        public override bool FreeDodge(Player.HurtInfo info)
        {
            // 22AUG2023: Ozzatron: god slayer damage resistance removed due to it being strong enough to godmode rev yharon
            // If the incoming damage is somehow less than 1 (TML doesn't allow this, but...), the hit is completely ignored.
            if (info.Damage < 1 /* || (godSlayerDamage && info.Damage <= 80) */)
                return true;

            // If this hit was marked to be completely ignored due to shield absorption, then process Adrenaline changes and ignore it.
            if (freeDodgeFromShieldAbsorption)
            {
                freeDodgeFromShieldAbsorption = false;

                // 20FEB2024: Ozzatron: Hits fully absorbed by shields remove half of your current Adrenaline.
                // If using Draedon's Heart, it pauses for half the typical duration.
                LoseAdrenalineOnHurt(info, true);
                return true;
            }

            // Gravistar Sabaton fall ram gives you a free dodge as long as you're slamming through NPCs
            // This also strikes the NPCs as a side effect
            if (gSabatonFalling)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    // Ignore critters with the Guide to Critter Companionship
                    if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[n.type])
                        continue;

                    if (!n.dontTakeDamage && !n.friendly && n.Calamity().dashImmunityTime[Player.whoAmI] <= 0)
                    {
                        Rectangle npcHitbox = n.getRect();
                        if ((Player.getRect()).Intersects(npcHitbox) && (n.noTileCollide || Collision.CanHit(Player.position, Player.width, Player.height, n.position, n.width, n.height)))
                        {
                            int damage = Player.ApplyArmorAccDamageBonusesTo(Player.CalcIntDamage<MeleeDamageClass>(GravistarSabaton.PassthroughDamage));

                            Projectile.NewProjectile(Player.GetSource_FromThis(), n.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), damage, 0, Main.myPlayer);

                            // 17APR2024: Ozzatron: Gravistar Sabaton gives iframes when passing through enemies for projectile safety.
                            // This is a fixed and intentionally very low number of iframes, and is not boosted by Cross Necklace.
                            n.Calamity().dashImmunityTime[Player.whoAmI] = 4;
                            Player.GiveUniversalIFrames(GravistarSabaton.PassthroughIFrames, false);

                            return true;
                        }
                    }
                }
            }

            // If no other effects occurred, run vanilla code
            return base.FreeDodge(info);
        }

        public override bool ConsumableDodge(Player.HurtInfo info)
        {
            // Vanilla dodges are gated behind the global dodge cooldown
            // The dodges will only trigger if the player has taken greater than or equal to 5% of their max HP in damage
            double dodgeDamageGateValuePercent = 0.05;
            int dodgeDamageGateValue = (int)Math.Round(Player.statLifeMax2 * dodgeDamageGateValuePercent);

            // 14MAY2024: Ozzatron: Chalice of the Blood God now works with dodges
            int actualDamageTaken = chaliceOfTheBloodGod ? chaliceHitOriginalDamage : info.Damage;
            bool sufficientDamageForDodging = actualDamageTaken >= dodgeDamageGateValue;

            if (!Player.HasCooldown(GlobalDodge.ID) && sufficientDamageForDodging)
            {
                double maxCooldownDurationDamagePercent = 0.5;
                int maxCooldownDurationDamageValue = (int)Math.Round(Player.statLifeMax2 * (maxCooldownDurationDamagePercent - dodgeDamageGateValuePercent));
                
                // Just in case...
                if (maxCooldownDurationDamageValue <= 0)
                    maxCooldownDurationDamageValue = 1;

                float cooldownDurationScalar = MathHelper.Clamp((actualDamageTaken - dodgeDamageGateValue) / (float)maxCooldownDurationDamageValue, 0f, 1f);

                // Re-implementation of vanilla item Black Belt as a consumable dodge
                if (Player.whoAmI == Main.myPlayer && Player.blackBelt)
                {
                    Player.NinjaDodge();
                    int cooldownDuration = (int)MathHelper.Lerp(BalancingConstants.BeltDodgeCooldownMin, BalancingConstants.BeltDodgeCooldownMax, cooldownDurationScalar);
                    Player.AddCooldown(GlobalDodge.ID, cooldownDuration);
                    return true;
                }

                // Re-implementation of vanilla item Brain of Confusion as a consumable dodge
                if (Player.whoAmI == Main.myPlayer && Player.brainOfConfusionItem != null && !Player.brainOfConfusionItem.IsAir)
                {
                    Player.BrainOfConfusionDodge();
                    int cooldownTime = amalgam ?
                        (int)MathHelper.Lerp(BalancingConstants.AmalgamDodgeCooldownMin, BalancingConstants.AmalgamDodgeCooldownMax, cooldownDurationScalar) :
                        (int)MathHelper.Lerp(BalancingConstants.BrainDodgeCooldownMin, BalancingConstants.BrainDodgeCooldownMax, cooldownDurationScalar);
                    Player.AddCooldown(GlobalDodge.ID, cooldownTime);
                    return true;
                }
            }

            //
            // CALAMITY DODGES
            //

            if (Player.whoAmI != Main.myPlayer || disableAllDodges)
                return false;

            if (spectralVeil && spectralVeilImmunity > 0)
            {
                SpectralVeilDodge();
                return true;
            }

            // TODO -- drag all dodge code into a CalamityPlayer sub-file dedicated to dodging and nothing else
            if (HandleDashDodges())
                return true;

            // Mirror evades do not work if the global dodge cooldown is active. This cooldown can be triggered by either mirror.
            if (!Player.HasCooldown(GlobalDodge.ID) && info.Damage >= dodgeDamageGateValue)
            {
                if (eclipseMirror)
                {
                    EclipseMirrorDodge(dodgeDamageGateValuePercent, dodgeDamageGateValue, info.Damage);
                    return true;
                }
                else if (abyssalMirror)
                {
                    AbyssMirrorDodge(dodgeDamageGateValuePercent, dodgeDamageGateValue, info.Damage);
                    return true;
                }
            }

            return base.ConsumableDodge(info);
        }
        #endregion

        #region Modify Hurt
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            // Handles energy shields and Boss Rush, in that order
            modifiers.ModifyHurtInfo += ModifyHurtInfo_Calamity;

            // TODO -- At some point it'd be nice to have a "TransformationPlayer" that has all the transformation sfx and visuals so their priorities can be more easily managed.
            #region Custom Hurt Sounds
            if (hurtSoundTimer == 0)
            {
                if (roverDrive && RoverDriveShieldDurability > 0)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(RoverDrive.ShieldHurtSound, Player.Center);
                    hurtSoundTimer = 20;
                }
                else if (lunicCorpsSet && LunicCorpsShieldDurability > 0)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(LunicCorpsHelmet.ShieldHurtSound, Player.Center);
                    hurtSoundTimer = 20;
                }
                else if (sponge && SpongeShieldDurability > 0)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(TheSponge.ShieldHurtSound, Player.Center);
                    hurtSoundTimer = 20;
                }
                else if (((pSoulArtifact && !profanedCrystal) || profanedCrystalBuffs) && pSoulShieldDurability > 0)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(ProfanedGuardianDefender.ShieldDeathSound);
                    hurtSoundTimer = 20;
                }
                else if ((profanedCrystal || profanedCrystalForce) && !profanedCrystalHide)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(Providence.HurtSound, Player.Center);
                    hurtSoundTimer = 20;
                }
                else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(SoundID.NPCHit4, Player.Center); //metal hit noise
                    hurtSoundTimer = 10;
                }
                else if (((aquaticHeartPower || aquaticHeartForce) && !aquaticHeartHide) || Player.GetModPlayer<CrystalHeartVodkaPlayer>().vanityEquipped)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(SoundID.FemaleHit, Player.Center); //female hit noise
                    hurtSoundTimer = 10;
                }
                else if (titanHeartSet)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(NPCs.Astral.Atlas.HurtSound, Player.Center);
                    hurtSoundTimer = 10;
                }
                else if (Player.GetModPlayer<WulfrumTransformationPlayer>().transformationActive)
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(SoundID.NPCHit4, Player.Center);
                    hurtSoundTimer = 10;
                }
                else if (Player.GetModPlayer<WulfrumArmorPlayer>().wulfrumSet && (Player.name.ToLower() == "wagstaff" || Player.name.ToLower() == "john wulfrum"))
                {
                    modifiers.DisableSound();
                    SoundEngine.PlaySound(SoundID.DSTMaleHurt, Player.Center);
                    hurtSoundTimer = 10;
                }
            }
            #endregion

            #region Player Incoming Damage Multiplier (Increases)
            double damageMult = 1D;
            if (dArtifact) // Dimensional Soul Artifact increases incoming damage by 15%.
                damageMult += 0.15;
            if (enraged) // Demonshade Enrage increases incoming damage by 25%.
                damageMult += 0.25;

            modifiers.SourceDamage *= (float)damageMult;
            #endregion

            //
            // At this point, the true, final incoming damage to the player has been calculated.
            // It has not yet been mitigated by any means.
            //

            if (blazingCoreParry > 0) //check for active parry
            {
                if (blazingCoreParry >= 12) //only the first 18 frames (0.3 seconds) counts for a valid parry
                {
                    if (!Player.HasCooldown(ParryCooldown.ID))
                    {
                        // 17APR2024: Ozzatron: Blazing Core is a parry. It uses vanilla parry iframes and benefits from Cross Necklace.
                        int blazingCoreParryIFrames = Player.ComputeParryIFrames();
                        Player.GiveUniversalIFrames(blazingCoreParryIFrames, true);

                        blazingCoreEmpoweredParry = true;

                        modifiers.SetMaxDamage(1); //ONLY REDUCE DAMAGE IF NOT ON COOLDOWN
                        modifiers.DisableSound(); //prevents hurt sound from playing, had no idea this was a thing
                    }

                    SoundEngine.PlaySound(BlazingCore.ParrySuccessSound, Player.Center);
                    blazingCoreSuccessfulParry = 60;
                    Player.AddCooldown(ParryCooldown.ID, 60 * 30, false, "blazingcore"); //cooldown is frames in seconds multiplied by the desired amount of seconds
                }

                if (blazingCoreParry > 1)
                    blazingCoreParry = 1; //schedule parry to end next frame
            }
            else if (flameLickedShellParry > 0)
            {
                if (flameLickedShellParry >= 12)
                {
                    if (!Player.HasCooldown(ParryCooldown.ID))
                    {
                        // 17APR2024: Ozzatron: Flame-Licked Shell is a parry. It uses vanilla parry iframes and benefits from Cross Necklace.
                        int flameLickedShellParryIFrames = Player.ComputeParryIFrames();
                        Player.GiveUniversalIFrames(flameLickedShellParryIFrames, true);

                        flameLickedShellEmpoweredParry = true;

                        modifiers.FinalDamage *= 0.1f; //90% dr
                        modifiers.DisableSound();
                    }

                    SoundEngine.PlaySound(ProfanedGuardianDefender.ShieldDeathSound, Player.Center);
                    Player.AddCooldown(ParryCooldown.ID, 60 * 20, false, "flamelickedshell");
                    FlameLickedShell.handleParry(Player);
                }
            }
        }

        private void ModifyHurtInfo_Calamity(ref Player.HurtInfo info)
        {
            // Boss Rush's damage floor is implemented as a dirty modifier
            // TODO -- implementing this correctly would require fully reimplementing all of DR and ADR
            if (BossRushEvent.BossRushActive)
            {
                int bossRushDamageFloor = (Main.expertMode ? 160 : 100) + (BossRushEvent.BossRushStage * 2);
                if (info.Damage < bossRushDamageFloor)
                    info.Damage += (bossRushDamageFloor - info.Damage);
            }

            // Energy shields are implemented as a dirty modifier
            // This is what SLR Barrier does; see
            // https://github.com/ProjectStarlight/StarlightRiver/blob/master/Core/Systems/BarrierSystem/BarrierPlayer.cs
            //
            // Currently implemented energy shields:
            // - Rover Drive
            // - Lunic Corps Armor set bonus
            // - Profaned Soul Artifact/Crystal
            // - The Sponge
            //
            // If the shield(s) completely absorb the hit, iframes are granted on the spot and the hit is marked to be dodged.
            // Shields are drained in order of progression, so your weaker shields will break first.
            // Damage can and will be blocked by multiple shields if it has to be.
            bool shieldsFullyAbsorbedHit = false;
            if (HasAnyEnergyShield)
            {
                bool shieldsTookHit = false;
                bool anyShieldBroke = false;
                int totalDamageBlocked = 0;

                // ROVER DRIVE
                if (roverDrive && RoverDriveShieldDurability > 0 && !shieldsFullyAbsorbedHit)
                {
                    // Check whether this shield can fully absorb the incoming hit (or what's left of it).
                    bool thisShieldCanFullyAbsorb = RoverDriveShieldDurability >= info.Damage;

                    // Tally up how much damage was blocked by this shield.
                    int roverDriveDamageBlocked = Math.Min(RoverDriveShieldDurability, info.Damage);
                    totalDamageBlocked += roverDriveDamageBlocked;

                    // Deal all incoming damage to this shield, because it is available.
                    RoverDriveShieldDurability -= info.Damage;
                    shieldsTookHit = true;

                    // Hits which break the Rover Drive shield cause a sound and slight screen shake.
                    // Multiple shields breaking simultaneously has slightly stronger screen shake.
                    if (RoverDriveShieldDurability <= 0)
                    {
                        RoverDriveShieldDurability = 0;
                        SoundEngine.PlaySound(RoverDrive.BreakSound, Player.Center);
                        Player.Calamity().GeneralScreenShakePower += anyShieldBroke ? 0.5f : 2f;
                        anyShieldBroke = true;
                    }

                    // Mark the hit as being canceled if this shield has enough durability to fully absorb it.
                    // This prevents further shields from attempting to absorb the hit.
                    if (thisShieldCanFullyAbsorb)
                        shieldsFullyAbsorbedHit = true;

                    // Actually remove damage from the incoming hit, so that later shields have less damage incoming.
                    info.Damage -= roverDriveDamageBlocked;
                }

                // LUNIC CORPS ARMOR
                if (lunicCorpsSet && LunicCorpsShieldDurability > 0 && !shieldsFullyAbsorbedHit)
                {
                    // Check whether this shield can fully absorb the incoming hit (or what's left of it).
                    bool thisShieldCanFullyAbsorb = LunicCorpsShieldDurability >= info.Damage;

                    // Tally up how much damage was blocked by this shield.
                    int masterChefDamageBlocked = Math.Min(LunicCorpsShieldDurability, info.Damage);
                    totalDamageBlocked += masterChefDamageBlocked;

                    // Deal all incoming damage to this shield, because it is available.
                    LunicCorpsShieldDurability -= info.Damage;
                    shieldsTookHit = true;

                    // Hits which break the Lunic Corps shield cause a sound and a slight screen shake.
                    // Multiple shields breaking simultaneously has slightly stronger screen shake.
                    if (LunicCorpsShieldDurability <= 0)
                    {
                        LunicCorpsShieldDurability = 0;
                        SoundEngine.PlaySound(LunicCorpsHelmet.BreakSound, Player.Center);
                        Player.Calamity().GeneralScreenShakePower += anyShieldBroke ? 0.5f : 2f;
                        anyShieldBroke = true;
                    }

                    // Mark the hit as being canceled if this shield has enough durability to fully absorb it.
                    // This prevents further shields from attempting to absorb the hit.
                    if (thisShieldCanFullyAbsorb)
                        shieldsFullyAbsorbedHit = true;

                    // Actually remove damage from the incoming hit, so that later shields have less damage incoming.
                    info.Damage -= masterChefDamageBlocked;
                }

                // PSA
                if (pSoulArtifact && pSoulShieldDurability > 0 && !shieldsFullyAbsorbedHit)
                {
                    // Check whether this shield can fully absorb the incoming hit (or what's left of it).
                    bool thisShieldCanFullyAbsorb = pSoulShieldDurability >= info.Damage;

                    // Tally up how much damage was blocked by this shield.
                    int pSoulDamageBlocked = Math.Min(pSoulShieldDurability, info.Damage);
                    totalDamageBlocked += pSoulDamageBlocked;

                    // Deal all incoming damage to this shield, because it is available.
                    pSoulShieldDurability -= info.Damage;
                    shieldsTookHit = true;

                    // Hits which break the PSA shield cause a sound and slight screen shake.
                    // Multiple shields breaking simultaneously has slightly stronger screen shake.
                    if (pSoulShieldDurability <= 0)
                    {
                        pSoulShieldDurability = 0;
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Player.Center);
                        Player.Calamity().GeneralScreenShakePower += anyShieldBroke ? 0.5f : 2f;
                        anyShieldBroke = true;
                    }

                    // Mark the hit as being canceled if this shield has enough durability to fully absorb it.
                    // This prevents further shields from attempting to absorb the hit.
                    if (thisShieldCanFullyAbsorb)
                        shieldsFullyAbsorbedHit = true;

                    // Actually remove damage from the incoming hit, so that later shields have less damage incoming.
                    info.Damage -= pSoulDamageBlocked;
                }

                // THE SPONGE
                if (sponge && SpongeShieldDurability > 0 && !shieldsFullyAbsorbedHit)
                {
                    // Check whether this shield can fully absorb the incoming hit (or what's left of it).
                    bool thisShieldCanFullyAbsorb = SpongeShieldDurability >= info.Damage;

                    // Tally up how much damage was blocked by this shield.
                    int spongeDamageBlocked = Math.Min(SpongeShieldDurability, info.Damage);
                    totalDamageBlocked += spongeDamageBlocked;

                    // Deal all incoming damage to this shield, because it is available.
                    SpongeShieldDurability -= info.Damage;
                    shieldsTookHit = true;

                    // Hits which break The Sponge's shield cause a sound and a slight screen shake.
                    // Multiple shields breaking simultaneously has slightly stronger screen shake.
                    if (SpongeShieldDurability <= 0)
                    {
                        SpongeShieldDurability = 0;
                        SoundEngine.PlaySound(TheSponge.BreakSound, Player.Center);
                        Player.Calamity().GeneralScreenShakePower += anyShieldBroke ? 0.5f : 2f;
                        anyShieldBroke = true;
                    }

                    // Mark the hit as being canceled if this shield has enough durability to fully absorb it.
                    // This prevents further shields from attempting to absorb the hit.
                    if (thisShieldCanFullyAbsorb)
                        shieldsFullyAbsorbedHit = true;

                    // Actually remove damage from the incoming hit, so that later shields have less damage incoming.
                    info.Damage -= spongeDamageBlocked;
                }

                // If any shields took damage, there is some code that must be run.
                if (shieldsTookHit)
                {
                    // If any shields took damage, display text indicating that shield damage was taken.
                    string shieldDamageText = (-totalDamageBlocked).ToString();
                    Rectangle location = new Rectangle((int)Player.position.X, (int)Player.position.Y - 16, Player.width, Player.height);
                    CombatText.NewText(location, Color.LightBlue, Language.GetTextValue(shieldDamageText));

                    // Give the player iframes for taking a shield hit, regardless of whether or not the shields broke.
                    int shieldHitIFrames = Player.ComputeHitIFrames(info);
                    Player.GiveIFrames(info.CooldownCounter, shieldHitIFrames, true);

                    // Spawn particles when hit with the shields up, regardless of whether or not the shields broke.
                    // More particles spawn if a shield broke.
                    if (pSoulArtifact)
                    {
                        for (int i = 0; i < Main.rand.Next(4, 8); i++) //very light dust
                        {
                            Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, (int)CalamityDusts.ProfanedFire);
                            dust.velocity = Main.rand.NextVector2Circular(3.5f, 3.5f);
                            dust.velocity.Y -= Main.rand.NextFloat(1f, 3f);
                            dust.scale = Main.rand.NextFloat(1.15f, 1.45f);
                        }
                    }
                    else
                    {
                        int numParticles = Main.rand.Next(2, 6) + (anyShieldBroke ? 6 : 0);
                        for (int i = 0; i < numParticles; i++)
                        {
                            // Rover Drive has slightly higher particle velocity
                            float maxVelocity = roverDrive ? 14f : 7f;
                            Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, maxVelocity);
                            velocity.X += 5f * info.HitDirection;

                            float scale = Main.rand.NextFloat(2.5f, 3f);
                            Color particleColor = Main.rand.NextBool() ? new Color(99, 255, 229) : new Color(25, 132, 247);
                            int lifetime = 25;

                            var shieldParticle = new TechyHoloysquareParticle(Player.Center, velocity, scale, particleColor, lifetime);
                            GeneralParticleHandler.SpawnParticle(shieldParticle);
                        }
                    }

                    // Update Rover Drive durability on the cooldown rack.
                    if (roverDrive && cooldowns.TryGetValue(WulfrumRoverDriveDurability.ID, out var roverDriveDurabilityCD))
                        roverDriveDurabilityCD.timeLeft = RoverDriveShieldDurability;

                    // Update Lunic Corps Armor durability on the cooldown rack.
                    if (lunicCorpsSet && cooldowns.TryGetValue(Cooldowns.LunicCorpsShieldDurability.ID, out var masterChefDurabilityCD))
                        masterChefDurabilityCD.timeLeft = LunicCorpsShieldDurability;

                    // Update PSA/PSC durability on the cooldown rack
                    if (pSoulArtifact && (!profanedCrystal || profanedCrystalBuffs) && cooldowns.TryGetValue(Cooldowns.ProfanedSoulShield.ID, out var profanedSoulDurabilityCD))
                        profanedSoulDurabilityCD.timeLeft = pSoulShieldDurability;

                    // Update Sponge durability on the cooldown rack.
                    if (sponge && cooldowns.TryGetValue(SpongeDurability.ID, out var spongeDurabilityCD))
                        spongeDurabilityCD.timeLeft = SpongeShieldDurability;
                }

                // Regardless of whether shields took damage, iterate over and stall all shield regen on ANY hit.
                // This applies even if you are hit while shields are fully down, or if you unequip any of the relevant items.
                {
                    // Rover Drive does not recharge while partially full, only when broken.
                    // If you are hit when recharging, though, that timer gets reset.
                    if (cooldowns.TryGetValue(WulfrumRoverDriveRecharge.ID, out var roverDriveRechargeCD))
                        roverDriveRechargeCD.timeLeft = RoverDrive.ShieldRechargeTime;

                    // Set the Lunic Corps Armor's recharge delay to full. Override any existing cooldown instance.
                    if (lunicCorpsSet)
                        Player.AddCooldown(LunicCorpsShieldRecharge.ID, LunicCorpsHelmet.ShieldRechargeDelay, true);

                    if (pSoulArtifact && (!profanedCrystal || profanedCrystalBuffs))
                        Player.AddCooldown(ProfanedSoulShieldRecharge.ID, profanedCrystalBuffs ? (60 * 5) : (60 * 10), true); // 5 seconds psc, 10 seconds psa 

                    // Set The Sponge's recharge delay to full. Override any existing cooldown instance.
                    if (sponge)
                        Player.AddCooldown(SpongeRecharge.ID, TheSponge.ShieldRechargeDelay, true);
                }

                // If the shields completely absorbed the hit, then delete the hit using reflection.
                if (shieldsFullyAbsorbedHit)
                {
                    freeDodgeFromShieldAbsorption = true;

                    // Cancel defense damage, if it was going to occur this frame.
                    nextHitDealsDefenseDamage = false;
                }
            }

            // Chalice of the Blood God is implemented as a dirty modifier.
            //
            // Chalice of the Blood God does nothing to a hit that was just fully blocked by shields.
            // Otherwise, it reduces the damage of any hit to 5, which allows for full iframes.
            // It then applies the full hit (minus that 5 damage) to its own bleedout buffer in OnHurt (see below).
            // Hits for less than 5 damage are ignored entirely and allowed to strike the player as normal.
            if (chaliceOfTheBloodGod && !shieldsFullyAbsorbedHit && info.Damage > ChaliceOfTheBloodGod.MinAllowedDamage)
            {
                chaliceBleedoutToApplyOnHurt = info.Damage - ChaliceOfTheBloodGod.MinAllowedDamage;

                chaliceHitOriginalDamage = info.Damage;
                info.Damage = ChaliceOfTheBloodGod.MinAllowedDamage;
            }
        }
        #endregion

        #region On Hurt
        public override void OnHurt(Player.HurtInfo hurtInfo)
        {
            // If Armageddon is active, instantly kill the player.
            if (CalamityWorld.armageddon && areThereAnyDamnBosses)
                KillPlayer();

            #region Actually Dealing Defense Damage
            // Check if the player has iframes for the sake of avoiding defense damage.
            bool hasIFrames = Player.HasIFrames();

            // If the player was just hit by something capable of dealing defense damage, then apply defense damage.
            // Bloodflare Core makes every hit deal defense damage (to enable its function).
            // Defense damage is not applied if the player has iframes.
            // This function will be ignored if the player is wearing Chalice, as it handles its defense damage elsewhere.
            bool hitCanApplyDefenseDamage = nextHitDealsDefenseDamage || bloodflareCore;

            if (hitCanApplyDefenseDamage && !hasIFrames && !Player.creativeGodMode)
            {
                double halfDefense = Player.statDefense / 2.0;
                int netMitigation = hurtInfo.SourceDamage - hurtInfo.Damage;
                double standardDefenseDamage = netMitigation * defenseDamageRatio;

                // Bloodflare Core overrides standard defense damage if it would be less than half of the player's total defense.
                if (bloodflareCore && standardDefenseDamage < halfDefense)
                {
                    // In this case, forcibly deal half of the player's total defense as defense damage. This ignores ratios.
                    DealDefenseDamage((int)halfDefense, true);

                    // Set up Bloodflare Core's heal over time. Any in-progress heals are overwritten if they would have a shorter duration.
                    if (bloodflareCoreRemainingHealOverTime < halfDefense)
                        bloodflareCoreRemainingHealOverTime = (int)halfDefense;

                    // Play a sound and make dust to signify that defense has been shattered
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Player.Center);
                    for (int i = 0; i < 36; ++i)
                    {
                        float speed = Main.rand.NextFloat(1.8f, 8f);
                        Vector2 dustVel = new Vector2(speed, speed);
                        Dust d = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.GemRuby);
                        d.velocity = dustVel;
                        d.noGravity = true;
                        d.scale *= Main.rand.NextFloat(1.1f, 1.4f);
                        Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.PiOver2);
                        Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi);
                        Dust.CloneDust(d).velocity = dustVel.RotatedBy(MathHelper.Pi * 1.5f);
                    }
                }

                // Chalice of the Blood God has to compensate for the "mitigation" provided by its bleedout buffer
                else if (chaliceOfTheBloodGod)
                    DealDefenseDamage(hurtInfo, chaliceBleedoutToApplyOnHurt);

                // Otherwise, just deal regular defense damage.
                else
                    DealDefenseDamage(hurtInfo);
            }

            nextHitDealsDefenseDamage = false;
            #endregion

            #region Chalice of the Blood God Bleed Application
            // This is handled in OnHurt so that Chalice hits can still be dodged based on their appropriate normal damage
            // Defense damage based on the "total lethality of the hit" is applied immediately prior to this
            // 
            // 1 - Actually apply bleedout to the player based on the damage they would have taken
            // 2 - Display an indicator of how much damage was dealt as bleedout instead of regular damage
            if (chaliceOfTheBloodGod)
            {
                int bleedoutToApply = chaliceBleedoutToApplyOnHurt;
                chaliceBleedoutBuffer += bleedoutToApply;

                // Display text indicating that damage was transferred to bleedout.
                string text = $"({-bleedoutToApply})";
                Rectangle location = new Rectangle((int)Player.position.X + 4, (int)Player.position.Y - 3, Player.width - 4, Player.height - 4);
                CombatText.NewText(location, ChaliceOfTheBloodGod.BleedoutBufferDamageTextColor, Language.GetTextValue(text), dot: true);
            }
            #endregion

            #region Shattered Community Rage Gain
            // Shattered Community makes the player gain rage based on the amount of damage taken.
            // Also set the Rage gain cooldown to prevent bizarre abuse cases.
            if (shatteredCommunity && rageGainCooldown == 0)
            {
                float HPRatio = (float)hurtInfo.SourceDamage / Player.statLifeMax2;
                float rageConversionRatio = 0.8f;

                // Damage to rage conversion is half as effective while Rage Mode is active.
                if (rageModeActive)
                    rageConversionRatio *= 0.5f;
                // If Rage is over 100%, damage to rage conversion scales down asymptotically based on how full Rage is.
                if (rage >= rageMax)
                    rageConversionRatio *= 3f / (3f + rage / rageMax);

                rage += rageMax * HPRatio * rageConversionRatio;
                rageGainCooldown = ShatteredCommunity.RageGainCooldown;
                // Rage capping is handled in MiscEffects
            }
            #endregion

            modStealth = 1f;

            // Give Rage combat frames because being hurt counts as combat.
            if (RageEnabled)
                rageCombatFrames = BalancingConstants.RageCombatDelayTime;

            // Hide of Astrum Deus' melee boost
            if (hideOfDeus)
            {
                hideOfDeusMeleeBoostTimer += 3 * hurtInfo.Damage;
                if (hideOfDeusMeleeBoostTimer > 600)
                    hideOfDeusMeleeBoostTimer = 600;
            }

            if (Player.whoAmI == Main.myPlayer)
            {
                // Summon a portal if needed.
                if (Player.Calamity().persecutedEnchant)
                {
                    if (NPC.CountNPCS(ModContent.NPCType<DemonPortal>()) < 2)
                    {
                        int tries = 0;
                        Vector2 spawnPosition;
                        Vector2 spawnPositionOffset = Vector2.One * 24f;
                        do
                        {
                            spawnPosition = Player.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(270f, 420f);
                            tries++;
                        }
                        while (Collision.SolidCollision(spawnPosition - spawnPositionOffset, 48, 24) && tries < 100);
                        CalamityNetcode.NewNPC_ClientSide(spawnPosition, ModContent.NPCType<DemonPortal>(), Player);
                    }
                }

                if (daedalusAbsorb && Main.rand.NextBool(10))
                {
                    int healAmt = (int)(hurtInfo.Damage / 2D);
                    Player.statLife += healAmt;
                    Player.HealEffect(healAmt);
                }

                if (absorber)
                {
                    int healAmt = (int)(hurtInfo.Damage / 20D);
                    Player.statLife += healAmt;
                    Player.HealEffect(healAmt);
                }

                if (witheringDamageDone > 0)
                {
                    double healCompenstationRatio = Math.Log(witheringDamageDone) * Math.Pow(witheringDamageDone, 2D / 3D) / 177000D;
                    if (healCompenstationRatio > 1D)
                        healCompenstationRatio = 1D;
                    int healCompensation = (int)(healCompenstationRatio * hurtInfo.Damage);
                    Player.statLife += (int)(healCompenstationRatio * hurtInfo.Damage);
                    Player.HealEffect(healCompensation);
                    Player.AddBuff(ModContent.BuffType<Withered>(), 1080);
                    witheringDamageDone = 0;
                }

                // Lose adrenaline on hit, unless using Draedon's Heart.
                if (AdrenalineEnabled)
                {
                    LoseAdrenalineOnHurt(hurtInfo, false);
                }

                if (evilSmasherBoost > 0)
                    evilSmasherBoost -= 1;

                hellbornBoost = 0;

                if (trinketOfChi)
                    chiBuffTimer = 0;

                if (amidiasBlessing && hurtInfo.Damage > 50)
                {
                    Player.ClearBuff(ModContent.BuffType<AmidiasBlessing>());
                    SoundEngine.PlaySound(SoundID.Item96, Player.Center);
                }

                if (gShell) //5 seconds of no dash reduction and reduced defense
                {
                    if (giantShellPostHit == 0)
                    {
                        float numberOfDusts = 35f;
                        float rotFactor = 360f / numberOfDusts;
                        for (int i = 0; i < numberOfDusts; i++)
                        {
                            float rot = MathHelper.ToRadians(i * rotFactor);
                            Vector2 offset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                            Vector2 velOffset = new Vector2(Main.rand.NextFloat(0.5f, 2.5f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                            Dust dust = Dust.NewDustPerfect(Player.Center + offset, Main.rand.NextBool() ? 249 : 118, new Vector2(velOffset.X, velOffset.Y));
                            dust.noGravity = false;
                            dust.velocity = velOffset;
                            dust.scale = Main.rand.NextFloat(1.5f, 1.2f);
                        }
                    }
                    giantShellPostHit = 300;
                }

                if (tortShell) //5 seconds of no dash reduction and reduced defense
                {
                    if (tortShellPostHit == 0)
                    {
                        float numberOfDusts = 43f;
                        float rotFactor = 360f / numberOfDusts;
                        for (int i = 0; i < numberOfDusts; i++)
                        {
                            float rot = MathHelper.ToRadians(i * rotFactor);
                            Vector2 offset = new Vector2(Main.rand.NextFloat(0.5f, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                            Vector2 velOffset = new Vector2(Main.rand.NextFloat(0.5f, 3.1f), 0).RotatedBy(rot * Main.rand.NextFloat(1.1f, 9.1f));
                            Dust dust = Dust.NewDustPerfect(Player.Center + offset, Main.rand.NextBool() ? 215 : 22, new Vector2(velOffset.X, velOffset.Y));
                            dust.noGravity = false;
                            dust.velocity = velOffset;
                            dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                        }
                    }
                    tortShellPostHit = 300;
                }

                if (abyssalDivingSuitPlates && hurtInfo.Damage > 50)
                {
                    if (abyssalDivingSuitPlateHits < 3)
                        abyssalDivingSuitPlateHits++;

                    bool plateCDExists = cooldowns.TryGetValue(DivingPlatesBreaking.ID, out CooldownInstance plateDurability);
                    if (plateCDExists)
                        plateDurability.timeLeft = abyssalDivingSuitPlateHits;

                    if (abyssalDivingSuitPlateHits >= 3)
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath14, Player.Center);

                        if (plateCDExists)
                            cooldowns.Remove(DivingPlatesBreaking.ID);

                        Player.AddCooldown(DivingPlatesBroken.ID, 10830);

                        for (int d = 0; d < 20; d++)
                        {
                            int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int d = 0; d < 35; d++)
                        {
                            int fire = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                            Main.dust[fire].noGravity = true;
                            Main.dust[fire].velocity *= 5f;
                            fire = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                            Main.dust[fire].velocity *= 2f;
                        }
                    }
                }

                if (aquaticHeartIce)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath7, Player.Center);
                    Player.AddCooldown(AquaticHeartIceShield.ID, CalamityUtils.SecondsToFrames(30));

                    for (int d = 0; d < 10; d++)
                    {
                        int ice = Dust.NewDust(Player.position, Player.width, Player.height, DustID.IceRod, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[ice].scale = 0.5f;
                            Main.dust[ice].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int d = 0; d < 15; d++)
                    {
                        int ice = Dust.NewDust(Player.position, Player.width, Player.height, DustID.IceRod, 0f, 0f, 100, default, 3f);
                        Main.dust[ice].noGravity = true;
                        Main.dust[ice].velocity *= 5f;
                        ice = Dust.NewDust(Player.position, Player.width, Player.height, DustID.IceRod, 0f, 0f, 100, default, 2f);
                        Main.dust[ice].velocity *= 2f;
                    }
                }

                if (tarraMelee)
                {
                    Player.AddBuff(ModContent.BuffType<TarraLifeRegen>(), 120);
                }
                else if (xerocSet)
                {
                    Player.AddBuff(ModContent.BuffType<EmpyreanRage>(), 180);
                    Player.AddBuff(ModContent.BuffType<EmpyreanWrath>(), 180);
                }
                else if (reaverDefense)
                {
                    Player.AddBuff(ModContent.BuffType<ReaverRage>(), 180);
                }

                if (fBarrier || (aquaticHeart && NPC.downedBoss3))
                {
                    SoundEngine.PlaySound(SoundID.Item27, Player.Center);
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;

                        float npcDist = (npc.Center - Player.Center).Length();
                        float freezeDist = 300 + (int)hurtInfo.Damage * 2;
                        if (freezeDist > 500f)
                            freezeDist = 500f + (freezeDist - 500f) * 0.5f;

                        if (npcDist < freezeDist)
                        {
                            float duration = Main.rand.Next(10 + (int)hurtInfo.Damage / 2, 20 + (int)hurtInfo.Damage);
                            if (duration > 120)
                                duration = 120;

                            npc.AddBuff(ModContent.BuffType<GlacialState>(), (int)duration, false);
                        }
                    }
                }

                // By setting brainOfConfusionItem, these accessories have this code already,
                // but doing it again allows for increased duration + The Amalgam's other buffs,
                // and also doesn't have random chance (why does Brain of Confusion not guarantee confusion on hit)
                if (aBrain || amalgam)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.dontTakeDamage)
                            continue;

                        float npcDist = (npc.Center - Player.Center).Length();
                        float range = Main.rand.Next(200 + (int)hurtInfo.Damage / 2, 301 + (int)hurtInfo.Damage * 2);
                        if (range > 500f)
                            range = 500f + (range - 500f) * 0.75f;
                        if (range > 700f)
                            range = 700f + (range - 700f) * 0.5f;
                        if (range > 900f)
                            range = 900f + (range - 900f) * 0.25f;

                        if (npcDist < range)
                        {
                            float duration = Main.rand.Next(300 + (int)hurtInfo.Damage / 3, 480 + (int)hurtInfo.Damage / 2);
                            npc.AddBuff(BuffID.Confused, (int)duration, false);
                            if (amalgam)
                            {
                                npc.AddBuff(BuffID.Venom, (int)duration);
                                npc.AddBuff(ModContent.BuffType<Plague>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), (int)duration);
                                npc.AddBuff(ModContent.BuffType<Irradiated>(), (int)duration);
                            }
                        }
                    }

                    // Spawn the harmless brain images that are actually projectiles
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<TheAmalgam>()));
                    Projectile.NewProjectile(source, Player.Center.X + Main.rand.Next(-40, 40), Player.Center.Y - Main.rand.Next(20, 60), Player.velocity.X * 0.3f, Player.velocity.Y * 0.3f, ProjectileID.BrainOfConfusion, 0, 0f, Player.whoAmI);
                }
            }

            if (Player.ownedProjectileCounts[ModContent.ProjectileType<DrataliornusBow>()] != 0)
            {
                foreach(Projectile p in Main.ActiveProjectiles)
                {
                    if (p.type == ModContent.ProjectileType<DrataliornusBow>() && p.owner == Player.whoAmI)
                    {
                        p.Kill();
                        break;
                    }
                }

                if (Player.wingTime > Player.wingTimeMax / 2)
                    Player.wingTime = Player.wingTimeMax / 2;
            }

            // Bone Wings: Getting hit halves current flight time
            if (Player.wingsLogic == (int)VanillaWingID.BoneWings)
            {
                // Drop some bones for visual effects
                if (Main.netMode != NetmodeID.Server && Player.wingTime > 0)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ItemID.BoneWings));
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 boneVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(1.5f, 2.5f);
                        Gore bone = Gore.NewGoreDirect(source, Player.Center, boneVelocity, 57, Main.rand.NextFloat(0.6f, 0.9f));
                        bone.timeLeft = Main.rand.Next(6, 30 + 1);
                    }
                }
                Player.wingTime /= 2;
            }
        }
        #endregion

        #region Post Hurt
        public override void PostHurt(Player.HurtInfo hurtInfo)
        {
            // Silver Armor medkit timer
            if (silverMedkit && hurtInfo.Damage >= SilverArmorSetChange.SetBonusMinimumDamageToHeal)
                silverMedkitTimer = SilverArmorSetChange.SetBonusHealTime;

            // Handle hit effects from the gem tech armor set.
            Player.Calamity().GemTechState.PlayerOnHitEffects((int)hurtInfo.Damage);

            if (Player.whoAmI == Main.myPlayer)
            {
                // Add extra iframes on hit based on various Calamity effects.
                int iFramesToAdd = Player.GetExtraHitIFrames(hurtInfo);

                // Give bonus immunity frames based on the type of damage dealt
                if (hurtInfo.CooldownCounter != -1)
                    Player.hurtCooldowns[hurtInfo.CooldownCounter] += iFramesToAdd;
                else
                    Player.immuneTime += iFramesToAdd;

                if (aeroSet && hurtInfo.Damage > 25)
                {
                    // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                    var source = Player.GetSource_OnHurt(hurtInfo.DamageSource, AerospecBreastplate.FeatherEntitySourceContext);
                    for (int n = 0; n < 4; n++)
                    {
                        int featherDamage = (int)Player.GetBestClassDamage().ApplyTo(35);
                        featherDamage = Player.ApplyArmorAccDamageBonusesTo(featherDamage);

                        CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 20f, ModContent.ProjectileType<StickyFeatherAero>(), featherDamage, 1f, Player.whoAmI);
                    }
                }
                if (hideOfDeus)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<HideofAstrumDeus>()));
                    SoundEngine.PlaySound(SoundID.Item74, Player.Center);

                    int blazeDamage = (int)Player.GetBestClassDamage().ApplyTo(25);
                    blazeDamage = Player.ApplyArmorAccDamageBonusesTo(blazeDamage);

                    int astralStarDamage = (int)Player.GetBestClassDamage().ApplyTo(320);
                    astralStarDamage = Player.ApplyArmorAccDamageBonusesTo(astralStarDamage);

                    Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, 0f, 0f, ModContent.ProjectileType<HideOfAstrumDeusExplosion>(), blazeDamage, 5f, Player.whoAmI, 0f, 1f);
                    for (int n = 0; n < 12; n++)
                    {
                        CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 29f, ModContent.ProjectileType<AstralStar>(), astralStarDamage, 5f, Player.whoAmI);
                    }
                }
                // TODO -- Make Deific Amulet and Rampart of Deities' retaliation effects way cooler
                if (dAmulet)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<DeificAmulet>()));
                    for (int n = 0; n < 3; n++)
                    {
                        int deificStarDamage = (int)Player.GetBestClassDamage().ApplyTo(130);
                        deificStarDamage = Player.ApplyArmorAccDamageBonusesTo(deificStarDamage);

                        Projectile star = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 29f, ProjectileID.StarVeilStar, deificStarDamage, 4f, Player.whoAmI);
                        if (star.whoAmI.WithinBounds(Main.maxProjectiles))
                        {
                            star.DamageType = DamageClass.Generic;
                            star.usesLocalNPCImmunity = true;
                            star.localNPCHitCooldown = 5;
                        }
                    }
                }
                if (aSpark)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<HideofAstrumDeus>()));
                    if (hurtInfo.Damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item93, Player.Center);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;

                        // Start with base damage, then apply the best damage class you can
                        int sDamage = 6;
                        if (transformer)
                            sDamage += 42;
                        sDamage = (int)Player.GetBestClassDamage().ApplyTo(sDamage);
                        sDamage = Player.ApplyArmorAccDamageBonusesTo(sDamage);

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int spark1 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                int spark2 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<Spark>(), sDamage, 1.25f, Player.whoAmI, 0f, 0f);
                                if (spark1.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark1].timeLeft = 120;
                                    Main.projectile[spark1].DamageType = DamageClass.Generic;
                                }
                                if (spark2.WithinBounds(Main.maxProjectiles))
                                {
                                    Main.projectile[spark2].timeLeft = 120;
                                    Main.projectile[spark2].DamageType = DamageClass.Generic;
                                }
                            }
                        }
                    }
                }
                if (inkBomb && !abyssalMirror && !eclipseMirror)
                {
                    var source = Player.GetSource_Accessory(FindAccessory(ModContent.ItemType<Items.Accessories.InkBomb>()));
                    if (Player.whoAmI == Main.myPlayer && !Player.HasCooldown(Cooldowns.InkBomb.ID))
                    {
                        Player.AddCooldown(Cooldowns.InkBomb.ID, CalamityUtils.SecondsToFrames(20));
                        rogueStealth += 0.5f;
                        for (int i = 0; i < 3; i++)
                        {
                            SoundEngine.PlaySound(SoundID.Item61, Player.Center);
                            int ink = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-0f, -4f), ModContent.ProjectileType<InkBombProjectile>(), 0, 0, Player.whoAmI);
                            if (ink.WithinBounds(Main.maxProjectiles))
                                Main.projectile[ink].DamageType = DamageClass.Generic;
                        }
                    }
                }
                if (ataxiaBlaze)
                {
                    var fuckYouBitch = Player.GetSource_Misc("21");
                    if (hurtInfo.Damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item74, Player.Center);
                        int eDamage = (int)Player.GetBestClassDamage().ApplyTo(230);
                        eDamage = Player.ApplyArmorAccDamageBonusesTo(eDamage);

                        if (Player.whoAmI == Main.myPlayer)
                            Projectile.NewProjectile(fuckYouBitch, Player.Center, Vector2.Zero, ModContent.ProjectileType<DeepseaBlaze>(), eDamage, 1f, Player.whoAmI, 0f, 0f);
                    }
                }
                else if (daedalusShard) // Daedalus Ranged helm
                {
                    var source = Player.GetSource_Misc("22");
                    if (hurtInfo.Damage > 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item27, Player.Center);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int sDamage = (int)Player.GetTotalDamage<RangedDamageClass>().ApplyTo(60);
                        sDamage = Player.ApplyArmorAccDamageBonusesTo(sDamage);

                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float randomSpeed = Main.rand.Next(1, 7);
                                float randomSpeed2 = Main.rand.Next(1, 7);
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                int shard = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, ProjectileID.CrystalShard, sDamage, 1f, Player.whoAmI, 0f, 0f);
                                int shard2 = Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, ProjectileID.CrystalShard, sDamage, 1f, Player.whoAmI, 0f, 0f);
                                if (shard.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard].DamageType = DamageClass.Generic;
                                if (shard2.WithinBounds(Main.maxProjectiles))
                                    Main.projectile[shard2].DamageType = DamageClass.Generic;
                            }
                        }
                    }
                }
                else if (reaverDefense) //Defense and DR Helm
                {
                    var source = Player.GetSource_Misc("23");
                    if (hurtInfo.Damage > 0)
                    {
                        int rDamage = (int)Player.GetBestClassDamage().ApplyTo(240);
                        rDamage = Player.ApplyArmorAccDamageBonusesTo(rDamage);

                        if (Player.whoAmI == Main.myPlayer)
                            Projectile.NewProjectile(source, Player.Center.X, Player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ReaverThornBase>(), rDamage, 0f, Player.whoAmI, 0f, 0f);
                    }
                }
                else if (godSlayerDamage) //god slayer melee helm
                {
                    var source = Player.GetSource_Misc("24");
                    if (hurtInfo.Damage > 80)
                    {
                        SoundEngine.PlaySound(SoundID.Item73, Player.Center);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(Player.velocity.X, Player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int shrapnelDamage = Player.ApplyArmorAccDamageBonusesTo(Player.CalcIntDamage<MeleeDamageClass>(675));
                        if (Player.whoAmI == Main.myPlayer)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                                Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), shrapnelDamage, 5f, Player.whoAmI, 0f, 0f);
                                Projectile.NewProjectile(source, Player.Center.X, Player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GodKiller>(), shrapnelDamage, 5f, Player.whoAmI, 0f, 0f);
                            }
                        }
                    }
                }
                else if (dsSetBonus)
                {
                    if (Player.whoAmI == Main.myPlayer)
                    {
                        // https://github.com/tModLoader/tModLoader/wiki/IEntitySource#detailed-list
                        var source = Player.GetSource_OnHurt(hurtInfo.DamageSource, DemonshadeHelm.ShadowScytheEntitySourceContext);
                        for (int l = 0; l < 2; l++)
                        {
                            int shadowbeamDamage = (int)Player.GetBestClassDamage().ApplyTo(3000);
                            shadowbeamDamage = Player.ApplyArmorAccDamageBonusesTo(shadowbeamDamage);

                            Projectile beam = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.ShadowBeamFriendly, shadowbeamDamage, 7f, Player.whoAmI);
                            if (beam.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                beam.DamageType = DamageClass.Generic;
                                beam.usesLocalNPCImmunity = true;
                                beam.localNPCHitCooldown = 10;
                            }
                        }
                        for (int l = 0; l < 5; l++)
                        {
                            int scytheDamage = (int)Player.GetBestClassDamage().ApplyTo(5000);
                            scytheDamage = Player.ApplyArmorAccDamageBonusesTo(scytheDamage);

                            Projectile scythe = CalamityUtils.ProjectileRain(source, Player.Center, 400f, 100f, 500f, 800f, 22f, ProjectileID.DemonScythe, scytheDamage, 7f, Player.whoAmI);
                            if (scythe.whoAmI.WithinBounds(Main.maxProjectiles))
                            {
                                scythe.DamageType = DamageClass.Generic;
                                scythe.usesLocalNPCImmunity = true;
                                scythe.localNPCHitCooldown = 10;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Kill Player
        public void KillPlayer()
        {
            var source = Player.GetSource_Death();
            Player.lastDeathPostion = Player.Center;
            Player.lastDeathTime = DateTime.Now;
            Player.showLastDeath = true;
            int coinsOwned = (int)Utils.CoinsCount(out bool flag, Player.inventory, new int[0]);
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.lostCoins = coinsOwned;
                Player.lostCoinString = Main.ValueToCoins(Player.lostCoins);
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Main.mapFullscreen = false;
            }
            if (Main.myPlayer == Player.whoAmI)
            {
                Player.trashItem.SetDefaults(0, false);
                if (Player.difficulty == PlayerDifficultyID.SoftCore || Player.difficulty == PlayerDifficultyID.Creative)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (Player.inventory[i].stack > 0 && ((Player.inventory[i].type >= ItemID.LargeAmethyst && Player.inventory[i].type <= ItemID.LargeDiamond) || Player.inventory[i].type == ItemID.LargeAmber))
                        {
                            int droppedLargeGem = Item.NewItem(source, (int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, Player.inventory[i].type, 1, false, 0, false, false);
                            Main.item[droppedLargeGem].netDefaults(Player.inventory[i].netID);
                            Main.item[droppedLargeGem].Prefix((int)Player.inventory[i].prefix);
                            Main.item[droppedLargeGem].stack = Player.inventory[i].stack;
                            Main.item[droppedLargeGem].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[droppedLargeGem].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[droppedLargeGem].noGrabDelay = 100;
                            Main.item[droppedLargeGem].favorited = false;
                            Main.item[droppedLargeGem].newAndShiny = false;
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, droppedLargeGem, 0f, 0f, 0f, 0, 0, 0);
                            }
                            Player.inventory[i].SetDefaults(0, false);
                        }
                    }
                }
                else if (Player.difficulty == PlayerDifficultyID.MediumCore)
                {
                    Player.DropItems();
                }
                else if (Player.difficulty == PlayerDifficultyID.Hardcore)
                {
                    Player.DropItems();
                    Player.KillMeForGood();
                }
            }
            SoundEngine.PlaySound(SoundID.PlayerKilled, Player.Center);
            Player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            Player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            Player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            Player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            if (Player.stoned)
            {
                Player.headPosition = Vector2.Zero;
                Player.bodyPosition = Vector2.Zero;
                Player.legPosition = Vector2.Zero;
            }
            for (int j = 0; j < 100; j++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.LifeDrain, (float)(2 * 0), -2f, 0, default, 1f);
            }
            Player.mount.Dismount(Player);
            Player.dead = true;
            Player.respawnTimer = 600;
            if (Main.expertMode)
            {
                Player.respawnTimer = (int)(Player.respawnTimer * 1.5);
            }
            Player.immuneAlpha = 0;
            Player.palladiumRegen = false;
            Player.iceBarrier = false;
            Player.crystalLeaf = false;

            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(Player.Male ? 14 : 15);
            if (abyssDeath)
            {
                SoundEngine.PlaySound(DrownSound, Player.Center);
                damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AbyssDrown" + Main.rand.Next(1, 3 + 1)).Format(Player.name));
            }
            else if (CalamityWorld.armageddon && areThereAnyDamnBosses)
            {
                damageSource = PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Armageddon").Format(Player.name));
            }

            NetworkText deathText = damageSource.GetDeathText(Player.name);
            if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
            {
                NetMessage.SendPlayerDeath(Player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
            }
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(deathText, new Color(225, 25, 25));
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(deathText.ToString(), 225, 25, 25);
            }

            if (Player.whoAmI == Main.myPlayer && (Player.difficulty == PlayerDifficultyID.SoftCore || Player.difficulty == PlayerDifficultyID.Creative))
            {
                Player.DropCoins();
            }
            Player.DropTombstone(coinsOwned, deathText, 0);

            if (Player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Defense Damage Functions
        /// <summary>
        /// Deals Calamity defense damage to a player the "normal way", using an incoming hit.<br />
        /// This is the convenience function which follows all standard Calamity balancing rules for taking a regular hit.
        /// </summary>
        /// <param name="hurtInfo">HurtInfo of the incoming strike to the player.</param>
        public void DealDefenseDamage(Player.HurtInfo hurtInfo)
        {
            // Legacy safeguard: Skip defense damage if the player is somehow "hit for zero" (this should never happen).
            if (hurtInfo.Damage <= 0 || hurtInfo.SourceDamage <= 0)
                return;

            // Under typical circumstances, defense damage scales with "net mitigation", aka how much damage the player DIDN'T take.
            // Thematically, this means it scales with how much damage the player's defense took instead of them.
            int netMitigation = hurtInfo.SourceDamage - hurtInfo.Damage;
            int incomingDamageToUse = netMitigation <= 0 ? 0 : netMitigation;

            // Leave it to the direct function to determine how much defense damage is taken. Use standard ratios.
            DealDefenseDamage(incomingDamageToUse, false);
        }

        /// <summary>
        /// Deals Calamity defense damage to a player, specifically built to handle Chalice of the Blood God's bleedout.
        /// </summary>
        /// <param name="hurtInfo">HurtInfo of the incoming strike to the player.</param>
        /// <param name="bleedoutApplied">The bleedout applied on this specific hit. Used for reducing the defense damage inflicted.</param>
        public void DealDefenseDamage(Player.HurtInfo hurtInfo, int bleedoutApplied)
        {
            // Legacy safeguard: Skip defense damage if the player is somehow "hit for zero" (this should never happen).
            if (hurtInfo.Damage <= 0 || hurtInfo.SourceDamage <= 0)
                return;

            // Under typical circumstances, defense damage scales with "net mitigation", aka how much damage the player DIDN'T take.
            // Thematically, this means it scales with how much damage the player's defense took instead of them.
            // Chalice of the Blood God makes you take much less direct damage than you should, which would catastrophically inflate defense damage.
            //
            // Subtract the bleedout applied on this hit from the net mitigation.
            // This prevents Chalice from making the player take much more defense damage than intended.
            int netMitigation = hurtInfo.SourceDamage - (hurtInfo.Damage + bleedoutApplied);
            int incomingDamageToUse = netMitigation <= 0 ? 0 : netMitigation;

            // Leave it to the direct function to determine how much defense damage is taken. Use standard ratios.
            DealDefenseDamage(incomingDamageToUse, false);
        }

        /// <summary>
        /// Deals Calamity defense damage to a player. This is the direct function, for unusual sources of defense damage.
        /// </summary>
        /// <param name="incomingDamage">The amount of defense damage to deal.</param>
        /// <param name="absolute">If true, deals exactly the specified defense damage, ignoring the standard ratios and Draedon's Heart.<br />
        /// Setting this to false is equivalent to considering the first parameter as standard incoming damage to the player.<br />
        /// Setting this to true bypasses the defense damage floor, and can thus inflict less defense damage than is typically allowed.</param>
        public void DealDefenseDamage(int incomingDamage, bool absolute = false)
        {
            // If absolute is specified, then ignore the ratio and always inflict EXACTLY THAT MUCH defense damage.
            // This means it bypasses Draedon's Heart!
            double ratioToUse = absolute ? 1D : defenseDamageRatio;

            // Intended amount of defense damage to take. Can round up, but can also be overwritten by the floor.
            int defenseDamageTaken = (int)Math.Round(incomingDamage * ratioToUse);

            // There is a floor on defense damage based on difficulty; i.e. there is a minimum amount of defense damage from any hit that can deal defense damage.
            // This floor is only applied if bosses are alive, but is bypassed by the absolute flag.
            // Details on the floor can be seen in the BalancingConstants file.
            bool useDefenseDamageFloor = areThereAnyDamnBosses && !absolute;
            if (useDefenseDamageFloor)
            {
                int defenseDamageFloor = CalamityUtils.GetDefenseDamageFloor();

                // Apply floor
                if (defenseDamageTaken < defenseDamageFloor)
                    defenseDamageTaken = defenseDamageFloor;
            }

            // The amount of defense damage taken is now final. Apply it.
            ApplyDefenseDamageInternal(defenseDamageTaken);
        }

        // Actually applies defense damage. Cannot be called externally.
        private void ApplyDefenseDamageInternal(int defenseDamage, bool showVisuals = true)
        {
            // If zero defense damage is being dealt, don't waste your time or display a grey 0.
            if (defenseDamage <= 0)
                return;
            
            // Can be dynamically reduced by Adamantite set bonus and maybe other future effects.
            int defenseDamageTaken = defenseDamage;

            // Apply incoming defense damage to the Adamantite armor set bonus.
            if (AdamantiteSetDefenseBoost > 0)
            {
                int defenseDamageToAdamantite = Math.Min(AdamantiteSetDefenseBoost, defenseDamageTaken);
                AdamantiteSetDefenseBoost -= defenseDamageToAdamantite;

                // Reduce remaining defense damage by whatever was applied to Adamantite armor.
                defenseDamageTaken -= defenseDamageToAdamantite;

                // If Adamantite Armor's set bonus entirely absorbed the defense damage, then display the number and play the sound,
                // but don't actually reduce defense or trigger the defense damage recovery cooldown.
                if (defenseDamageTaken <= 0)
                {
                    ShowDefenseDamageEffects(defenseDamageToAdamantite);
                    return;
                }
            }

            // Apply incoming defense damage on top of whatever defense damage the player currently has.
            int previousDefenseDamage = CurrentDefenseDamage;
            totalDefenseDamage = previousDefenseDamage + defenseDamageTaken;

            // Safety check to prevent illegal recovery time
            if (defenseDamageRecoveryFrames < 0)
                defenseDamageRecoveryFrames = 0;

            // Directly add the base defense damage recovery time to whatever recovery time the player already has.
            totalDefenseDamageRecoveryFrames = defenseDamageRecoveryFrames + DefenseDamageBaseRecoveryTime;
            if (totalDefenseDamageRecoveryFrames > DefenseDamageMaxRecoveryTime)
                totalDefenseDamageRecoveryFrames = DefenseDamageMaxRecoveryTime;

            // Reset any recovery progress they may have already made.
            // They start the new recovery timer from the beginning.
            defenseDamageRecoveryFrames = totalDefenseDamageRecoveryFrames;

            // Reset the delay between iframes ending and defense damage recovery starting.
            defenseDamageDelayFrames = DefenseDamageRecoveryDelay;

            if (showVisuals)
                ShowDefenseDamageEffects(defenseDamage);
        }

        // Displays visuals for taking defense damage.
        private void ShowDefenseDamageEffects(int defenseDamage)
        {
            // Play a sound from taking defense damage.
            if (hurtSoundTimer == 0 && Main.myPlayer == Player.whoAmI)
            {
                double maxVolumeDefenseDamageScalar = Main.masterMode ? 0.7 : CalamityWorld.death ? 0.6 : CalamityWorld.revenge ? 0.55 : Main.expertMode ? 0.5 : 0.4;
                float maxVolumeDefenseDamage = (float)Math.Round(Player.statDefense * maxVolumeDefenseDamageScalar);
                float minVolume = 0.5f;
                float maxVolume = 1f;
                float lerpAmount = MathHelper.Clamp(defenseDamage / maxVolumeDefenseDamage, 0f, 1f);
                float defenseDamageSoundVolumeMultiplier = MathHelper.Lerp(minVolume, maxVolume, lerpAmount);
                SoundEngine.PlaySound(DefenseDamageSound with { Volume = DefenseDamageSound.Volume * defenseDamageSoundVolumeMultiplier }, Player.Center);
                hurtSoundTimer = 30;
            }

            // Display text indicating that defense damage was taken.
            string text = (-defenseDamage).ToString();
            Color messageColor = Color.LightGray;
            Rectangle location = new Rectangle((int)Player.position.X, (int)Player.position.Y - 16, Player.width, Player.height);
            CombatText.NewText(location, messageColor, Language.GetTextValue(text));
        }
        #endregion

        #region Adrenaline Loss Function
        /// <summary>
        /// Causes the player to lose Adrenaline based on an incoming hit. The behavior differs based on energy shields or Draedon's Heart.
        /// </summary>
        /// <param name="hurtInfo">The incoming damage event to the player.</param>
        /// <param name="fullyAbsorbedByShield">Whether or not the hit was fully absorbed by one or more energy shields. Tends to halve Adrenaline loss.</param>
        private void LoseAdrenalineOnHurt(Player.HurtInfo hurtInfo, bool fullyAbsorbedByShield = false)
        {
            // Being hit for zero from Paladin's Shield damage share has no effects on Adrenaline, regardless of all other circumstances.
            // Likewise, being struck while Adrenaline is actively burning has no effects on Adrenaline.
            if (hurtInfo.Damage <= 0 || adrenalineModeActive)
                return;

            // Draedon's Heart pauses for half the usual duration on a shield hit.
            // Otherwise, nothing happens here because no Adrenaline is actually lost.
            if (draedonsHeart)
            {
                int pauseTime = fullyAbsorbedByShield ? DraedonsHeart.NanomachinePauseAfterShieldDamage : DraedonsHeart.NanomachinePauseAfterDamage;
                adrenalinePauseTimer += pauseTime;
            }

            // Standard Adrenaline behavior
            else
            {
                // Regular Adrenaline pauses on any hit, even if you lose all Adrenaline.
                adrenalinePauseTimer += BalancingConstants.AdrenalinePauseAfterDamage;

                // Play a sound if Adrenaline was lost from full (this means Adrenaline DR helped mitigate the hit).
                // If this occurs, since Adrenaline DR helped mitigate the hit's damage, we can't allow the amount of Adrenaline lost to actually be reduced.
                bool hitAtFullAdrenaline = adrenaline >= adrenalineMax;
                if (hitAtFullAdrenaline)
                {
                    SoundEngine.PlaySound(AdrenalineHurtSound, Player.Center);
                    adrenaline = 0f;
                    return;
                }

                // 19MAR2024: Ozzatron: Chalice makes you lose adrenaline based on the damage you would have suffered in total.
                int damageToUse = hurtInfo.Damage;
                if (chaliceOfTheBloodGod && chaliceHitOriginalDamage > 0)
                {
                    damageToUse = chaliceHitOriginalDamage;
                    // Maybe at some point in the future, tracking this value will be useful elsewhere. Until then, it's only used here, so it is reset here.
                    chaliceHitOriginalDamage = 0;
                }

                // Calculate the amount of Adrenaline to lose. This is done in 3 steps:
                // 1. Find out how much %HP the player lost (or was absorbed by a shield).
                // 2. Use an inverse lerp to calculate the Adrenaline loss scaling down for very small hits (5% HP or less).
                // 3. Re-scale the lerp result into a % of Adrenaline loss from 10% (min loss) to 100%.
                float damageMaxHPRatio = (float)damageToUse / Player.statLifeMax2;
                float smallHitAdrenalineLossRatio = (float)Utils.GetLerpValue(0f, BalancingConstants.AdrenalineFalloffTinyHitHealthRatio, damageMaxHPRatio, true);
                float adrenalineLossFraction = MathHelper.Lerp(BalancingConstants.MinimumAdrenalineLoss, 1f, smallHitAdrenalineLossRatio);
                float adrenalineToLose = adrenaline * adrenalineLossFraction;

                // If the hit was fully absorbed by a shield, then lose half that much instead.
                if (fullyAbsorbedByShield)
                    adrenalineToLose /= 2f;

                // Actually subtract Adrenaline.
                adrenaline -= adrenalineToLose;
                if (adrenaline < 0f)
                    adrenaline = 0f;
            }
        }
        #endregion
    }
}
