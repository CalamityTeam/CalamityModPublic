using CalamityMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;
using Terraria.ID;

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

        #region Fixing Splitting Worm Interaction in Deathmode
        // CONTEXT FOR FIX: In Death Mode, normal worms are capable of splitting similarly to the Eater of Worlds. This comes with problems with bestiary entries. The entries are incremented only when
        // the killed npc is the head segment and it has been interacted at least once. These two conditions may be violated because NPC.NPCLoot only triggers once because of the above fix.
        // The fix for former condition is in CalamityGlobalNPCLoot.OnKill. For the latter condition, NPC.PlayerInteraction should broadcast interaction to every other worm segments.
        // This is already done for vanilla worm enemies like EOW.
        private static void FixSplittingWormInteraction(On_NPC.orig_PlayerInteraction orig, NPC npc, int player)
        {
            orig(npc, player);
            CalamityGlobalNPC.SplittingWormBroadcastInteractionWrapper(npc, player);
        }
        #endregion

        #region Fixing Vanilla Not Accounting For Spritebatch Modification in Held Projectiles
        private static bool HasLoggedHeldProjectileBlendStateCatch = false;
        private void FixHeldProjectileBlendState(On_PlayerDrawLayers.orig_DrawHeldProj orig, PlayerDrawSet drawinfo, Projectile proj)
        {
            orig(drawinfo, proj);

            // Vanilla uses a worse quality sampler state for mounts when moving for some reason. Really couldn't say why.
            var sampler = (drawinfo.drawPlayer.mount.Active && drawinfo.drawPlayer.fullRotation != 0f) ? LegacyPlayerRenderer.MountedSamplerState : Main.DefaultSamplerState;

            try
            {
                // Restart the spritebatch, to ensure that modifications made to it are properly restored.
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, sampler, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            catch
            {
                if (!HasLoggedHeldProjectileBlendStateCatch)
                    LogFailure("FixHeldProjectileBlendState", "The spritebatch was not left properly by another mod! The game will now most likely crash.");

                HasLoggedHeldProjectileBlendStateCatch = true;
            }
        }
        #endregion

        #region Fix Vanilla Not Accounting For Multiple Bobbers When Fishing With Truffle Worm
        private void FixTruffleWormFishing(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Initialize a flag variable whether truffle worm was used.
            il.Method.Body.Variables.Add(new VariableDefinition(il.Module.TypeSystem.Boolean));
            byte truffleWormUsed = (byte) (il.Method.Body.Variables.Count - 1);
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.Stloc_S, truffleWormUsed);

            // Move after beq.s, which is before Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait gets called
            if (!cursor.TryGotoNext(MoveType.After, i => i.Match(OpCodes.Beq_S)))
            {
                LogFailure("FixTruffleWormFishing", "Could not locate beq.s before Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait.");
                return;
            }

            // Skip if truffle worm was already used.
            var loopEnd = il.DefineLabel();
            cursor.Emit(OpCodes.Ldloc_S, truffleWormUsed);
            cursor.Emit(OpCodes.Brtrue_S, loopEnd);

            // Find the call to Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Player>("ItemCheck_CheckFishingBobber_PickAndConsumeBait")))
            {
                LogFailure("FixTruffleWormFishing", "Could not locate the call to Player.ItemCheck_CheckFishingBobber_PickAndConsumeBait.");
                return;
            }

            // Retrive baitTypeUsed, compare with truffle worm, and save it.
            cursor.Emit(OpCodes.Ldloc_S, (byte)4);
            cursor.Emit(OpCodes.Ldc_I4, ItemID.TruffleWorm);
            cursor.Emit(OpCodes.Ceq);
            cursor.Emit(OpCodes.Stloc_S, truffleWormUsed);

            // Move before next ldloc.0, which is the end of the loop
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc0()))
            {
                LogFailure("FixTruffleWormFishing", "Could not find the end of the loop.");
                return;
            }

            cursor.MarkLabel(loopEnd);
        }
        #endregion
    }
}
