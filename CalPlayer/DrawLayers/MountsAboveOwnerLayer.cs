using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class MountsAboveOwnerLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();
            return drawPlayer.mount != null && (modPlayer.fab || modPlayer.crysthamyr || modPlayer.onyxExcavator);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.dead || !drawInfo.drawPlayer.active)
                return;

            try
            {
                drawInfo.drawPlayer.mount.Draw(drawInfo.DrawDataCache, 3, drawInfo.drawPlayer, drawInfo.Center - drawInfo.drawPlayer.Size * 0.5f, drawInfo.colorMount, drawInfo.playerEffect, drawInfo.shadow);
            }

            // Problem with hooks. No idea why it happens. No, adding a drawInfo.drawPlayer.grappling[0] check does not work. I tried.
            catch (IndexOutOfRangeException) { }
        }
    }
}
