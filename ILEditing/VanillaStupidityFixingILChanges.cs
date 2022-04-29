using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
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
            if (!Main.player[plr].Calamity().bossZen)
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
            cursor.Emit(OpCodes.Ldloc, 6);
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

        #region Fix Chlorophyte Crystal Attacking Where it Shouldn't

        #endregion Fix Chlorophyte Crystal Attacking Where it Shouldn't
    }
}
