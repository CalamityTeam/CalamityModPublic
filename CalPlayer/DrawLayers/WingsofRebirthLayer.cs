using CalamityMod.Items.Accessories.Wings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class WingsofRebirthLayer : PlayerDrawLayer
    {
        public static Asset<Texture2D> yharwingTexture;

        public override void Load()
        {
            yharwingTexture = ModContent.Request<Texture2D>("CalamityMod/Items/Accessories/Wings/WingsofRebirth_Wings_Real");
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Wings);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.wings == EquipLoader.GetEquipSlot(Mod, "WingsofRebirth", EquipType.Wings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            if (drawPlayer.dead)
                return;

            Texture2D texture = yharwingTexture.Value;
            Vector2 Position = drawInfo.Position;
            Vector2 pos = new Vector2((int)(Position.X - Main.screenPosition.X + (drawPlayer.width / 2) - (2 * drawPlayer.direction)), (int)(Position.Y - Main.screenPosition.Y + (drawPlayer.height / 2) - 2f * drawPlayer.gravDir));
            Color lightColor = Lighting.GetColor((int)drawPlayer.Center.X / 16, (int)drawPlayer.Center.Y / 16, Color.White);
            Color color = lightColor * (1 - drawInfo.shadow);
            DrawData d = new DrawData(texture, pos, texture.Frame(1, 8, 0, drawInfo.drawPlayer.wingFrame), drawInfo.colorArmorBody, 0f, new Vector2(texture.Width / 2, texture.Height / 16), 1f, drawInfo.playerEffect, 0);
            d.shader = drawInfo.drawPlayer.cWings;
            drawInfo.DrawDataCache.Add(d);
        }
    }
}
