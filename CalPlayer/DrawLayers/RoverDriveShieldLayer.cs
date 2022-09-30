using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class RoverDriveShieldLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.roverDriveTimer < 616 && modPlayer.roverDrive;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/RoverDriveShield").Value;
            Vector2 drawPos = drawPlayer.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
            Rectangle frame = texture.Frame(1, 11, 0, drawPlayer.Calamity().roverFrame);
            Color color = Color.White * 0.625f;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f / 11f);
            float scale = 1f + (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 0.1f;
            SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            drawInfo.DrawDataCache.Add(new DrawData(texture, drawPos, frame, color, 0f, origin, scale, spriteEffects, 0));
        }
    }
}
