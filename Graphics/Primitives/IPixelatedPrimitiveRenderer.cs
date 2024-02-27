using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Graphics.Primitives
{
    /// <summary>
    /// Use to sucessfully render primitives with pixelation with an NPC or Projectile.
    /// </summary>
    public interface IPixelatedPrimitiveRenderer
    {
        /// <summary>
        /// The layer to render the primitive(s) to.
        /// </summary>
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.BeforeNPCs;

        /// <summary>
        /// Render primitives that use pixelation here.
        /// </summary>
        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch);
    }
}
