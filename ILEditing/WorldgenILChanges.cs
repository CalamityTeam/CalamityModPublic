using CalamityMod.World;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Replacement of Pharaoh Set in Pyramids
        // Note: There is no need to replace the other Pharaoh piece, due to how the vanilla code works.
        // The other Pharaoh vanity piece is added automatically if the mask is found in the chest.
        private static void ReplacePharaohSetInPyramids(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Find the instruction which loads in the item ID of the Pharaoh's Mask.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(ItemID.PharaohsMask)))
            {
                LogFailure("Pharaoh Set Pyramid Replacement", "Could not locate the Pharaoh's Mask item ID.");
                return;
            }

            // Replace the Pharaoh's Mask with the Amber Hook.
            // THIS INT CAST IS MANDATORY. DO NOT REMOVE IT.
            // In TML's DLL, despite item IDs being shorts (16 bits), the hardcoded 848 here is a 32-bit integer literal.
            // As such, it must be replaced with a 32-bit integer literal or the branches of the switch will misalign and the IL edit fails.
            cursor.Next.Operand = (int)ItemID.AmberHook;
        }
        #endregion Replacement of Pharaoh Set in Pyramids

        #region Fixing of Abyss/Dungeon Interactions
        private static void PreventDungeonHorizontalCollisions(ILContext il)
        {
            // Prevent the Dungeon from appearing near the Sulph sea.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStsfld<WorldGen>("dungeonY")))
            {
                LogFailure("Dungeon/Abyss Collision Avoidance (Starting Position)", "Could not locate the dungeon's vertical position.");
                return;
            }

            cursor.EmitDelegate<Action>(() =>
            {
                WorldGen.dungeonX = Utils.Clamp(WorldGen.dungeonX, SulphurousSea.BiomeWidth + 100, Main.maxTilesX - SulphurousSea.BiomeWidth - 100);

                // Adjust the Y position of the dungeon to accomodate for the X shift.
                WorldUtils.Find(new Point(WorldGen.dungeonX, WorldGen.dungeonY), Searches.Chain(new Searches.Down(9001), new Conditions.IsSolid()), out Point result);
                WorldGen.dungeonY = result.Y - 10;
            });
        }

        private static void PreventDungeonHallCollisions(ILContext il)
        {
            // Prevent the Dungeon's halls from getting anywhere near the Abyss.
            var cursor = new ILCursor(il);

            // Forcefully clamp the X position of the new hall end.
            // This prevents a hall, and as a result, the dungeon, from ever impeding on the Abyss/Sulph Sea.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(6)))
            {
                LogFailure("Dungeon/Abyss Collision Avoidance (Halls)", "Could not locate the hall horizontal position.");
                return;
            }

            cursor.Emit(OpCodes.Ldloc, 6);
            cursor.EmitDelegate<Func<Vector2, Vector2>>(unclampedValue =>
            {
                unclampedValue.X = MathHelper.Clamp(unclampedValue.X, SulphurousSea.BiomeWidth + 25, Main.maxTilesX - SulphurousSea.BiomeWidth - 25);
                return unclampedValue;
            });
            cursor.Emit(OpCodes.Stloc, 6);
        }
        #endregion Fixing of Abyss/Dungeon Interactions

        #region Fixing of Living Tree/Sulphurous Sea Interactions
        private static void BlockLivingTreesNearOcean(ILContext il)
        {
            var cursor = new ILCursor(il);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<int, int>>(x => Utils.Clamp(x, 560, Main.maxTilesX - 560));
            cursor.Emit(OpCodes.Starg, 0);
        }
        #endregion Fixing of Living Tree/Sulphurous Sea Interactions

        #region Removal of Hardmode Ore Generation from Evil Altars
        private static void PreventSmashAltarCode(On.Terraria.WorldGen.orig_SmashAltar orig, int i, int j)
        {
            if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                return;

            orig(i, j);
        }
        #endregion Removal of Hardmode Ore Generation from Evil Altars

        #region Chlorophyte Spread Improvements
        private static void AdjustChlorophyteSpawnRate(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(300))) // 1 in 300 genRand call used to generate Chlorophyte in mud tiles near jungle grass.
            {
                LogFailure("Chlorophyte Spread Rate", "Could not locate the update chance.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 150); // Increase the chance to 1 in 150.
        }

        private static void AdjustChlorophyteSpawnLimits(ILContext il)
        {
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(40))) // Find the 40 Chlorophyte tile limit. This limit is checked within a 71x71-tile square, with the reference tile as the center.
            {
                LogFailure("Chlorophyte Spread Limit", "Could not locate the lower limit.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 60); // Increase the limit to 60.

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(130))) // Find the 130 Chlorophyte tile limit. This limit is checked within a 171x171-tile square, with the reference tile as the center.
            {
                LogFailure("Chlorophyte Spread Limit", "Could not locate the upper limit.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 200); // Increase the limit to 200.
        }
        #endregion Chlorophyte Spread Improvements

        #region World Creation UI Default Size Change
        /// <summary>
        /// Modifies the default world size on the world creation menu to be Large instead of Small.
        /// </summary>
        private static void ChangeDefaultWorldSize(ILContext il)
        {
            // Objective 1: Pop value '0' off the stack and emit value '2'. This changes the enum used for setting the default world size.
            // Objective 2: Invoke UpdatePreviewPlate at the end of the method and set _optionSize to Large.
            var c = new ILCursor(il);
            
            // OBJECTIVE 1
            
            // Find and anchor ourselves at roughly the start of the first for loop of this method.
            if (!c.TryGotoNext(x => x.MatchBr(out _)))
            {
                LogFailure("Change Default World Size", "Could not match start of branched for loop.");
                return;
            }
            
            // Position ourselves directly after where '0' is pushed.
            if (!c.TryGotoNext(MoveType.After, x => x.MatchLdcI4(0)))
            {
                LogFailure("Change Default World Size", "Could not match '0' indicating WorldSizeId.Small.");
                return;
            }

            // Pop original value off.
            c.Emit(OpCodes.Pop);
            
            // Push '2' to the stack.
            c.Emit(OpCodes.Ldc_I4_2);
            
            // OBJECTIVE 2

            // Match right before the method returns.
            if (!c.TryGotoNext(x => x.MatchRet()))
            {
                LogFailure("Change Default World Size", "Could not match end of method.");
                return;
            }
            
            // Set _optionSize to Large.
            c.Emit(OpCodes.Ldarg_0); // this
            c.Emit(OpCodes.Ldc_I4_2); // '2'
            c.Emit<UIWorldCreation>(OpCodes.Stfld, "_optionSize"); // UIWorldCreation._optionSize

            // Invoke UpdatePreviewPlate with our current instance.
            c.Emit(OpCodes.Ldarg_0); // this
            c.Emit<UIWorldCreation>(OpCodes.Call, "UpdatePreviewPlate"); // UIWorldCreation.UpdatePreviewPlate
        }
        #endregion
    }
}
