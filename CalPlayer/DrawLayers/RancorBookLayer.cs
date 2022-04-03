using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class RancorBookLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return false;

            return drawPlayer.heldProj != -1 && Main.projectile[drawPlayer.heldProj].active && Main.projectile[drawPlayer.heldProj].type == ModContent.ProjectileType<RancorHoldout>();
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            int bookType = ModContent.ProjectileType<RancorHoldout>();
            Texture2D bookTexture = TextureAssets.Projectile[bookType].Value;
            Projectile book = Main.projectile[drawPlayer.heldProj];
            Rectangle frame = bookTexture.Frame(1, Main.projFrames[bookType], 0, book.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = drawPlayer.Center + Vector2.UnitX * drawPlayer.direction * 8f - Main.screenPosition;
            Color drawColor = book.GetAlpha(Color.White);
            SpriteEffects direction = book.spriteDirection == 1f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            DrawData bookDrawData = new DrawData(bookTexture, drawPosition, frame, drawColor, book.rotation, origin, book.scale, direction, 0);
            drawInfo.DrawDataCache.Add(bookDrawData);
        }
    }
}
