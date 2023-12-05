using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.DataStructures
{
    /// <summary>
    /// Allows for custom shader drawing used by accessories or armor set bonuses to have dyes applied to them.
    /// </summary>
    public interface IDyeableShaderRenderer
    {
        #region Layer Constants
        public const float RoverDriveDepth = 1f;
        public const float HaloShieldDepth = 2f;
        public const float ProfanedSoulShieldDepth = 3f;
        public const float SpongeShieldDepth = 4f;
        #endregion

        /// <summary>
        /// The draw depth of the renderer. This is used to draw them in descending order; higher values are drawn first.
        /// </summary>
        public float RenderDepth { get; }

        /// <summary>
        /// Whether this should draw this frame.
        /// </summary>
        public bool ShouldDrawDyeableShader { get; }

        // This exists for things such as the halo shield, where it uses this interface for layering, but shouldn't actually be dyed as its color is a reference.
        /// <summary>
        /// Whether the shader should have dyes applied.
        /// </summary>
        public bool ShaderIsDyeable => true;

        /// <summary>
        /// Draw the visuals in here.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawDyeableShader(SpriteBatch spriteBatch);
    }
}
