using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class MetaballManager : ModSystem
    {
        internal static readonly List<Metaball> metaballs = new();

        public override void OnModLoad()
        {
            // Prepare event subscribers.
            RenderTargetManager.RenderTargetUpdateLoopEvent += PrepareMetaballTargets;
            On_Main.DrawProjectiles += DrawMetaballsAfterProjectiles;
            On_Main.DrawNPCs += DrawMetaballsBeforeNPCs;
        }

        public override void OnModUnload()
        {
            // Clear all unmanaged metaball target resources on the GPU on mod unload.
            Main.QueueMainThreadAction(() =>
            {
                foreach (Metaball metaball in metaballs)
                    metaball?.Dispose();
            });
        }

        public override void OnWorldUnload()
        {
            foreach (Metaball metaball in metaballs)
                metaball.ClearInstances();
        }

        private void PrepareMetaballTargets()
        {
            // Get a list of all metaballs currently in use.
            var activeMetaballs = metaballs.Where(m => m.AnythingToDraw);

            // Don't bother wasting resources if metaballs are not in use at the moment.
            if (!activeMetaballs.Any())
                return;

            // Prepare the sprite batch for drawing. Metaballs may restart the sprite batch via PrepareSpriteBatch if their implementation requires it.
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);

            var gd = Main.instance.GraphicsDevice;
            foreach (Metaball metaball in activeMetaballs)
            {
                // Update the metaball collection.
                if (!Main.gamePaused)
                    metaball.Update();

                // Prepare the sprite batch in accordance to the needs of the metaball instance. By default this does nothing, 
                metaball.PrepareSpriteBatch(Main.spriteBatch);

                // Draw the raw contents of the metaball to each of its render targets.
                foreach (ManagedRenderTarget target in metaball.LayerTargets)
                {
                    gd.SetRenderTarget(target);
                    gd.Clear(Color.Transparent);
                    metaball.DrawInstances();

                    // Flush metaball contents to its render target and reset the sprite batch for the next iteration.
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Matrix.Identity);
                }
            }

            // Return to the backbuffer and end the sprite batch.
            gd.SetRenderTarget(null);
            Main.spriteBatch.End();
        }

        private void DrawMetaballsAfterProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            if (AnyActiveMetaballsAtLayer(MetaballDrawLayer.BeforeProjectiles))
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                DrawMetaballs(MetaballDrawLayer.BeforeProjectiles);
                Main.spriteBatch.End();
            }

            orig(self);

            if (AnyActiveMetaballsAtLayer(MetaballDrawLayer.AfterProjectiles))
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                DrawMetaballs(MetaballDrawLayer.AfterProjectiles);
                Main.spriteBatch.End();
            }
        }

        private void DrawMetaballsBeforeNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            if (!behindTiles && AnyActiveMetaballsAtLayer(MetaballDrawLayer.BeforeNPCs))
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                DrawMetaballs(MetaballDrawLayer.BeforeNPCs);
                Main.spriteBatch.ExitShaderRegion();
            }
            orig(self, behindTiles);
        }

        /// <summary>
        /// Checks if a metaball of a given layering type is currently in use. This is used to minimize needless <see cref="SpriteBatch"/> restarts when there is nothing to draw.
        /// </summary>
        /// <param name="layerType">The metaball layer type to check against.</param>
        internal static bool AnyActiveMetaballsAtLayer(MetaballDrawLayer layerType) =>
            metaballs.Any(m => m.AnythingToDraw && m.DrawContext == layerType);

        /// <summary>
        /// Draws all metaballs of a given <see cref="MetaballDrawLayer"/>. Used for layer ordering reasons.
        /// </summary>
        /// <param name="layerType">The layer type to draw with.</param>
        public static void DrawMetaballs(MetaballDrawLayer layerType)
        {
            foreach (Metaball metaball in metaballs.Where(m => m.DrawContext == layerType && m.AnythingToDraw))
            {
                for (int i = 0; i < metaball.LayerTargets.Count; i++)
                {
                    // Prepare shaders for the given layer target.
                    metaball.PrepareShaderForTarget(i);

                    // Draw the metaball's raw contents with the shader.
                    Main.spriteBatch.Draw(metaball.LayerTargets[i], Main.screenLastPosition - Main.screenPosition, Color.White);
                }
            }
        }
    }
}
