using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.ExtraJumps;
using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class SpringStoolLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> stoolTexture;
        private Asset<Texture2D> springTexture;
        private Asset<Texture2D> topTexture;

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.PortableStool);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            // Only draw if the player is holding Up and still
            return drawInfo.drawPlayer.GetModPlayer<SpringStoolPlayer>().springStool &&
                   drawInfo.drawPlayer.portableStoolInfo.IsInUse;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            // Load textures if null
            stoolTexture ??= ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/SpringStoolBottom");
            springTexture ??= ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/SpringStoolSpring");
            topTexture ??= ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/SpringStoolTop");

            Texture2D stoolTex = stoolTexture.Value;
            Texture2D springTex = springTexture.Value;
            Texture2D topTex = topTexture.Value;

            // Bottom Position
            Vector2 bottomPosition = new Vector2(
                (int)(drawInfo.Position.X - Main.screenPosition.X + drawPlayer.width / 2f),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawPlayer.height + drawPlayer.portableStoolInfo.HeightBoost)
            );
            Rectangle bottomSource = stoolTex.Frame();
            Vector2 bottomOrigin = new Vector2(bottomSource.Width / 2f, bottomSource.Height - 2);

            // Spring 
            Vector2 springPosition = bottomPosition - new Vector2(0, bottomSource.Height);
            Rectangle springSource = springTex.Frame();
            Vector2 springOrigin = new Vector2(springSource.Width / 2f, springSource.Height - 2);

            // Top
            Vector2 topPosition = springPosition - new Vector2(0, springSource.Height - 5);
            Rectangle topSource = topTex.Frame();
            Vector2 topOrigin = new Vector2(topSource.Width / 2f, topSource.Height);

            // Add all to cache
            DrawPart(ref drawInfo, stoolTex, bottomPosition, bottomSource, bottomOrigin);
            DrawPart(ref drawInfo, springTex, springPosition, springSource, springOrigin);
            DrawPart(ref drawInfo, topTex, topPosition, topSource, topOrigin);
        }

        private void DrawPart(ref PlayerDrawSet drawInfo, Texture2D tex, Vector2 pos, Rectangle src, Vector2 origin)
        {
            drawInfo.DrawDataCache.Add(new DrawData(tex, pos, src, drawInfo.colorArmorLegs, drawInfo.drawPlayer.bodyRotation, origin, 1f, drawInfo.playerEffect, 0)
            { shader = drawInfo.cPortableStool });
        }
    }
}
