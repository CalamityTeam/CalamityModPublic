using CalamityMod.Balancing;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Decrease Sandstorm Wind Speed Requirement
        private static void DecreaseSandstormWindSpeedRequirement(ILContext il)
        {
            // Sandstorms don't rapidly diminish unless the wind speed is less than 0.2f instead of 0.6f.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.6f))) // The 0.6f wind speed check.
            {
                LogFailure("Decrease Sandstorm Wind Speed Requirement", "Could not locate the wind speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.2f); // Change to 0.2f.
        }
        #endregion Decrease Sandstorm Wind Speed Requirement

        #region Reforge Requirement Relaxation
        private static void RelaxPrefixRequirements(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Search for the first instance of Math.Round, which is used to round damage.
            // This one isn't edited, but hitting the Round function is the easiest way to get to the relevant part of the method.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall("System.Math", "Round")))
            {
                LogFailure("Prefix Requirements", "Could not locate the damage Math.Round call.");
                return;
            }

            // Search for the second instance of Math.Round, which is used to round use time.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall("System.Math", "Round")))
            {
                LogFailure("Prefix Requirements", "Could not locate the use time Math.Round call.");
                return;
            }

            // Search for the branch-if-not-equal which checks whether the use time change rounds to nothing.
            // If the change rounds to nothing, then it's equal, so the branch is NOT taken.
            // The branch skips over the "fail this prefix" code.
            ILLabel passesUseTimeCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesUseTimeCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate use time rounding equality branch.");
                return;
            }

            // To allow use-time affecting prefixes even on super fast weapons where they would round to nothing,
            // add another branch which skips over the "fail this prefix" code, given a custom condition.

            // Load the item itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which returns whether the item's use time is 2, 3, 4 or 5.
            cursor.EmitDelegate<Func<Item, bool>>((Item i) => i.useAnimation >= 2 && i.useAnimation <= 5);

            cursor.Emit(OpCodes.Brtrue_S, passesUseTimeCheck);

            // Search for the branch-if-not-equal which checks whether the mana change rounds to nothing.
            ILLabel passesManaCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesManaCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate mana prefix failure branch.");
                return;
            }

            // Emit an unconditional branch which skips the mana check failure.
            cursor.Emit(OpCodes.Br_S, passesManaCheck);

            // Search for the instance field load which retrieves the item's knockback.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Item>("knockBack")))
            {
                LogFailure("Prefix Requirements", "Could not locate knockback load instruction.");
                return;
            }

            // Search for the immediately-following constant load which pulls in 0.0.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0f)))
            {
                LogFailure("Prefix Requirements", "Could not locate zero knockback comparison constant.");
                return;
            }

            // Completely nullify the knockback computation by replacing the check against 0 with a check against negative one million.
            // If you absolutely need to block knockback reforges for some reason, you can set your knockback to this value.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, -1000000f);
        }
        #endregion Reforge Requirement Relaxation

        #region Prevention of Slime Rain Spawns When Near Bosses
        private static void PreventBossSlimeRainSpawns(On.Terraria.NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].Calamity().isNearbyBoss && CalamityConfig.Instance.BossZen)
                orig(plr);
        }
        #endregion Prevention of Slime Rain Spawns When Near Bosses

        #region Voodoo Demon Doll Spawn Manipulations
        // This may seem absolutely obscene, but vanilla spawn behavior does not occur within the spawn pool that TML provides, only modded
        // spawns do. Pretty much everything else is simply a fuckton of manual conditional NPC.NewNPC calls. As such, the only way to bypass
        // vanilla spawn behaviors is the IL edit them out of existence. Here, simply replacing the voodoo demon ID with an empty one is performed.
        // Something cleaner could probably be done, such as getting rid of the entire NPC.NewNPC call, but this is the easiest solution I can come up with.
        private static void MakeVoodooDemonDollWork(ILContext il)
        {
            var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(NPCID.VoodooDemon)))
            {
                LogFailure("Voodoo Demon Doll Mechanic", "Could not locate the Voodoo Demon ID.");
                return;
            }

            cursor.Remove();
            cursor.Emit(OpCodes.Ldloc, 10);
            cursor.EmitDelegate<Func<int, int>>(spawnPlayerIndex =>
            {
                if (Main.player[spawnPlayerIndex].active && Main.player[spawnPlayerIndex].Calamity().disableVoodooSpawns)
                    return NPCID.None;

                return NPCID.VoodooDemon;
            });
        }
        #endregion Voodoo Demon Doll Spawn Manipulations

        #region Disabling of Lava Slime Lava Creation
        private static void RemoveLavaDropsFromExpertLavaSlimes(ILContext il)
        {
            // Prevent Lava Slimes from dropping lava.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<WorldGen>("SquareTileFrame"))) // The only SquareTileFrame call in HitEffect.
            {
                LogFailure("Remove Lava Drops From Expert Lava Slimes", "Could not locate the SquareTileFrame function call.");
                return;
            }
            if (!cursor.TryGotoPrev(MoveType.Before, i => i.MatchLdcI4(NPCID.LavaSlime))) // The ID of Lava Slimes.
            {
                LogFailure("Remove Lava Drops From Expert Lava Slimes", "Could not locate the Lava Slime ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 0); // Change to an impossible scenario.
        }
        #endregion Disabling of Lava Slime Lava Creation

        #region Increase Pylon Interaction Range
        private static void IncreasePylonInteractionRange(ILContext il)
        {
            // Find the tile range variables and change them to something greater.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(20)))
            {
                LogFailure("Increase Pylon Interaction Range", "Could not locate the tile range X variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 100);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(20)))
            {
                LogFailure("Increase Pylon Interaction Range", "Could not locate the tile range X variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 100);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(20)))
            {
                LogFailure("Increase Pylon Interaction Range", "Could not locate the tile range Y variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 100);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(20)))
            {
                LogFailure("Increase Pylon Interaction Range", "Could not locate the tile range Y variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 100);
        }
        #endregion

        #region Make Meteorite Explodable
        private static void MakeMeteoriteExplodable(ILContext il)
        {
            // Find the Tile ID of Meteorite and change it to something that doesn't matter.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(TileID.Meteorite))) // The Meteorite Tile ID check.
            {
                LogFailure("Make Meteorite Explodable", "Could not locate the Meteorite Tile ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, TileID.HellstoneBrick); // Change to Hellstone Brick. They're made of Hellstone, so it makes sense they can't be exploded until Hardmode starts :^)
        }
        #endregion

        #region Make Windy Day Music Play Less Often
        private static void MakeWindyDayMusicPlayLessOften(ILContext il)
        {
            // Make windy day theme only play when the wind speed is over 0.5f instead of 0.4f and make it stop when the wind dies down to below 0.44f instead of 0.34f.
            var cursor = new ILCursor(il);

            FieldInfo _minWindField = typeof(Main).GetField("_minWind", BindingFlags.NonPublic | BindingFlags.Static);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(_minWindField))) // The min wind speed check that stops the windy day theme when the wind dies down enough.
            {
                LogFailure("Make Windy Day Music Play Less Often", "Could not locate the _minWind variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.44f); // Change to 0.44f.

            FieldInfo _maxWindField = typeof(Main).GetField("_maxWind", BindingFlags.NonPublic | BindingFlags.Static);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(_maxWindField))) // The max wind speed check that causes the windy day theme to play.
            {
                LogFailure("Make Windy Day Music Play Less Often", "Could not locate the _maxWind variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.5f); // Change to 0.5f.
        }
        #endregion Make Windy Day Music Play Less Often

        #region Change Blood Moon Max HP Requirements
        private static void BloodMoonsRequire200MaxLife(ILContext il)
        {
            // Blood Moons only happen when the player has over 200 max life.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(120))) // The 120 max life check.
            {
                LogFailure("Make Blood Moons Require 200 Max Life", "Could not locate the max life variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 200); // Change to 200.
        }
        #endregion Change Blood Moon Max HP Requirements

        #region Prevent Fossil Shattering
        private static void PreventFossilShattering(ILContext il)
        {
            // Find the Tile ID of Desert Fossil and change it to something that doesn't matter.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(TileID.DesertFossil))) // The Desert Fossil Tile ID check.
            {
                LogFailure("Prevent Fossil Shattering", "Could not locate the Desert Fossil Tile ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, TileID.PixelBox); // Change to Pixel Box because it cannot be obtained in-game without cheating.
        }
        #endregion

        #region Make Tag Damage Multiplicative
        private static void MakeTagDamageMultiplicative(ILContext il)
        {
            var cursor = new ILCursor(il);
            int damageLocalIndex = 37;

            bool replaceWithMultipler(int flagLocalIndex, float damageFactor, bool usesExtraVariableToStoreDamage = false)
            {
                // Move after the bool load and branch-if-false instruction.
                cursor.Goto(0);
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(flagLocalIndex)))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the flag local index of '{flagLocalIndex}'.");
                    return false;
                }

                // Move to the point at which a local is loaded after the boolean.
                // Ideally this would be two instructions afterwards (load bool, branch), but we cannot guarantee that this will be the case.
                // As such, a match is done instead.
                if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(out _)))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the succeeding local after the flag local index of '{flagLocalIndex}'.");
                    return false;
                }

                // OPTIONAL case for if an extra variable to store damage is used:
                // Load damage to add.
                // Store damage to add as a variable.

                // Load damage ->
                // Load damage addition ->
                // Add the two ->
                // Store damage.

                // This logic for adding damage is disabled by popped at the point at which the addition happens and replacing it with zero, resulting in x += 0.
                if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchAdd()))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the damage addition at the flag local index of '{flagLocalIndex}'.");
                    return false;
                }
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4_0);

                // After this, the following operations are done as a replacement to achieve multiplicative damage:

                // Load damage ->
                // Cast damage to float ->
                // Load the damage factor ->
                // Multiply the two ->
                // Cast the result to int, removing the fractional part ->
                // Store damage.
                cursor.Emit(OpCodes.Ldloc, damageLocalIndex);
                cursor.Emit(OpCodes.Conv_R4);
                cursor.Emit(OpCodes.Ldc_R4, damageFactor);
                cursor.Emit(OpCodes.Mul);
                cursor.Emit(OpCodes.Conv_I4);
                cursor.Emit(OpCodes.Stloc, damageLocalIndex);
                return true;
            }

            // Leather whip.
            replaceWithMultipler(50, BalancingConstants.LeatherWhipTagDamageMultiplier);

            // Durendal.
            replaceWithMultipler(51, BalancingConstants.DurendalTagDamageMultiplier);

            // Snapthorn.
            replaceWithMultipler(54, BalancingConstants.SnapthornTagDamageMultiplier);

            // Spinal Tap.
            replaceWithMultipler(55, BalancingConstants.SpinalTapTagDamageMultiplier);

            // Morning Star.
            replaceWithMultipler(56, BalancingConstants.MorningStarTagDamageMultiplier);

            // Kaleidoscope.
            replaceWithMultipler(57, BalancingConstants.KaleidoscopeTagDamageMultiplier, true);

            // SPECIAL CASE: Firecracker's damage is fucking absurd and everything needs to go.
            cursor.Goto(0);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(64)))
            {
                LogFailure("Making Tag Damage Multiplicative", $"Could not locate the flag local index of 52.");
                return;
            }

            // Change the damage of the explosions.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<Projectile, int>>(projectile =>
            {
                int damage = (int)(Main.player[projectile.owner].ActiveItem().damage * BalancingConstants.FirecrackerExplosionDamageMultiplier);
                damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(damage);
                return damage;
            });
            cursor.Emit(OpCodes.Stloc, 64);

            // Change the x in damage += x; to zero.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchAdd()))
            {
                LogFailure("Making Tag Damage Multiplicative", $"Could not locate the damage additive value.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_0);
        }
        #endregion Make Tag Damage Multiplicative

        #region Remove Hellforge Pickaxe Requirement
        private static int RemoveHellforgePickaxeRequirement(On.Terraria.Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            if (tileTarget.TileType == TileID.Hellforge)
                pickPower = 65;

            return orig(self, x, y, pickPower, hitBufferIndex, tileTarget);
        }
        #endregion

        #region Fix Chlorophyte Crystal Attacking Where it Shouldn't
        // TODO -- Finish this
        #endregion Fix Chlorophyte Crystal Attacking Where it Shouldn't

        #region Color Blighted Gel
        private static void ColorBlightedGel(On.Terraria.GameContent.ItemDropRules.CommonCode.orig_ModifyItemDropFromNPC orig, NPC npc, int itemIndex)
        {
            orig(npc, itemIndex);

            Item item = Main.item[itemIndex];
            int itemID = item.type;
            bool colorWasChanged = false;

            if (itemID == ModContent.ItemType<BlightedGel>() && npc.type == ModContent.NPCType<CrimulanBlightSlime>())
            {
                item.color = new Color(1f, 0f, 0.16f, 0.6f);
                colorWasChanged = true;
            }
            if (itemID == ItemID.SharkFin && npc.type == ModContent.NPCType<Mauler>())
            {
                item.color = new Color(151, 115, 57, 255);
                colorWasChanged = true;
            }

            // Sync the color changes.
            if (colorWasChanged)
                NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, itemID, 1f);
        }
        #endregion Color Blighted Gel
    }
}
