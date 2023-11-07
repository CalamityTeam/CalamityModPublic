using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace CalamityMod.Graphics
{
    /// <summary>
    /// Wrapper class for <see cref="RenderTarget2D"/> that safely handles resizing, unloading and auto-disposal if not currently in use to save
    /// on GPU memory.
    /// </summary>
    public class ManagedRenderTarget : IDisposable
    {
        private RenderTarget2D target;

        public readonly RenderTargetCreationCondition CreationCondition;

        public readonly bool ShouldResetUponScreenResize;

        public readonly bool ShouldAutoDispose;

        public bool WaitingForFirstInitialization
        {
            get;
            private set;
        } = true;

        public bool IsUninitialized => target is null || target.IsDisposed;

        public bool IsDisposed
        {
            get;
            private set;
        }

        /// <summary>
        /// The actual <see cref="RenderTarget2D"/> instance that the wrapper contains.
        /// </summary>
        public RenderTarget2D Target
        {
            get
            {
                if (IsUninitialized)
                {
                    target = CreationCondition(Main.screenWidth, Main.screenHeight);
                    WaitingForFirstInitialization = false;
                    IsDisposed = false;
                }

                TimeSinceLastAccessed = 0;
                return target;
            }
            private set => target = value;
        }

        public int Width => Target.Width;

        public int Height => Target.Height;

        public Vector2 Size => new(Width, Height);

        /// <summary>
        /// How many frames since this target has been gotten. Used to dispose of unused targets for the sake of performance.
        /// </summary>
        public int TimeSinceLastAccessed;

        public delegate RenderTarget2D RenderTargetCreationCondition(int screenWidth, int screenHeight);

        // For some reason, adding more arguments to this constructor causes really low end pcs to crash in FNA. I cannot figure out why, so do not use them.
        public static RenderTarget2D CreateScreenSizedTarget(int screenWidth, int screenHeight) => new(Main.instance.GraphicsDevice, screenWidth, screenHeight);

        public ManagedRenderTarget(bool shouldResetUponScreenResize, RenderTargetCreationCondition creationCondition, bool shouldAutoDispose = true)
        {
            ShouldResetUponScreenResize = shouldResetUponScreenResize;
            CreationCondition = creationCondition;
            ShouldAutoDispose = shouldAutoDispose;
            RenderTargetManager.ManagedTargets.Add(this);
        }

        /// <summary>
        /// Automatically called by <see cref="RenderTargetManager"/>, do not call!
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            target?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Automatically called by <see cref="RenderTargetManager"/>, do not call!
        /// </summary>
        public void Recreate(int screenWidth, int screenHeight)
        {
            Dispose();
            IsDisposed = false;

            target = CreationCondition(screenWidth, screenHeight);
            TimeSinceLastAccessed = 0;
        }

        public static implicit operator RenderTarget2D(ManagedRenderTarget target) => target.Target;
    }
}
