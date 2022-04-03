using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class ProvidenceBurnFireLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            CalamityPlayer modPlayer = drawPlayer.Calamity();

            modPlayer.ProvidenceBurnEffectDrawer.DrawSet(drawPlayer.Bottom - Vector2.UnitY * 10f);
            modPlayer.ProvidenceBurnEffectDrawer.SpawnAreaCompactness = 18f;
            modPlayer.ProvidenceBurnEffectDrawer.RelativePower = 0.4f;
        }
    }
}
