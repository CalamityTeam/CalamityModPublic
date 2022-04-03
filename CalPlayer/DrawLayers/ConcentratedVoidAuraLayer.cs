using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class ConcentratedVoidAuraLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && (modPlayer.voidAura || modPlayer.voidAuraDamage);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VoidConcentrationAura").Value;
            Vector2 drawPos = drawInfo.Center - Main.screenPosition + new Vector2(0f, drawPlayer.gfxOffY);
            drawPos.Y -= 9f;

            // Intentionally inverse due to giving more space for the player model without faffing about with the specific positioning
            SpriteEffects spriteEffects = drawPlayer.direction != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float scale = 1.75f;
            Rectangle frame = tex.Frame(1, 4, 0, drawPlayer.Calamity().voidFrame);
            Vector2 origin = new Vector2(tex.Width / 2f, tex.Height / 2f / 4f);
            drawInfo.DrawDataCache.Add(new DrawData(tex, drawPos, frame, Color.White * 0.4f, 0f, origin, scale, spriteEffects, 0));
        }
    }
}
