using CalamityMod.CalPlayer;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Magnet Flower and Arcane Flower Changes
        private static void DecreaseMagnetFlowerAndArcaneFlowerManaCost(ILContext il)
        {
            // Decrease the mana cost.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ItemID.ArcaneFlower)))
            {
                LogFailure("Decrease Arcane Flower Mana Cost", "Could not locate the Arcane Flower item ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.08f)))
            {
                LogFailure("Decrease Arcane Flower Mana Cost", "Could not locate the Arcane Flower decreased mana cost value.");
                return;
            }
            cursor.Next.Operand = 0.2f;

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ItemID.MagnetFlower)))
            {
                LogFailure("Decrease Magnet Flower Mana Cost", "Could not locate the Magnet Flower item ID.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.08f)))
            {
                LogFailure("Decrease Magnet Flower Mana Cost", "Could not locate the Magnet Flower decreased mana cost value.");
                return;
            }
            cursor.Next.Operand = 0.2f;
        }
        #endregion

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

        #region Jump Speed Changes
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
            cursor.Prev.Operand = CalamityPlayer.BalloonJumpSpeedBoost;
            // Load the player's current jumpSpeed onto the stack and add the boost to it.
            cursor.Emit(OpCodes.Ldsfld, typeof(Player).GetField("jumpSpeed"));
            cursor.Emit(OpCodes.Add);

            // Remove the jump height addition from the Werewolf buff (Moon Charm).
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(2)))
            {
                LogFailure("Jump Height Boost Fixes", "Could not locate Moon Charm jump height boost value.");
                return;
            }
            cursor.Next.Operand = 0;
        }

        private static void JumpSpeedAdjustment(ILContext il)
        {
            // Increase the base jump speed of the player to make early game less of a slog.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(5.01f))) // The jumpSpeed variable is set to this specific value before anything else occurs.
            {
                LogFailure("Base Jump Speed Buff", "Could not locate the jump speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 5.51f); // Increase by 10%.
        }
        #endregion

        #region Run Speed Changes
        private static void MaxRunSpeedAdjustment(ILContext il)
        {
            // Increase the base max run speed of the player to make early game less of a slog.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(3f))) // The maxRunSpeed variable is set to this specific value before anything else occurs.
            {
                LogFailure("Base Max Run Speed Buff", "Could not locate the max run speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 4.5f); // Increase by 50%.
        }

        private static void RunSpeedAdjustments(ILContext il)
        {
            var cursor = new ILCursor(il);
            float horizontalSpeedCap = 3f; // +200%, aka triple speed. Vanilla caps at +60%
            float asphaltTopSpeedMultiplier = 1.75f; // +75%. Vanilla is +250%
            float asphaltSlowdown = 1f; // Vanilla is 2f. This should actually make asphalt faster.

            // Multiplied by 0.6 on frozen slime, for +26% acceleration
            // Multiplied by 0.7 on ice, for +47% acceleration
            float iceSkateAcceleration = 2.1f;
            float iceSkateTopSpeed = 1f; // no boost at all

            //
            // HORIZONTAL MOVEMENT SPEED CAP
            //
            {
                // Find the +60% horizontal movement speed cap. This is loaded as a double.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR8(1.6D)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate the horizontal movement speed cap.");
                    return;
                }

                // Replace it with the new, much higher cap.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R8, (double)horizontalSpeedCap);

                // Find the "replace your speed bonus with this if you're over the cap". This is loaded as a float.
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.6f)))
                {
                    LogFailure("Run Speed Adjustments", "Could not locate the horizontal movement speed cap replacement.");
                    return;
                }

                // Replace it with the new, much higher, cap.
                cursor.Remove();
                cursor.Emit(OpCodes.Ldc_R4, horizontalSpeedCap);
            }

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
        #endregion Run Speed Changes

        #region Vanilla Hover Wing Nerfs
        private static void ReduceWingHoverVelocities(ILContext il)
        {
            // Reduce wing hover horizontal velocities. Hoverboard is fine because both stats are at 10.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(6.25f))) // The accRunSpeed variable is set to this specific value before hover adjustments occur.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the base speed variable.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Vortex Booster and Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the vortex booster/nebula mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Vortex Booster and Nebula Mantle.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the vortex booster/nebula mantle speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The accRunSpeed for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the betsy's wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(12f))) // The runAcceleration for Betsy Wings.
            {
                LogFailure("Wing Hover Nerfs", "Could not locate the betsy's wings speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 10.8f); // Reduce by 10%.
        }
        #endregion Vanilla Hover Wing Nerfs

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
        #endregion Mana Regen Nerfs

        #region Damage Variance Dampening
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

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(16))) // The 15% upper bound of the variance.
            {
                LogFailure("+/-5% Damage Variance", "Could not locate the upper bound.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4_6); // Decrease to +5%.
        }
        #endregion Damage Variance Dampening

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
        #endregion Expert Hardmode Scaling Removal

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
        #endregion Chlorophyte Bullet Speed Nerfs
    }
}
