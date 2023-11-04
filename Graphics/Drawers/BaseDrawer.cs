using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Graphics.Drawers
{
    /// <summary>
    /// A class to handle drawing a visual effect to a rendertarget, then drawing the target to the screen at selected layer.
    /// </summary>
    public abstract class BaseDrawer
    {
        #region Fields/Properties
        /// <summary>
        /// The layer that the target should be drawn at.
        /// </summary>
        public abstract DrawerLayer Layer { get; }

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
        public void Load()
        {
            MainTarget = new(true, ManagedRenderTarget.CreateScreenSizedTarget);
            OnLoad();
        }

        /// <summary>
        /// Called on mod load.
        /// </summary>
        public virtual void OnLoad()
        {

        }

        /// <summary>
        /// Called on mod unload.
        /// </summary>
        public virtual void OnUnload()
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
        public virtual void DrawTarget(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(MainTarget, Vector2.Zero, Color.White);
        }
        #endregion
    }
}
