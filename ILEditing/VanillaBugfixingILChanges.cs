using CalamityMod.NPCs;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Renderers;

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
    }
}
