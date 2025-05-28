using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Primitives
{
    public class PrimitivePixelationSystem : ModSystem
    {
        #region Fields/Properties
        private static ManagedRenderTarget PixelationTarget_BeforeNPCs;

        private static ManagedRenderTarget PixelationTarget_AfterNPCs;

        private static ManagedRenderTarget PixelationTarget_BeforeProjectiles;

        private static ManagedRenderTarget PixelationTarget_AfterProjectiles;

        private static ManagedRenderTarget PixelationTarget_AfterPlayers;

        private static RenderTarget2D CreatePixelTarget(int width, int height) => new(Main.instance.GraphicsDevice, width / 2, height / 2);

        /// <summary>
        /// Whether the system is currently rendering any primitives.
        /// </summary>
        public static bool CurrentlyRendering
        {
            get;
            private set;
        }
        #endregion

        #region Loading
        public override void Load()
        {
            On_Main.CheckMonoliths += DrawToTargets;
            On_Main.DoDraw_DrawNPCsOverTiles += DrawTarget_NPCs;
            On_Main.DrawProjectiles += DrawTarget_Projectiles;
            On_Main.DrawPlayers_AfterProjectiles += DrawTarget_Players;

            PixelationTarget_BeforeNPCs = new(true, CreatePixelTarget);
            PixelationTarget_AfterNPCs = new(true, CreatePixelTarget);
            PixelationTarget_BeforeProjectiles = new(true, CreatePixelTarget);
            PixelationTarget_AfterProjectiles = new(true, CreatePixelTarget);
            PixelationTarget_AfterPlayers = new(true, CreatePixelTarget);
        }

        public override void Unload()
        {
            On_Main.CheckMonoliths -= DrawToTargets;
            On_Main.DoDraw_DrawNPCsOverTiles -= DrawTarget_NPCs;
        }
        #endregion

        #region Drawing To Targets
        private void DrawToTargets(On_Main.orig_CheckMonoliths orig)
        {
            if (Main.gameMenu)
            {
                orig();
                return;
            }

            var beforeNPCs = new List<IPixelatedPrimitiveRenderer>();
            var afterNPCs = new List<IPixelatedPrimitiveRenderer>();
            var beforeProjectiles = new List<IPixelatedPrimitiveRenderer>();
            var afterProjectiles = new List<IPixelatedPrimitiveRenderer>();
            var afterPlayers = new List<IPixelatedPrimitiveRenderer>();

            // Check every active projectile.
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                // If the projectile is active, a mod projectile, and uses the interface, add it to the list of primitives to draw this frame.
                if (projectile.ModProjectile != null && projectile.ModProjectile is IPixelatedPrimitiveRenderer pixelPrimitiveProjectile)
                {
                    if (pixelPrimitiveProjectile.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.BeforeNPCs))
                        beforeNPCs.Add(pixelPrimitiveProjectile);
                    if (pixelPrimitiveProjectile.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.AfterNPCs))
                        afterNPCs.Add(pixelPrimitiveProjectile);
                    if (pixelPrimitiveProjectile.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.BeforeProjectiles))
                        beforeProjectiles.Add(pixelPrimitiveProjectile);
                    if (pixelPrimitiveProjectile.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.AfterPlayers))
                        afterPlayers.Add(pixelPrimitiveProjectile);
                }
            }

            // Check every active NPC.
            foreach (NPC npc in Main.ActiveNPCs)
            {
                // If the NPC is active, a mod NPC, and uses the interface, add it to the list of primitives to draw this frame.
                if (npc.ModNPC != null && npc.ModNPC is IPixelatedPrimitiveRenderer pixelPrimitiveNPC)
                {
                    if (pixelPrimitiveNPC.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.BeforeNPCs))
                        beforeNPCs.Add(pixelPrimitiveNPC);
                    if (pixelPrimitiveNPC.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.AfterNPCs))
                        afterNPCs.Add(pixelPrimitiveNPC);
                    if (pixelPrimitiveNPC.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.BeforeProjectiles))
                        beforeProjectiles.Add(pixelPrimitiveNPC);
                    if (pixelPrimitiveNPC.LayerToRenderTo.HasFlag(PixelationPrimitiveLayer.AfterPlayers))
                        afterPlayers.Add(pixelPrimitiveNPC);
                }
            }

            CurrentlyRendering = true;

            DrawPrimsToRenderTarget(PixelationTarget_BeforeNPCs, PixelationPrimitiveLayer.BeforeNPCs, beforeNPCs);
            DrawPrimsToRenderTarget(PixelationTarget_AfterNPCs, PixelationPrimitiveLayer.AfterNPCs, afterNPCs);
            DrawPrimsToRenderTarget(PixelationTarget_BeforeProjectiles, PixelationPrimitiveLayer.BeforeProjectiles, beforeProjectiles);
            DrawPrimsToRenderTarget(PixelationTarget_AfterProjectiles, PixelationPrimitiveLayer.AfterProjectiles, afterProjectiles);
            DrawPrimsToRenderTarget(PixelationTarget_AfterPlayers, PixelationPrimitiveLayer.AfterPlayers, afterPlayers);

            Main.instance.GraphicsDevice.SetRenderTarget(null);

            CurrentlyRendering = false;
            orig();
        }

        private static void DrawPrimsToRenderTarget(RenderTarget2D renderTarget, PixelationPrimitiveLayer layer, List<IPixelatedPrimitiveRenderer> pixelPrimitives)
        {
            // Swap to the target regardless, in order to clear any leftover content from last frame. Not doing this results in the final frame lingering once it stops rendering.
            renderTarget.SwapTo();

            if (pixelPrimitives.Any())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

                foreach (var pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.RenderPixelatedPrimitives(Main.spriteBatch, layer);

                Main.spriteBatch.End();
            }
        }
        #endregion

        #region Target Drawing
        private void DrawTarget_NPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            DrawTargetScaled(PixelationTarget_BeforeNPCs);
            orig(self);
            DrawTargetScaled(PixelationTarget_AfterNPCs);
        }

        private void DrawTarget_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            DrawTargetScaled(PixelationTarget_BeforeProjectiles);
            orig(self);
            DrawTargetScaled(PixelationTarget_AfterProjectiles);
        }

        private void DrawTarget_Players(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            DrawTargetScaled(PixelationTarget_AfterPlayers);
        }

        private static void DrawTargetScaled(ManagedRenderTarget target)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(target, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }
        #endregion
    }
}
