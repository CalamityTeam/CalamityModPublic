using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Renderers
{
    /// <summary>
    /// A class to handle drawing a visual effect to a rendertarget, then drawing the target to the screen at selected layer.
    /// </summary>
    public abstract class BaseRenderer : ModType
    {
        #region Fields/Properties
        /// <summary>
        /// The layer that the target should be drawn at.
        /// </summary>
        public abstract DrawLayer Layer { get; }

        /// <summary>
        /// Whether the target should draw and be drawn to this frame.
        /// </summary>
        public abstract bool ShouldDraw { get; }

        /// <summary>
        /// The target that the visual is drawn to. Automatically handled.
        /// </summary>
        public ManagedRenderTarget MainTarget
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        protected sealed override void Register()
        {
            ModTypeLookup<BaseRenderer>.Register(this);

            if (RendererManager.Renderers.Contains(this))
                throw new Exception($"Renderer '{Name}' has already been registered!");

            RendererManager.Renderers.Add(this);
        }


        public sealed override void SetupContent() => SetStaticDefaults();

        public sealed override void SetStaticDefaults() => MainTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);

        /// <summary>
        /// Called from <see cref="ModSystem.PreUpdateEntities"/>.
        /// </summary>
        public virtual void PreUpdate()
        {

        }

        /// <summary>
        /// Called from <see cref="ModSystem.PostUpdateEverything"/>.
        /// </summary>
        public virtual void PostUpdate()
        {

        }

        /// <summary>
        /// Draw whatever needed to the target here.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void DrawToTarget(SpriteBatch spriteBatch);

        /// <summary>
        /// Draw the target here. By default, just draws the target.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void DrawTarget(SpriteBatch spriteBatch) => spriteBatch.Draw(MainTarget, Vector2.Zero, Color.White);
        #endregion
    }
}
