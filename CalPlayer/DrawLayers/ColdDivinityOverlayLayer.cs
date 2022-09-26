using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class ColdDivinityOverlayLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.coldDivinity;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ColdDivinityBody").Value;
            int drawX = (int)(drawInfo.Center.X - Main.screenPosition.X);
            int drawY = (int)(drawInfo.Center.Y - Main.screenPosition.Y);
            Player drawPlayer = drawInfo.drawPlayer;
            SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            drawInfo.DrawDataCache.Add(new DrawData(texture, new Vector2(drawX, drawY), null, new Color(53, Main.DiscoG, 255) * 0.5f, 0f, texture.Size() * 0.5f, 1.15f, spriteEffects, 0));
        }
    }
}
