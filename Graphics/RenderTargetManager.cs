using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics
{
    /// <summary>
    /// Manager class that handles all created instances of <see cref="ManagedRenderTarget"/>
    /// </summary>
    public class RenderTargetManager : ModSystem
    {
        internal static List<ManagedRenderTarget> ManagedTargets = new();

        public delegate void RenderTargetUpdateDelegate();

        /// <summary>
        /// Use this event to update/draw things to the target before anything else is drawn to the game.
        /// </summary>
        public static event RenderTargetUpdateDelegate RenderTargetUpdateLoopEvent;

        /// <summary>
        /// How many frames should pass since a target was last accessed before automatically disposing of it.
        /// </summary>
        public const int TimeBeforeAutoDispose = 600;

        internal static void ResetTargetSizes(Vector2 screenSize)
        {
            foreach (ManagedRenderTarget target in ManagedTargets)
            {
                // Don't attempt to recreate targets that are already initialized or shouldn't be recreated.
                if (target is null || target.IsDisposed || target.WaitingForFirstInitialization)
                    continue;

                Main.QueueMainThreadAction(() =>
                {
                    target.Recreate((int)screenSize.X, (int)screenSize.Y);
                });
            }
        }

        internal static void DisposeOfTargets()
        {
            if (ManagedTargets is null)
                return;

            Main.QueueMainThreadAction(() =>
            {
                foreach (ManagedRenderTarget target in ManagedTargets)
                    target?.Dispose();
                ManagedTargets.Clear();
            });
        }

        public override void OnModLoad()
        {
            ManagedTargets = new();
            Main.OnPreDraw += HandleTargetUpdateLoop;
            Main.OnResolutionChanged += ResetTargetSizes;
        }

        public override void OnModUnload()
        {
            DisposeOfTargets();
            Main.OnPreDraw -= HandleTargetUpdateLoop;
            Main.OnResolutionChanged -= ResetTargetSizes;

            if (RenderTargetUpdateLoopEvent != null)
            {
                foreach (var subscription in RenderTargetUpdateLoopEvent.GetInvocationList())
                    RenderTargetUpdateLoopEvent -= (RenderTargetUpdateDelegate)subscription;
            }
        }

        private void HandleTargetUpdateLoop(GameTime obj)
        {
            // Auto-disposal of targets that havent been used in a while, to stop them hogging GPU memory.
            if (ManagedTargets != null)
            {
                foreach (ManagedRenderTarget target in ManagedTargets)
                {
                    if (target == null || target.IsDisposed || !target.ShouldAutoDispose)
                        continue;

                    if (target.TimeSinceLastAccessed >= TimeBeforeAutoDispose)
                        target.Dispose();
                    else
                        target.TimeSinceLastAccessed++;
                }
            }
            RenderTargetUpdateLoopEvent?.Invoke();
        }
    }
}
