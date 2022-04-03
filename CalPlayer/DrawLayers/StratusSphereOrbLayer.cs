using System;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class StratusSphereOrbLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            if (drawInfo.shadow != 0f || drawPlayer.dead)
                return false;

            return drawPlayer.ActiveItem().type == ModContent.ItemType<StratusSphere>();
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;

            SpriteEffects effect;
            if (drawPlayer.direction == 1)
                effect = SpriteEffects.None;
            else
                effect = SpriteEffects.FlipHorizontally;
            if (drawPlayer.gravDir != 1f)
                effect |= SpriteEffects.FlipVertically;

            Vector2 itemDrawPosition = drawPlayer.Center;
            Texture2D drawTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/StratusSphereHold").Value;
            Rectangle rectangle = drawTexture.Frame(1, 4, 0, (int)(Math.Sin(drawPlayer.miscCounter / 20f * MathHelper.TwoPi) * 2f + 2));
            Vector2 drawOffset = new Vector2(rectangle.Width / 2 * drawPlayer.direction, 0f);
            Vector2 origin = rectangle.Size() / 2f;
            drawInfo.DrawDataCache.Add(new DrawData(drawTexture,
                                                 (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                 new Rectangle?(rectangle),
                                                 Color.White,
                                                 drawPlayer.itemRotation,
                                                 origin,
                                                 drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                 effect,
                                                 0));
            drawTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/StratusSphereHoldGlow").Value;
            drawInfo.DrawDataCache.Add(new DrawData(drawTexture,
                                                 (itemDrawPosition - Main.screenPosition + drawOffset).Floor(),
                                                 new Rectangle?(rectangle),
                                                 Color.White,
                                                 drawPlayer.itemRotation,
                                                 origin,
                                                 drawPlayer.inventory[drawPlayer.selectedItem].scale,
                                                 effect,
                                                 0));
        }
    }
}
