using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class FathomSwarmerTailLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Skin);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawInfo.shadow == 0f && !drawPlayer.dead && modPlayer.fathomSwarmerTail;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Armor/FathomSwarmerArmor_Tail").Value;
            Player drawPlayer = drawInfo.drawPlayer;
            Rectangle frame = texture.Frame(1, 4, 0, drawPlayer.Calamity().tailFrame);
            int dyeShader = drawPlayer.dye?[2].dye ?? 0;
            int frameSizeY = texture.Height / 4;
            int drawX = (int)(drawInfo.Center.X - Main.screenPosition.X - (3 * drawPlayer.direction));
            int drawY = (int)(drawInfo.Center.Y - Main.screenPosition.Y - 4f);
            DrawData tailDrawData = new DrawData(texture, new Vector2(drawX, drawY), frame, drawInfo.colorPants, 0f, new Vector2(texture.Width / 2f, frameSizeY / 2f), 1f, drawInfo.playerEffect, 0)
            {
                shader = dyeShader
            };
            drawInfo.DrawDataCache.Add(tailDrawData);
        }
    }
}
