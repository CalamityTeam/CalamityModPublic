using System.Linq;
using CalamityMod.Items.Dyes;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AngelicAllianceAuroraLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.EyebrellaCloud);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return false;

            return drawInfo.drawPlayer.Calamity().divineBless;
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            drawInfo.DrawDataCache.AddRange(CalamityUtils.DrawAuroras(drawPlayer, 7, 0.4f, CalamityUtils.ColorSwap(new Color(255, 163, 56), new Color(242, 48, 187), 3f)));
        }
    }
}
