using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Drawers
{
    public class DrawerManager : ModSystem
    {
        #region Fields/Properties
        public static List<BaseDrawer> Drawers
        {
            get;
            private set;
        }
        #endregion

        #region Loading
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Drawers = new();

            Main.QueueMainThreadAction(() =>
            {
                // This hooks here, because doing it any sooner causes the screen position to be a frame behind.
                On_Main.CheckMonoliths += DrawToTargets;
                On_Main.DrawNPCs += DrawNPCDrawers;
                On_Main.DrawProjectiles += DrawProjectileDrawers;
                On_Main.DrawPlayers_AfterProjectiles += DrawPlayerDrawers;
                On_Main.DoDraw_WallsTilesNPCs += DrawBeforeTileDrawers;
                On_Main.DrawInfernoRings += DrawAfterEverythingDrawers;

                var drawerType = typeof(BaseDrawer);

                foreach (var type in Mod.Code.GetTypes())
                {
                    if (!type.IsAbstract && type.IsSubclassOf(drawerType))
                    {
                        var drawer = Activator.CreateInstance(type) as BaseDrawer;
                        drawer.Load();
                        Drawers.Add(drawer);
                    }
                }
            });
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Main.QueueMainThreadAction(() =>
            {
                // Supposed to be auto-unloaded, but considering TMods poor track record with actually doing this, manually unload them.
                On_Main.CheckMonoliths -= DrawToTargets;
                On_Main.DrawNPCs -= DrawNPCDrawers;
                On_Main.DrawProjectiles -= DrawProjectileDrawers;
                On_Main.DrawPlayers_AfterProjectiles -= DrawPlayerDrawers;
                On_Main.DoDraw_WallsTilesNPCs -= DrawBeforeTileDrawers;
                On_Main.DrawInfernoRings -= DrawAfterEverythingDrawers;

                foreach (var drawer in Drawers)
                    drawer.OnUnload();

                Drawers = null;
            });
        }
        #endregion

        #region Drawing
        private void DrawToTargets(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu)
                return;

            foreach (var drawer in Drawers)
            {
                if (!drawer.ShouldDraw)
                    continue;

                drawer.MainTarget.SwapTo(Color.Transparent);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                drawer.DrawToTarget(Main.spriteBatch);
                Main.spriteBatch.End();
            }

            Main.instance.GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawNPCDrawers(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig(self, behindTiles);

            if (behindTiles)
                return;

            var drawers = Drawers.Where(drawer => drawer.ShouldDraw && drawer.Layer is DrawerLayer.NPC);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var drawer in drawers)
                drawer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.ExitShaderRegion();
        }

        private void DrawProjectileDrawers(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig(self);

            var drawers = Drawers.Where(drawer => drawer.ShouldDraw && drawer.Layer is DrawerLayer.Projectile);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var drawer in drawers)
                drawer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawPlayerDrawers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);

            var drawers = Drawers.Where(drawer => drawer.ShouldDraw && drawer.Layer is DrawerLayer.Player);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (var drawer in drawers)
                drawer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawBeforeTileDrawers(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
        {
            var drawers = Drawers.Where(drawer => drawer.ShouldDraw && drawer.Layer is DrawerLayer.BeforeTiles);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);
            foreach (var drawer in drawers)
                drawer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            orig(self);
        }

        private void DrawAfterEverythingDrawers(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            var drawers = Drawers.Where(drawer => drawer.ShouldDraw && drawer.Layer is DrawerLayer.AfterEverything);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);
            foreach (var drawer in drawers)
                drawer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            orig(self);
        }
        #endregion

        #region Misc Methods
        public static T GetDrawer<T>() where T : BaseDrawer
        {
            if (!Drawers.Any())
                return null;

            return Drawers.First(drawer => drawer.GetType() == typeof(T)) as T;
        }
        #endregion
    }
}
