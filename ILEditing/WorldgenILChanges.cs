using CalamityMod.World;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Replacement of Pharaoh Set in Pyramids
        // Note: There is no need to replace the other Pharaoh piece, due to how the vanilla code works.
        private static void ReplacePharaohSetInPyramids(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Determine the area which determines the held item.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(36)))
            {
                LogFailure("Pharaoh Set Pyramid Replacement", "Could not locate the pyramid item selector value.");
                return;
            }

            int startOfItemSelection = cursor.Index;
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(34)))
            {
                LogFailure("Pharaoh Set Pyramid Replacement", "Could not locate the pyramid loot room left position.");
                return;
            }
            int endOfItemSelection = cursor.Index;

            // And delete it completely with intent to replace it.
            // Nuances with compliation appear to make simple a load + remove by constant not work.
            cursor.Index = startOfItemSelection;
            cursor.RemoveRange(endOfItemSelection - startOfItemSelection);

            // And select the item type directly.
            cursor.Emit(OpCodes.Ldloc, 36);
            cursor.EmitDelegate<Func<int, int>>(choice =>
            {
                switch (choice)
                {
                    case 0:
                        return ItemID.SandstorminaBottle;

                    // TODO - Replace this with an amber hook in 1.4 to make more sense thematically.
                    case 1:
                        return ItemID.RubyHook;
                    case 2:
                    default:
                        return ItemID.FlyingCarpet;
                }
            });
            cursor.Emit(OpCodes.Stloc, 36);
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
    }
}
