using System;
using CalamityMod.Balancing;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Soaring Insignia Changes
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

        private static void NerfSoaringInsigniaRunAcceleration(ILContext il)
        {
            // Nerf the run acceleration boost from 2x to 1.1x.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")))
            {
                LogFailure("Soaring Insignia Mobility Nerf", "Could not locate the Soaring Insignia bool.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2f)))
            {
                LogFailure("Soaring Insignia Mobility Nerf", "Could not locate the Soaring Insignia run acceleration multiplier.");
                return;
            }
            cursor.Next.Operand = 1.1f;

            // Prevent the rocket boots infinite flight effect.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("empressBrooch")))
            {
                LogFailure("Soaring Insignia Mobility Nerf", "Could not locate the Soaring Insignia bool.");
                return;
            }

            // AND with 0 (false) so that the Soaring Insignia is never considered equipped and thus infinite rocket boots never triggers.
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Magiluminescence Changes
        private static void NerfMagiluminescence(ILContext il)
        {
            // Nerf the run acceleration boost from 2x to 1.25x.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Player>("hasMagiluminescence")))
            {
                LogFailure("Magiluminescence Nerf", "Could not locate the Magiluminescence bool.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2f)))
            {
                LogFailure("Magiluminescence Nerf", "Could not locate the Magiluminescence run acceleration multiplier.");
                return;
            }
            cursor.Next.Operand = 1.25f;

            // Nerf the max run speed boost from 1.2x to 1x.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.2f)))
            {
                LogFailure("Magiluminescence Nerf", "Could not locate the Magiluminescence max run speed multiplier.");
                return;
            }
            cursor.Next.Operand = 1f;

            // Nerf the acc run speed boost from 1.2x to 1x.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.2f)))
            {
                LogFailure("Magiluminescence Nerf", "Could not locate the Magiluminescence acc run speed multiplier.");
                return;
            }
            cursor.Next.Operand = 1f;

            // Nerf the run slowdown boost from 2x to 1.25x.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2f)))
            {
                LogFailure("Magiluminescence Nerf", "Could not locate the Magiluminescence run slowdown multiplier.");
                return;
            }
            cursor.Next.Operand = 1.25f;
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
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2.4f)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Soaring Insignia jump speed boost value.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.5f); // Decrease to 0.5f.

            // Find the Frog Leg jump speed bonus and reduce it to 1.2f.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(2.4f)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Frog Leg jump speed boost value.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 1.2f); // Decrease to 1.2f.

            // Remove the jump height addition from the Werewolf buff (Moon Charm).
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(2)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Moon Charm jump height boost value.");
                return;
            }
            cursor.Next.Operand = 0;
        }

        private const float VanillaBaseJumpHeight = 5.01f;
        private static void BaseJumpHeightAdjustment(ILContext il)
        {
            // Increase the base jump height of the player to make early game less of a slog.
            var cursor = new ILCursor(il);

            // The jumpSpeed variable is set to this specific value before anything else occurs.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(VanillaBaseJumpHeight)))
            {
                LogFailure("Base Jump Height Buff", "Could not locate the jump height variable.");
                return;
            }
            cursor.Remove();

            // Increase by 10% if the higher jump speed is enabled.
            cursor.EmitDelegate<Func<float>>(() => CalamityConfig.Instance.FasterJumpSpeed ? BalancingConstants.ConfigBoostedBaseJumpHeight : VanillaBaseJumpHeight);
        }
        #endregion

        #region Run Speed Changes
        private static void RunSpeedAdjustments(ILContext il)
        {
            var cursor = new ILCursor(il);
            float asphaltTopSpeedMultiplier = 1.75f; // +75%. Vanilla is +250%
            float asphaltSlowdown = 1f; // Vanilla is 2f. This should actually make asphalt faster.

            // Dunerider Boots multiply all run stats by 1.75f in vanilla
            float duneRiderBootsMultiplier = 1.25f; // Change to 1.25f

            // Multiplied by 0.6 on frozen slime, for +26% acceleration
            // Multiplied by 0.7 on ice, for +47% acceleration
            float iceSkateAcceleration = 2.1f;
            float iceSkateTopSpeed = 1f; // no boost at all

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
            // DUNERIDER BOOTS + SAND BLOCKS
            //
            {
                // Find the multiplier for Dunerider Boots on Sand Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.75f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate the Dunerdier Boots multiplier.");
                    return;
                }

                // Massively reduce the increased speed of Dunerider Boots while on Sand Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, duneRiderBootsMultiplier);
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

                // Find the top speed multiplier of Ice Skates on Frozen Slime Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Frozen Slime Block top speed multiplier.");
                    return;
                }

                // Make Ice Skates give no top speed boost whatsoever on Frozen Slime Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateTopSpeed);
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

                // Find the top speed multiplier of Ice Skates on Ice Blocks.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.25f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate Ice Skates + Ice Block top speed multiplier.");
                    return;
                }

                // Make Ice Skates give no top speed boost whatsoever on Ice Blocks.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, iceSkateTopSpeed);
            }
        }
        #endregion

        #region Vanilla Hover Wing Nerfs
        private static void ReduceWingHoverVelocities(ILContext il)
        {
            // Reduce wing hover horizontal velocities. Hoverboard is fine because both stats are at 10.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(4.5f))) // The acceleration multiplier for Celestial Starboard.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Celestial Starboard acceleration variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 2.75f); // Reduce by ~38.8%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(16f))) // The accRunSpeed for Celestial Starboard.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Celestial Starboard speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 11.6f); // Reduce by 27.5%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(16f))) // The runAcceleration for Celestial Starboard.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Celestial Starboard speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 11.6f); // Reduce by 27.5%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Betsy's Wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Betsy's Wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Dev Wings capable of hovering.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Dev Wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Dev Wings capable of hovering.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Dev Wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Vortex Booster.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Vortex Booster speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Vortex Booster.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Vortex Booster speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Nebula Mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the Nebula Mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.
        }
        #endregion

        #region Life Regen Changes
        private static void PreventWellFedFromBeingRequiredInExpertModeForFullLifeRegen(ILContext il)
        {
            // Prevent the greatly reduced life regen while without the well fed buff in expert mode.
            var cursor = new ILCursor(il);
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

        private static void ManaRegenAdjustment(ILContext il)
        {
            // Increase the base mana regen so that mage is less annoying to play without mana regen buffs.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.8f))) // The multiplier for the mana regen formula: (float)statMana / (float)statManaMax2 * 0.8f + 0.2f.
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
        private static void AdjustDamageVariance(ILContext il)
        {
            // Change the damage variance from +-15% to +-5%.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(-15))) // The -15% lower bound of the variance.
            {
                LogFailure("+/-5% Damage Variance", "Could not locate the lower bound.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, -5); // Increase to -5%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(16))) // The +15% upper bound of the variance.
            {
                LogFailure("+/-5% Damage Variance", "Could not locate the upper bound.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_6); // Decrease to +5%.

            // Remove the ability for luck to affect damage variance.
            // TODO -- Old Die should re-enable this later, which will cause a lot of complexity as DamageVar has no context.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg(1))) // The function argument for luck.
            {
                LogFailure("Damage Variance Luck Removal", "Could not locate the first luck load.");
                return;
            }

            // Multiply the loaded luck value by zero so the positive luck condition never triggers.
            cursor.Emit(OpCodes.Ldc_R4, 0f);
            cursor.Emit(OpCodes.Mul);

            // Do the same thing again for the second luck check.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdarg(1))) // The function argument for luck.
            {
                LogFailure("Damage Variance Luck Removal", "Could not locate the second luck load.");
                return;
            }

            // Multiply the loaded luck value by zero so the negative luck condition never triggers.
            cursor.Emit(OpCodes.Ldc_R4, 0f);
            cursor.Emit(OpCodes.Mul);
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

        #region Chlorophyte Bullet Speed Nerfs
        private static void AdjustChlorophyteBullets(ILContext il)
        {
            // Reduce dust from 10 to 5 and homing range.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ProjectileID.ChlorophyteBullet)))
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the bullet ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(10))) // The number of dust spawned by the bullet.
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the dust quantity.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_5); // Decrease dust to 5.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(300f))) // The 300 unit distance required to home in.
            {
                LogFailure("Chlorophyte Bullet AI", "Could not locate the homing range.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 150f); // Reduce homing range by 50%.
        }
        #endregion

        #region Sharpening Station Nerf
        private static void NerfSharpeningStation(ILContext il)
        {
            // Reduce armor penetration from the Sharpening Station from 12 (it was originally 16!)
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdcI4(BuffID.Sharpened)))
            {
                LogFailure("Sharpening Station Nerf", "Could not locate the Sharpened buff ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The amount of armor penetration to grant.
            {
                LogFailure("Sharpening Station Nerf", "Could not locate the amount of armor penetration granted.");
                return;
            }

            // Replace the value entirely.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, BalancingConstants.SharpeningStationArmorPenetration);
        }
        #endregion
    }
}
