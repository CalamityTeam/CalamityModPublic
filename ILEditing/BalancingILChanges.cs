using System;
using CalamityMod.Balancing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.FurniturePlagued;
using CalamityMod.Items.Potions;
using CalamityMod.NPCs.DraedonLabThings;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Shimmer Changes

        private static bool AdjustShimmerRequirements(On_Item.orig_CanShimmer orig, Item item)
        {
            //Rod of Harmony / psc requires Draedong and SCal dead instead of Moon Lord.
            if (item.type == ItemID.RodofDiscord || item.type == ModContent.ItemType<ProfanedSoulCrystal>())
            {
                return DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs;
            }

            return orig(item);
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
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(1.75f)))
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
            cursor.EmitDelegate<Func<float>>(() => CalamityConfig.Instance.HigherJumpHeight ? BalancingConstants.ConfigBoostedBaseJumpHeight : VanillaBaseJumpHeight);
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
        private static int AdjustDamageVariance(Terraria.On_Main.orig_DamageVar_float_int_float orig, float dmg, int percent, float luck)
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

        #region Remove Lunatic Cultist Homing Resist
        private static void RemoveLunaticCultistHomingResist(ILContext il)
        {
            // Change Lunatic Cultist's resist from 25% to 0% (effectively removing it).
            var cursor = new ILCursor(il);
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
    }
}
