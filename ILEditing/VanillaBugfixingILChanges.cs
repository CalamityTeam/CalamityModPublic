using CalamityMod.NPCs;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Fixing Splitting Worm Banner Spam in Deathmode
        // CONTEXT FOR FIX: In Death Mode, normal worms are capable of splitting similarly to the Eater of Worlds. This, as expected, comes with problems with loot dropping, as you can kill multiple
        // head segments from the same original worm. TML allows us to safely handle this with its drop hooks. Unfortunately, however, this does not apply to banner dropping logic based on total kills or
        // for bestiary registrations.
        // As such, we must IL Edit the vanilla drop method to stop it from registering kills based on worms that can still be split. This references the same blocking logic as the aforementioned hooks.
        private static void FixSplittingWormBannerDrops(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Find the first return in the method. This will be marked as a label to jump to if the splitting loot check is failed, effectively terminating any and all
            // loot code, including banners and bestiary stuff.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchRet()))
            {
                LogFailure("Splitting worm banner spam fix", "Could not locate the first method return.");
            }

            // Save the ret as a place to return to.
            ILLabel ret = cursor.MarkLabel();

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(0)))
            {
                LogFailure("Splitting worm banner spam fix", "Could not locate the closest player storage.");
                return;
            }

            // Load the NPC caller onto the stack.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<NPC, bool>>(npc => CalamityGlobalNPC.SplittingWormLootBlockWrapper(npc, CalamityMod.Instance));

            // If the block is false (indicating the drop logic should stop), return the method early.
            cursor.Emit(OpCodes.Brfalse, ret);
        }
        #endregion Fixing Splitting Worm Banner Spam in Deathmode
    }
}
