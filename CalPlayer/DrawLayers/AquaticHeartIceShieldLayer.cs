using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AquaticHeartIceShieldLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.Calamity().aquaticHeartIce && drawInfo.shadow == 0f;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/IceShield").Value;
            int drawX = (int)(drawInfo.Center.X - Main.screenPosition.X);
            int drawY = (int)(drawInfo.Center.Y - Main.screenPosition.Y);
            drawInfo.DrawDataCache.Add(new DrawData(texture, new Vector2(drawX, drawY), null, Color.White, 0f, texture.Size() * 0.5f, 1f, SpriteEffects.None, 0));
        }
    }
}
