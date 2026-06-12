using System;
using CalamityMod.Balancing;
using CalamityMod.Enums;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Shimmer Changes

        private static bool AdjustShimmerRequirements(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            //Rod of Harmony requires Draedong and SCal dead instead of Moon Lord.
            if (type == ItemID.RodofDiscord)
            {
                return !DownedBossSystem.downedCalamitas || !DownedBossSystem.downedExoMechs;
            }

            return orig(type);
        }

        #endregion

        #region Remove Soaring Insignia Infinite Flight
        private static void RemoveSoaringInsigniaInfiniteWingTime(ILContext il)
        {
            // Prevent the infinite flight effect.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")))
            {
                LogFailure("Soaring Insignia Infinite Flight Removal", "Could not locate the Soaring Insignia bool.");
                return;
            }

            // AND with 0 (false) so that the Soaring Insignia is never considered equipped and thus infinite flight never triggers.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Jump Height Changes
        private static void FixJumpHeightBoosts(ILContext il)
        {
            // Remove the code that makes Shiny Red Balloon SET jump height to a specific value to make balancing jump speed easier.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(20)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Shiny Red Balloon jump height assignment value.");
                return;
            }

            // Delete both the ldc.i4 20 AND the store that assigns it to Player.jumpHeight.
            cursor.RemoveRange(2);

            // Change the jump speed from Shiny Red Balloon to be an actual boost instead of a hardcoded replacement.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcR4(6.51f)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Shiny Red Balloon jump speed assignment value.");
                return;
            }

            // Replace the hardcoded 6.51 with a balanceable value in CalamityPlayer.
            cursor.Prev.Operand = BalancingConstants.BalloonJumpSpeedBoost;
            // Load the player's current jumpSpeed onto the stack and add the boost to it.
            cursor.Emit(OpCodes.Ldsfld, typeof(Player).GetField("jumpSpeed"));
            cursor.Emit(OpCodes.Add);

            // Find the Soaring Insignia jump speed bonus and reduce it to 0.5f.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.8f)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Soaring Insignia jump speed boost value.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.5f); // Decrease to 0.5f.
        }

        private const float VanillaBaseJumpSpeed = 5.01f;
        private static void BaseJumpSpeedAdjustment(ILContext il)
        {
            // Increase the base jump height of the player to make early game less of a slog.
            var cursor = new ILCursor(il);

            // The jumpSpeed variable is set to this specific value before anything else occurs.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(VanillaBaseJumpSpeed)))
            {
                LogFailure("Base Jump Height Buff", "Could not locate the jump height variable.");
                return;
            }
            cursor.Remove();

            // Increase by 10% if the higher jump speed is enabled.
            cursor.EmitDelegate<Func<float>>(() => CalamityServerConfig.Instance.FasterJumpSpeed ? BalancingConstants.ConfigBoostedBaseJumpSpeed : VanillaBaseJumpSpeed);
        }
        #endregion

        #region Run Speed Changes
        private static void RunSpeedAdjustments(ILContext il)
        {
            var cursor = new ILCursor(il);
            float asphaltTopSpeedMultiplier = 2.25f; // +125%. Vanilla is +250%
            float asphaltSlowdown = 1f; // Vanilla is 2f. This should actually make asphalt faster.

            // Multiplied by 0.6 on frozen slime, for +26% acceleration
            // Multiplied by 0.7 on ice, for +47% acceleration
            float iceSkateAcceleration = 2.1f;

            //
            // ASPHALT
            //
            {
                // Find the top speed multiplier of Asphalt.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Asphalt's top speed multiplier.");
                    return;
                }

                // Massively reduce the increased speed cap of Asphalt.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, asphaltTopSpeedMultiplier);

                // Find the run slowdown multiplier of Asphalt.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Asphalt's run slowdown multiplier.");
                    return;
                }

                // Reducing the slowdown actually makes the (slower) Asphalt more able to reach its top speed.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, asphaltSlowdown);
            }

            //
            // ICE SKATES + FROZEN SLIME BLOCKS
            //
            {
                // Find the acceleration multiplier of Ice Skates on Frozen Slime Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Frozen Slime Block acceleration multiplier.");
                    return;
                }

                // Massively reduce the acceleration bonus of Ice Skates on Frozen Slime Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateAcceleration);
            }

            //
            // ICE SKATES + ICE BLOCKS
            //
            {
                // Find the acceleration multiplier of Ice Skates on Ice Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3.5f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Ice Block acceleration multiplier.");
                    return;
                }

                // Massively reduce the acceleration bonus of Ice Skates on Ice Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateAcceleration);
            }
        }

        private static void NerfOverpoweredRunAccelerationSources(ILContext il)
        {
            // First: Soaring Insignia. Find the check for whether it's equipped for run speeds.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")))
            {
                LogFailure("Run Acceleration Nerfs", "Could not locate the Soaring Insignia bool.");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.75f)))
            {
                LogFailure("Run Acceleration Nerfs", "Could not locate the Soaring Insignia run acceleration multiplier.");
                return;
            }
            cursor.Next.Operand = BalancingConstants.SoaringInsigniaRunAccelerationMultiplier;

            // Second: Shadow Armor. Find the check for whether it's equipped for run speeds.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("shadowArmor")))
            {
                LogFailure("Run Acceleration Nerfs", "Could not locate the Shadow Armor bool.");
                return;
            }

            // Load the player onto the stack as an argument to the following delegate.
            // Emit a delegate which consumes the Shadow Armor bool, performs Calamity effects, then always returns false.
            // Returning false ensures vanilla Shadow Armor code never runs.
            cursor.Emit(OpCodes.Ldarg_0);

            cursor.EmitDelegate((bool shadowArmor, Player p) => {
                // If you don't even have Shadow Armor equipped, do nothing.
                if (!shadowArmor)
                    return 0;

                // Shadow Armor does not stack with Magiluminescence if you are on the ground.
                if (p.hasMagiluminescence && p.velocity.Y == 0)
                    return 0;

                // Shadow Armor grants reduced movement bonuses if in the air, or on the ground WITHOUT Magiluminescence.
                p.runAcceleration *= BalancingConstants.ShadowArmorRunAccelerationMultiplier;
                p.maxRunSpeed *= BalancingConstants.ShadowArmorMaxRunSpeedMultiplier;
                p.accRunSpeed *= BalancingConstants.ShadowArmorAccRunSpeedMultiplier;
                p.runSlowdown *= BalancingConstants.ShadowArmorRunSlowdownMultiplier;

                // Vanilla Shadow Armor behavior should still always be skipped.
                return 0;
            });


            // Finally: Back to Soaring Insignia. Prevent the rocket boots infinite flight effect, since it's in the same function.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")))
            {
                LogFailure("Run Acceleration Nerfs", "Could not locate the Soaring Insignia bool.");
                return;
            }

            // AND with 0 (false) so that the Soaring Insignia is never considered equipped and thus infinite rocket boots never triggers.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Life Regen Changes
        private static void UpdateLifeRegenBalancingChanges(ILContext il)
        {
            // This IL edit accomplishes two things related to life regen:
            // 1. Prevents Nebula armor's Life Boosters from cancelling out negative life regen.
            // 2. Prevents the greatly reduced life regen while without a Well Fed buff in Expert Mode.
            var cursor = new ILCursor(il);

            // First, move to where the game checks if the Life Booster level is greater than 0.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("nebulaLevelLife")))
            {
                LogFailure("Nebula Armor DoT Ignoring Nerf", "Could not locate the Nebula Armor Life Booster variable.");
                return;
            }

            // Pop this value off the stack and replace it with 0.
            // 0 will never be greater than 0, so negative life regen will never be canceled out.
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_0);

            // Now move to where the game checks if it is Expert Mode and the player does not have a Well Fed buff.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("wellFed")))
            {
                LogFailure("Expert Mode Well Fed Reduced Life Regen Prevention", "Could not locate the Well Fed bool.");
                return;
            }

            // OR with 1 (true) so that Well Fed is considered permanently active and reduced life regen never triggers.
            cursor.Emit(OpCodes.Ldc_I4_1);
            cursor.Emit(OpCodes.Or);
        }
        #endregion

        #region Mana Regen Changes
        private static void ManaRegenDelayAdjustment(ILContext il)
        {
            // Decrease the max mana regen delay so that mage is less annoying to play without mana regen buffs.
            // Decreases the max mana regen delay from a range of 31.5 - 199.5 to 4 - 52.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(45f))) // The flat amount added to max regen delay in the formula.
            {
                LogFailure("Max Mana Regen Delay Reduction", "Could not locate the max mana regen flat variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 20f); // Decrease to 20f.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.7f))) // The multiplier for max mana regen delay.
            {
                LogFailure("Max Mana Regen Delay Reduction", "Could not locate the max mana regen delay multiplier variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.2f); // Decrease to 0.2f.
        }

        private static void UpdateManaRegenBalancingChanges(ILContext il)
        {
            // This IL edit accomplishes two things:
            // 1. Nerfs Nebula armor's Mana Boosters.
            // 2. Increases the base mana regen so that mage is less annoying to play without mana regen buffs.
            var cursor = new ILCursor(il);

            // Reduce Nebula armor mana regen.
            // See BalancingConstants for a more in-depth explanation as to how the Mana Booster works.
            // Just know that we need to raise this value in order to nerf it.
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcI4(6)))
            {
                LogFailure("Nebula Armor Mana Regen Nerf", "Could not locate the Nebula Armor mana regeneration frame counter threshold.");
                return;
            }

            // Swap the threshold with Calamity's value.
            cursor.Next.Operand = BalancingConstants.NebulaManaRegenFrameCounterThreshold;

            // Next, move to the mana regen formula.
            // The multiplier for the mana regen formula: (float)statMana / (float)statManaMax2 * 0.8f + 0.2f.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.8f)))
            {
                LogFailure("Mana Regen Buff", "Could not locate the mana regen multiplier variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.25f); // Decrease to 0.25f.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.2f))) // The flat added mana regen amount.
            {
                LogFailure("Mana Regen Buff", "Could not locate the flat mana regen variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.75f); // Increase to 0.75f.
        }
        #endregion

        #region Damage Variance Dampening and Luck Removal
        private static int AdjustDamageVariance(On_Main.orig_DamageVar_float_int_float orig, float dmg, int percent, float luck)
        {
            // Change the default damage variance from +-15% to +-5%.
            // If other mods decide to change the scale, they can override this. We're solely killing the default value.
            if (percent == Main.DefaultDamageVariationPercent)
                percent = BalancingConstants.NewDefaultDamageVariationPercent;
            // Remove the ability for luck to affect damage variance by setting it to 0 always.
            return orig(dmg, percent, 0f);
        }
        #endregion

        #region Expert Hardmode Scaling Removal
        private static void RemoveExpertHardmodeScaling(ILContext il)
        {
            // Completely disable the weak enemy scaling that occurs when Hardmode is active in Expert Mode.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1000))) // The less than 1000 HP check in order for the scaling to take place.
            {
                LogFailure("Expert Hardmode Scaling Removal", "Could not locate the HP check.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_M1); // Replace the 1000 with -1, no NPC can have less than -1 HP on spawn, so it fails to run.
        }
        #endregion

        #region Vanilla Boss Resist Changes
        private static void VanillaBossResistChanges(ILContext il)
        {
            // This IL edit accomplishes two things:
            // 1. Reduces Expert+ Eater of Worlds' resist to explosives from 80% to 66%.
            // 2. Effectively removes Lunatic Cultist's resistance to homing projectiles.
            var cursor = new ILCursor(il);

            // First, the EoW grenade resist. Naturally, this is 800 lines into Projectile.Damage, so we must do funky things.
            for (int f = 0; f < 2; f++)
            {
                if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(1002)))
                {
                    LogFailure("Reduce EoW Grenade Resist", "Could not move to the resist factor.");
                    return;
                }
            }
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcR4(5f)))
            {
                LogFailure("Reduce EoW Grenade Resist", "Could not move to the resist factor.");
                return;
            }

            // Pop the 5 off the stack, and replace it with a 3.
            cursor.EmitPop();
            cursor.EmitLdcR4(3f);

            // Now, move to the Cultist resist which is right below it.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(typeof(ProjectileID.Sets), "CultistIsResistantTo")))
            {
                LogFailure("Lunatic Cultist Homing Resist Removal", "Could not locate the Cultist resist set.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.75f))) // The resist ratio.
            {
                LogFailure("Lunatic Cultist Homing Resist Removal", "Could not locate the resist percentage.");
                return;
            }

            // Replace the value with 1, meaning -0% damage or no resist.
            cursor.Next.Operand = 1f;
        }
        #endregion

        #region Flail Balance Changes
        // Make flails be unaffected by player velocity
        private static void FlailsNoLongerAffectedByPlayerVelocity(On_Projectile.orig_AI_015_Flails orig, Projectile self)
        {
            orig(self);
            if (self.ai[0] == 1f && self.ai[1] == 0f)
                self.velocity -= Main.player[self.owner].velocity;
        }

        // Increase Flower Pow's return speed
        private static void IncreaseFlowerPowRetSpeed(ILContext il)
        {
            var cursor = new ILCursor(il);
            // Move to where Flower Pow receives its specific flail stat changes. This moves it after num2 = 23f
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcR4(23f), i => i.MatchStloc(6)))
            {
                LogFailure("Flower Pow Buff", "Could not move to Flower Pow's specific flail stat changes.");
                return;
            }

            // Emit an instruction setting num5 (the return speed) to 21. Vanilla is 16
            cursor.Emit(OpCodes.Ldc_R4, 21f);
            cursor.Emit(OpCodes.Stloc, 9);
        }
        #endregion

        #region Terrarian Projectile Limitation for Extra Updates
        private static void LimitTerrarianProjectiles(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(ProjectileID.Terrarian)))
            {
                LogFailure("Limit Terrarian Yoyo Projectiles", "Could not locate the yoyo ID.");
                return;
            }

            // Emit a delegate which corrupts the projectile ID checked for if the projectile is not on its final extra update.
            // This delegate intentionally eats the original ID off the stack and gives it back if finished.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate((int x, Projectile p) => p.FinalExtraUpdate() ? x : int.MinValue);
        }
        #endregion

        #region UpdateBuffs Balancing Changes
        private static void UpdateBuffsBalancingChanges(ILContext il)
        {
            // This IL edit accomplishes three things:
            // 1. Nerf Beetle Scale Mail's set bonus Beetle Might melee speed from 10% per stack to 5%.
            // 2. Nerf Nebula armor's Damage and Life Boosters (Mana Boosters are not handled in this method).
            // 3. Remove the vanilla implementation of Feral Bite inflicting random debuffs.
            var cursor = new ILCursor(il);

            // First, move to Beetle Scale Mail's melee speed boost from Beetle Might buff.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(BuffID.BeetleMight1)))
            {
                LogFailure("Beetle Scale Mail Nerf", "Could not locate the Beetle Might buff ID.");
                return;
            }
            for (int i = 0; i < 2; i++)
            {
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.1f))) // The amount of melee damage to grant.
                {
                    LogFailure("Beetle Scale Mail Nerf", "Could not locate the amount of melee speed granted.");
                    return;
                }
            }

            // Replace the value entirely.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, BalancingConstants.BeetleScaleMailMeleeSpeedPerBeetle);

            // Then, Nebula armor's Life and Damage Boosters.
            // First is the Life Boosters.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(BuffID.NebulaUpLife1)))
            {
                LogFailure("Nebula Armor Nerf", "Could not locate the Nebula Life buff ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("lifeRegen")))
            {
                LogFailure("Nebula Armor Nerf", "Could not locate the player's life regen being loaded.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(6)))
            {
                LogFailure("Nebula Armor Nerf", "Could not locate the amount of life regen to grant.");
                return;
            }

            // Replace the constant "load 6" opcode with a regular integer load with Calamity's value.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, BalancingConstants.NebulaLifeRegenPerBooster);

            // And then the Damage Boosters.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(BuffID.NebulaUpDmg1)))
            {
                LogFailure("Nebula Armor Nerf", "Could not locate the Nebula Damage buff ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcR4(0.15f)))
            {
                LogFailure("Nebula Armor Nerf", "Could not locate the amount of damage to grant.");
                return;
            }

            // There are multiple branches pointing to this instruction, so it cannot be removed. Instead, swap its value directly.
            cursor.Next.Operand = BalancingConstants.NebulaDamagePerBooster;

            // Finally, removing Feral Bite's vanilla debuff infliction.
            // Find the random debuff duration multiplier for the debuffs inflicted by Feral Bite.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.01f))) // The 0.01f random debuff duration multiplier.
            {
                LogFailure("Remove Feral Bite Random Debuffs", "Could not locate the Feral Bite random debuff duration multiplier.");
                return;
            }

            // Remove and change to 0f, this makes the random debuffs from Feral Bite have 0 duration.
            // Calamity reimplements a different version of this in CalamityGlobalBuff.Update
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0f);
        }
        #endregion

        #region Shield of Cthulhu Buffs
        private static void DashMovementEdits(On_Player.orig_DashMovement orig, Player self)
        {
            //This is a modified version of Vanilla's Shield of Cthulhu dash collision checks
            //This is done to be able to adjust values as needed. Here we change the iframe amount and recoil velocity
            if (self.dash == 2 && self.eocDash > 0 && self.eocHit < 0)
            {
                Rectangle DashHitbox = new Rectangle((int)(self.position.X + self.velocity.X * 0.5 - 4.0), (int)(self.position.Y + self.velocity.Y * 0.5 - 4.0), self.width + 8, self.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    NPC hitNPC = Main.npc[i];
                    if (!hitNPC.active || hitNPC.dontTakeDamage || hitNPC.friendly || (hitNPC.aiStyle == NPCAIStyleID.Fairy && !(hitNPC.ai[2] <= 1f)) || !self.CanNPCBeHitByPlayerOrPlayerProjectile(hitNPC))
                    {
                        continue;
                    }
                    Rectangle npcHitbox = hitNPC.getRect();
                    if (DashHitbox.Intersects(npcHitbox) && (hitNPC.noTileCollide || self.CanHit(hitNPC)))
                    {
                        float dmg = self.GetTotalDamage(DamageClass.Melee).ApplyTo(self.Calamity().copyrightInfringementShield ? 300f : 30f);
                        float kb = self.GetTotalKnockback(DamageClass.Melee).ApplyTo(self.Calamity().copyrightInfringementShield ? 12f : 9f);
                        bool crit = false;
                        if (Main.rand.Next(100) < self.GetTotalCritChance(DamageClass.Melee))
                        {
                            crit = true;
                        }
                        int direction = self.direction;
                        if (self.velocity.X < 0f)
                        {
                            direction = -1;
                        }
                        if (self.velocity.X > 0f)
                        {
                            direction = 1;
                        }
                        self.eocHit = i;
                        if (self.whoAmI == Main.myPlayer)
                        {
                            self.ApplyDamageToNPC(hitNPC, (int)dmg, kb, direction, crit, DamageClass.Melee);
                        }
                        self.eocDash = 10;
                        self.dashDelay = BalancingConstants.OnShieldBonkCooldown;
                        self.velocity.X = -direction * 9;
                        self.velocity.Y = -4f;
                        self.GiveImmuneTimeForCollisionAttack(8); //This is normally 4 in vanilla
                        int heldDir = 0;
                        if (self.controlLeft)
                            heldDir--;
                        if (self.controlRight)
                            heldDir++;
                        int dirSum = Math.Abs(direction + heldDir);
                        switch (dirSum)
                        {
                            case 0: //Holding in direction of recoil
                                self.velocity.X *= 1.75f;
                                break;
                            case 1: //Neutral direction
                                self.velocity.X *= 1.5f;
                                break;
                            case 2: //Holding in direction of enemy
                                self.velocity.X *= 1.25f;
                                break;
                        }
                    }
                }
            }
            orig(self);
        }
        #endregion

        #region Stardust Guardian Buffs
        private static void StardustGuardianAttackBuffs(ILContext il)
        {
            // Increase the Stardust Guardian's attack range while wearing Stardust Wings.
            var cursor = new ILCursor(il);

            // Move to the label after the instruction that sets num3 to 500f.
            for (int i = 0; i < 2; i++)
            {
                if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcR4(100f)))
                {
                    LogFailure("Stardust Guardian Buffs", "Could not move to after the Stardust Guardian's attack range.");
                    return;
                }
            }

            // Define a label for the branch statement.
            var label = il.DefineLabel();

            // Load Player.wingsLogic, and check if it's Stardust Wings.
            // If it is, increase the two attack range variables.
            cursor.Emit(OpCodes.Ldloc_0);
            cursor.Emit(OpCodes.Ldfld, typeof(Player).GetField("wingsLogic"));
            cursor.Emit(OpCodes.Ldc_I4, (int)VanillaWingID.WingsStardust);
            cursor.Emit(OpCodes.Bne_Un, label);

            cursor.Emit(OpCodes.Ldc_R4, 960f);
            cursor.Emit(OpCodes.Stloc, 4);
            cursor.Emit(OpCodes.Ldc_R4, 960f);
            cursor.Emit(OpCodes.Stloc, 5);

            cursor.MarkLabel(label);

            // Now, increase the Stardust Guardian's move speed when attacking targets.
            // This is applied regardless of having Stardust Wings.
            // This jumps to the base speed value when moving towards a target.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcR4(6f)))
            {
                LogFailure("Stardust Guardian Buffs", "Could not locate the Stardust Guardian's attack move speed.");
                return;
            }

            // Remove and replace with a higher value.
            cursor.EmitPop();
            cursor.Emit(OpCodes.Ldc_R4, 12f);
        }
        #endregion

        #region Solar Wings Change to Solar Flare Armor
        private static bool SolarWingsDashChange(On_Player.orig_ConsumeSolarFlare orig, Player self)
        {
            // Solar Wings restore flight time when Solar Flare armor's shield explodes
            // This can trigger from either ramming enemies or taking damage
            if (orig(self))
            {
                if (self.wingsLogic == (int)VanillaWingID.WingsSolar)
                    self.wingTime += 60;
                return true;
            }
            return false;
        }
        #endregion

        #region Remove Melee Armor (Beetle Shell + Solar Flare) Multiplicative DR
        private static void RemoveBeetleAndSolarFlareMultiplicativeDR(ILContext il)
        {
            // Remove the multiplicative DR from Solar Flare armor's Solar Shields
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("setSolar")))
            {
                LogFailure("Melee Multiplicative DR Removal", "Could not locate the Solar Flare set bonus field.");
                return;
            }

            // AND with 0 (false) so that the Solar Flare set bonus is never considered to be active. This stops the multiplicative DR from applying.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);

            // Remove the multiplicative DR from Beetle Shell's beetles
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("beetleDefense")))
            {
                LogFailure("Melee Multiplicative DR Removal", "Could not locate the Beetle Shell set bonus field.");
                return;
            }

            // AND with 0 (false) so that the Beetle Shell set bonus is never considered to be active. This stops the multiplicative DR from applying.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Remove Frozen Infliction From Deerclops Ice Spikes
        private static void RemoveFrozenInflictionFromDeerclopsIceSpikes(ILContext il)
        {
            // Prevent Deerclops from freezing players with Ice Spike projectiles.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(ProjectileID.DeerclopsIceSpike)))
            {
                LogFailure("Remove Frozen Infliction From Deerclops Ice Spikes", "Could not locate the Deerclops Ice Spike projectile ID.");
                return;
            }

            // AND with 0 (false) so that the Ice Spike is never considered to be hitting the player and thus never trigger the Frozen debuff.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Make GFB Nurse Meteor Undodgeable
        private static bool GFBNurseMeteorUndodgeable(On_Projectile.orig_IsDamageDodgable orig, Projectile self)
        {
            // Make the Leviathan meteor that spawns when talking to the Nurse in GFB undodgeable
            // Unfortunately the Dodgeable value in HurtModifiers cannot be set in the hook, thus On editing a vanilla function
            if (self.type == ModContent.ProjectileType<LeviathanBomb>() && self.damage == 9999)
                return false;
            else
                return orig(self);
        }
        #endregion

        #region Adjust lifesteal costs
        /// <summary>
        /// Reimplemnts our own implementation of Spectre Healing so we can customize effects
        /// Also means it will no longer reduce lifesteal cooldown when hitting with non-magic attacks, and not proc if every player is full HP
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="dmg"></param>
        /// <param name="Position"></param>
        /// <param name="victim"></param>
        private static void AdjustSpectreHealing(On_Projectile.orig_ghostHeal orig, Projectile self, int dmg, Vector2 Position, Entity victim)
        {
            float HealingMultiplier = 0.2f;
            float LifestealCooldownMult = 0.66f;
            
            var owner = Main.player[self.owner];
            HealingMultiplier -= self.numHits * 0.05f;
            int AmountToHeal = (int)Math.Round(dmg * HealingMultiplier);
            if (!self.magic || AmountToHeal <= 0 || Main.player[Main.myPlayer].lifeSteal <= 0f)
            {
                return;
            }
            float MissingLifeGoal = 0f;
            int targetPlayer = self.owner;
            foreach (var player in Main.ActivePlayers)
            {
                if (!player.dead && ((!self.hostile && !owner.hostile) || owner.team == player.team) && self.Distance(player.Center) <= 3000f)
                {
                    int MissingLife = player.statLifeMax2 - player.statLife;
                    if ((float)MissingLife > MissingLifeGoal)
                    {
                        MissingLifeGoal = MissingLife;
                        targetPlayer = player.whoAmI;
                    }
                }
            }
            AmountToHeal = (int)MathHelper.Min(AmountToHeal, MissingLifeGoal);
            if (AmountToHeal <= 0)
                return;
            owner.lifeSteal -= AmountToHeal * LifestealCooldownMult;
            Projectile.NewProjectile(self.GetSource_OnHit(victim, ProjectileSourceID.ToContextString(ProjectileSourceID.SetBonus_GhostHeal)), Position.X, Position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, self.owner, targetPlayer, AmountToHeal);
        }
        /// <summary>
        /// Adjust Vampire Knives implementation, stopping them from trying to heal when at full HP
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="dmg"></param>
        /// <param name="Position"></param>
        /// <param name="victim"></param>
        private static void AdjustVampireHealing(On_Projectile.orig_vampireHeal orig, Projectile self, int dmg, Vector2 Position, Entity victim)
        {
            int healAmount = (int)(dmg * 0.075f);
            float LifestealCooldownMult = 1f;
            if ((int)healAmount != 0 && !(Main.player[Main.myPlayer].lifeSteal <= 0f) && Main.player[Main.myPlayer].statLifeMax2 > Main.player[Main.myPlayer].statLife)
            {
                Main.player[Main.myPlayer].lifeSteal -= healAmount * LifestealCooldownMult;
                Projectile.NewProjectile(self.GetSource_OnHit(victim, ProjectileSourceID.ToContextString(ProjectileSourceID.VampireKnives)), Position.X, Position.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, self.owner, self.owner, healAmount);
            }
        }
        #endregion

        #region Tweak Pygmy Staff Aggro Distance Logic
        // Code written by Habble
        private static void PygmyAggroOnClosestPointInHitbox(ILContext context)
        {
            // Adjust Pygmy Staff's attack distance logic to be measured from the closest point to the enemy instead of the enemy's center.
            ILCursor cursor = new(context);

            // Go to the latest newly set variable near the point we wanna be, as that is an easy unique instruction to jump to. 131 is the number assigned to that local variable.
            if (!cursor.TryGotoNext(i => i.MatchStloc(131)))
            {
                LogFailure("Tweaking Pygmy Staff aggro distance logic", "Could not locate unique Stloc(131) instruction nearest to aggro distance check.");
                return;
            }
            // Move before the minion range variable emission so as to receive the value being targetted at it.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(126)))
            {
                LogFailure("Tweaking Pygmy Staff aggro distance logic", "Could not locate the minion range variable.");
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0); // Emits the Projectile entity itself
            cursor.Emit(OpCodes.Ldloc, 129); // Emits the NPC index via the incremented loop variable
            cursor.Emit(OpCodes.Ldloc, 6); // Emits the bool containing type check for Pygmies
            // Replace the distance with a different value calculated off of closest point in hitboxes
            cursor.EmitDelegate((float distance, Projectile projectile, int npcIndex, bool pygmy) =>
            {
                if (!pygmy)
                    return distance;

                NPC npc = Main.npc[npcIndex];
                Player player = Main.player[projectile.owner];
                float finalDistance = npc.Hitbox.ClosestPointInRect(player.Center).Distance(player.Hitbox.ClosestPointInRect(npc.Center));
                return finalDistance;
            });

            // That was part 1, time for part 2.
            // Go directly before the instruction that's meant to receive the distance value of this tagged NPC.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(185)))
            {
                LogFailure("Tweaking Pygmy Staff aggro distance logic", "Could not locate first unique Stloc(185) instruction.");
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc, 181); // Emits the tagged NPC
            cursor.Emit(OpCodes.Ldloc, 6);
            // Replace the distance with a different value calculated off of closest point in hitboxes
            cursor.EmitDelegate((float distance, Projectile projectile, NPC npc, bool pygmy) =>
            {
                if (!pygmy)
                    return distance;

                float finalDistance = npc.Hitbox.ClosestPointInRect(projectile.Center).Distance(projectile.Hitbox.ClosestPointInRect(npc.Center));
                return finalDistance;
            });

            // Go directly before the instruction that's meant to receive the distance value of this NPC currently being iterated over.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(189)))
            {
                LogFailure("Tweaking Pygmy Staff aggro distance logic", "Could not locate first unique Stloc(189) instruction.");
                return;
            }
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldloc, 186); // Emits the NPC index via the incremented loop variable
            cursor.Emit(OpCodes.Ldloc, 6);
            // Replace the distance with a different value calculated off of closest point in hitboxes
            cursor.EmitDelegate((float distance, Projectile projectile, int npcIndex, bool pygmy) =>
            {
                if (!pygmy)
                    return distance;

                NPC npc = Main.npc[npcIndex];
                float finalDistance = npc.Hitbox.ClosestPointInRect(projectile.Center).Distance(projectile.Hitbox.ClosestPointInRect(npc.Center));
                return finalDistance;
            });
        }
        #endregion

        #region Remove Vanilla Whip Tag Crits
        // Code written by Habble
        /// <summary>
        /// IL edits both vanilla whip crit tags' crit bool set calls to ensure they're never assigned a new value
        /// </summary>
        /// <param name="context"></param>
        private static void PreventVanillaWhipTagCrits(ILContext context)
        {
            ILCursor cursor = new(context);
            #region Morning Star
            // Go to the unique instructions nearest to the Morning Star tag's crit roll and then stand before the crit bool set call
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(59)) || !cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(30)))
            {
                LogFailure("Removing vanilla whip tag crits", "Could not locate the crit bool set call under Morning Star tag");
                return;
            }
            // Put the previous value of the crit bool on the stack to then return that instead of the supplied value by "consuming" them as parameters, so as to not overwrite it
            // This is more ideal than modifying label conditions because it is likelier for the latter to change over time
            cursor.Emit(OpCodes.Ldloc, 30);
            cursor.EmitDelegate((int input, bool originalValue) => originalValue.ToInt());
            #endregion Morning Star
            #region Kaleidoscope
            // Similar deal for Kaleidoscope as well
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(30)) || !cursor.TryGotoNext(MoveType.Before, i => i.MatchStloc(30)))
            {
                LogFailure("Removing vanilla whip tag crits", "Could not locate the crit bool set call under Kaleidoscope tag");
                return;
            }
            cursor.Emit(OpCodes.Ldloc, 30);
            cursor.EmitDelegate((int input, bool originalValue) => originalValue.ToInt());
            #endregion Kaleidoscope
        }
        #endregion
    }
}
