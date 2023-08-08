using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class ForbiddenSignLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.forbiddenCirclet;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            SpriteEffects spriteEffects;
            if (drawPlayer.direction == 1)
                spriteEffects = SpriteEffects.None;
            else spriteEffects = SpriteEffects.FlipHorizontally;

            if (drawPlayer.gravDir != 1f)
                spriteEffects |= SpriteEffects.FlipVertically;

            int dyeShader = 0;
            if (drawPlayer.dye[1] != null)
                dyeShader = drawPlayer.dye[1].dye;

            Color baseColor = drawPlayer.GetImmuneAlphaPure(Lighting.GetColor((int)drawInfo.Center.X / 16, (int)drawInfo.Center.Y / 16, Color.White), drawInfo.shadow);
            Color color = Color.Lerp(baseColor, Color.White, 0.7f);
            Texture2D texture = TextureAssets.Extra[ExtrasID.ForbiddenSign].Value;
            Texture2D glowmask = TextureAssets.GlowMask[GlowMaskID.ForbiddenSign].Value;
            float offsetY = (float)Math.Sin(drawPlayer.miscCounter / 300f * MathHelper.TwoPi) * 6f;
            float sinusoidalTime = (float)Math.Cos(drawPlayer.miscCounter / 75f * MathHelper.TwoPi);
            Color afterimageColor = new Color(80, 70, 40, 0) * (sinusoidalTime * 0.5f + 0.5f) * 0.8f;
            float gravCheckOffset = drawPlayer.gravDir != 1f ? -20f : 20f;

            Vector2 position = new Vector2(drawInfo.Center.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2, drawInfo.Center.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.bodyPosition;
            position += new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2) + new Vector2(-drawPlayer.direction * 10, offsetY - gravCheckOffset);
            position -= Main.screenPosition + drawPlayer.Size * 0.5f;

            // Draw the original sign.
            DrawData drawData = new(texture, position, null, color, drawPlayer.bodyRotation, texture.Size() * 0.5f, 1f, spriteEffects, 0)
            {
                shader = dyeShader
            };
            drawInfo.DrawDataCache.Add(drawData);

            // Draw 4 semi-transparent copies.
            float timeX4 = sinusoidalTime * 4f;
            Vector2 origin = texture.Size() * 0.5f;
            for (float i = 0f; i < 4f; i++)
            {
                float angle = MathHelper.PiOver2 * i;
                drawData = new(glowmask, position + angle.ToRotationVector2() * timeX4, null, afterimageColor, drawPlayer.bodyRotation, origin, 1f, spriteEffects, 0);
                drawInfo.DrawDataCache.Add(drawData);
            }
        }
    }
}
