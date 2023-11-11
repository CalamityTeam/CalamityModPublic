using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public abstract class Metaball : ModType
    {
        internal List<ManagedRenderTarget> LayerTargets = new();

        /// <summary>
        /// Required utility that is used to determine whether this metaball has anything to draw.<br></br>
        /// This exists for efficiency, ensuring that as few operations are executed as possible when not required.
        /// </summary>
        public abstract bool AnythingToDraw
        {
            get;
        }

        /// <summary>
        /// The collection of all textures to draw on top of the metaball contents.
        /// </summary>
        public abstract IEnumerable<Texture2D> Layers
        {
            get;
        }

        /// <summary>
        /// The draw layer in which metaballs should be drawn.
        /// </summary>
        public abstract MetaballDrawLayer DrawContext
        {
            get;
        }

        /// <summary>
        /// The color that metaballs should draw at the edge between air and particle contents.
        /// </summary>
        public abstract Color EdgeColor
        {
            get;
        }

        /// <summary>
        /// Whether the layer overlay contents from <see cref="Layers"/> should be fixed to the screen.<br></br>
        /// When true, the texture will be statically drawn to the screen with no respect for world position.
        /// </summary>
        public virtual bool FixedToScreen => false;

        /// <summary>
        /// Optionally overridable method for clearing particle instances as necessary. This is used automatically in contexts such as world unloads.
        /// </summary>
        public virtual void ClearInstances() { }

        /// <summary>
        /// Optionally overridable method that can be used to update the metaball set. This can be useful for over-time update effects or for keeping special data particle types all updated.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// Optionally overridable method that can be used to make layers offset when drawn, to allow for layer-specific animations. Defaults to <see cref="Vector2.Zero"/>, aka no animation.
        /// </summary>
        public virtual Vector2 CalculateManualOffsetForLayer(int layerIndex) => Vector2.Zero;

        /// <summary>
        /// Optionally overridable method that allows for <see cref="SpriteBatch"/> preparations prior to the drawing of the individual raw metaball instances <i>(Not the final result)</i>.<br></br>
        /// An example of this could be having the metaball particles drawn with <see cref="BlendState.Additive"/>.
        /// </summary>
        /// <param name="spriteBatch">Shorthand parameter for <see cref="Main.spriteBatch"/>.</param>
        public virtual void PrepareSpriteBatch(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Optionally overridable method that defines for preparations for the render target. Defaults to using the typical texture overlay behavior.
        /// </summary>
        /// <param name="layerIndex">The layer index that should be prepared for.</param>
        public virtual void PrepareShaderForTarget(int layerIndex)
        {
            // Store the graphics device and shader in easy to use local variables.
            var metaballShader = CalamityShaders.MetaballEdgeShader;
            var gd = Main.instance.GraphicsDevice;

            // Fetch the layer texture. This is the texture that will be overlayed over the greyscale contents on the screen.
            Texture2D layerTexture = Layers.ElementAt(layerIndex);

            // Calculate the layer scroll offset. This is used to ensure that the texture contents of the given metaball have parallax, rather than being static over the screen
            // regardless of world position.
            // This may be toggled off optionally by the metaball.
            Vector2 screenSize = new(Main.screenWidth, Main.screenHeight);
            Vector2 layerScrollOffset = Main.screenPosition / screenSize + CalculateManualOffsetForLayer(layerIndex);
            if (FixedToScreen)
                layerScrollOffset = Vector2.Zero;

            // Supply shader parameter values.
            metaballShader.Parameters["layerSize"]?.SetValue(layerTexture.Size());
            metaballShader.Parameters["screenSize"]?.SetValue(screenSize);
            metaballShader.Parameters["layerOffset"]?.SetValue(layerScrollOffset);
            metaballShader.Parameters["edgeColor"]?.SetValue(EdgeColor.ToVector4());
            metaballShader.Parameters["singleFrameScreenOffset"]?.SetValue((Main.screenLastPosition - Main.screenPosition) / screenSize);

            // Supply the metaball's layer texture to the graphics device so that the shader can read it.
            gd.Textures[1] = layerTexture;
            gd.SamplerStates[1] = SamplerState.LinearWrap;

            // Apply the metaball shader.
            metaballShader.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>
        /// Required overridable that is intended to draw all metaball instances.
        /// </summary>
        public abstract void DrawInstances();

        protected sealed override void Register()
        {
            // Register this metaball mod TML's inbuilt ModType handlers.
            ModTypeLookup<Metaball>.Register(this);

            // Store this metaball instance in the personalized manager so that it can be kept track of for rendering purposes.
            if (!MetaballManager.metaballs.Contains(this))
                MetaballManager.metaballs.Add(this);

            // Disallow render target creation on servers.
            if (Main.netMode == NetmodeID.Server)
                return;

            // Generate render targets.
            Main.QueueMainThreadAction(() =>
            {
                // Load render targets.
                int layerCount = Layers.Count();
                for (int i = 0; i < layerCount; i++)
                    LayerTargets.Add(new(true, ManagedRenderTarget.CreateScreenSizedTarget));
            });
        }

        /// <summary>
        /// Disposes of all unmanaged GPU resources used up by the <see cref="LayerTargets"/>. This is called automatically on mod unload.<br></br>
        /// <i>It is your responsibility to recreate layer targets later if you call this method manually.</i>
        /// </summary>
        public void Dispose()
        {
            for (int i = 0; i < LayerTargets.Count; i++)
                LayerTargets[i]?.Dispose();
        }
    }
}
