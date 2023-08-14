using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class BlazingCoreCrystalLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.FrontAccFront); //me when the player layer is called front acc front :skull:

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.blazingCore && modPlayer.blazingCoreParry > 0;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/CalPlayer/DrawLayers/BlazingCoreCrystal").Value;
            Player drawPlayer = drawInfo.drawPlayer;
            Vector2 drawPos = drawInfo.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
            drawPos.Y += 15f;
            drawPos.X += 15f;
            int currentParry = drawPlayer.Calamity().blazingCoreParry;
            int maxParry = 30;
            float colorIntensity = currentParry >= 18 ? 0.725f : 1f - Utils.GetLerpValue(maxParry, 0f, currentParry, true);
            SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            drawInfo.DrawDataCache.Add(new DrawData(texture, drawPos, null, Color.White * colorIntensity, 0f, texture.Size() * 0.75f, 1.15f, spriteEffects, 0));
        }
    }
}
