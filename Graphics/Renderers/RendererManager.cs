using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Renderers
{
    public class RendererManager : ModSystem
    {
        #region Fields/Properties
        public static List<BaseRenderer> Renderers
        {
            get;
            private set;
        } = new();
        #endregion

        #region Loading
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Main.QueueMainThreadAction(() =>
            {
                // This hooks here, because doing it any sooner causes the screen position to be a frame behind.
                On_Main.CheckMonoliths += DrawToTargets;
                On_Main.DrawNPCs += DrawNPCRenderers;
                On_Main.DrawProjectiles += DrawProjectileRenderers;
                On_Main.DrawPlayers_AfterProjectiles += DrawPlayerRenderers;
                On_Main.DrawBackgroundBlackFill += DrawBeforeTileRenderers;
                On_Main.DrawInfernoRings += DrawAfterEverythingRenderers;
            });
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Clear the cached renderers, so they can be readded on mod loading when initialized.
            Renderers.Clear();

            Main.QueueMainThreadAction(() =>
            {
                // Supposed to be auto-unloaded, but considering TMods poor track record with actually doing this, manually unload them.
                On_Main.CheckMonoliths -= DrawToTargets;
                On_Main.DrawNPCs -= DrawNPCRenderers;
                On_Main.DrawProjectiles -= DrawProjectileRenderers;
                On_Main.DrawPlayers_AfterProjectiles -= DrawPlayerRenderers;
                On_Main.DrawBackgroundBlackFill -= DrawBeforeTileRenderers;
                On_Main.DrawInfernoRings -= DrawAfterEverythingRenderers;
            });
        }
        #endregion

        #region Updating
        public override void PreUpdateEntities()
        {
            if (Main.netMode is NetmodeID.Server)
                return;

            foreach (var renderer in Renderers)
                renderer.PreUpdate();
        }

        public override void PostUpdateEverything()
        {
            if (Main.netMode is NetmodeID.Server)
                return;

            foreach (var renderer in Renderers)
                renderer.PostUpdate();
        }
        #endregion

        #region Drawing
        private void DrawToTargets(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu || Main.dedServ)
                return;

            foreach (var renderer in Renderers)
            {
                if (!renderer.ShouldDraw)
                    continue;

                try 
                {
                    renderer.MainTarget.SwapTo(Color.Transparent);
                    Main.spriteBatch.TryBegin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                    renderer.DrawToTarget(Main.spriteBatch);
                }
                catch (Exception e)
                {
                    Main.QueueMainThreadAction(() =>
                    {
                        CalamityMod.Instance.Logger.Error($"Exception Found on RenderDetour: {e}");
                    });
                }
                finally
                {
                    // Always end the Batch!
                    Main.spriteBatch.TryEnd();
                }
            }

            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawNPCRenderers(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            if (behindTiles)
                return;

            var renderers = Renderers.Where(renderer => renderer.ShouldDraw && renderer.Layer is DrawLayer.NPC);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var renderer in renderers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.ExitShaderRegion();
        }

        private void DrawProjectileRenderers(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            var renderers = Renderers.Where(renderer => renderer.ShouldDraw && renderer.Layer is DrawLayer.Projectile);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var renderer in renderers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawPlayerRenderers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            var renderers = Renderers.Where(renderer => renderer.ShouldDraw && renderer.Layer is DrawLayer.Player);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var renderer in renderers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawBeforeTileRenderers(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            var renderers = Renderers.Where(renderer => renderer.ShouldDraw && renderer.Layer is DrawLayer.BeforeTiles);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var renderer in renderers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            orig(self);
        }

        private void DrawAfterEverythingRenderers(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            var renderers = Renderers.Where(renderer => renderer.ShouldDraw && renderer.Layer is DrawLayer.AfterEverything);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);
            foreach (var renderer in renderers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            orig(self);
        }
        #endregion
    }
}
