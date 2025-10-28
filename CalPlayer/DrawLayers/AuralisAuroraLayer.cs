using System.Linq;
using CalamityMod.Items.Dyes;
using CalamityMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer.DrawLayers
{
    public class AuralisAuroraLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return false;

            Player drawPlayer = drawInfo.drawPlayer;
            return !(drawPlayer.Calamity().auralisAuroraCounter < 300 || drawPlayer.Calamity().auralisAuroraCounter > 1500);
        }

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            int totalMoonlightDyes = drawPlayer.dye.Count(dyeItem => dyeItem.type == ModContent.ItemType<ProfanedMoonlightDye>());
            drawInfo.DrawDataCache.AddRange(CalamityUtils.DrawAuroras(drawPlayer, 7, 0.4f, CalamityUtils.ColorSwap(Auralis.blueColor, Auralis.greenColor, 3f)));
        }
    }
}
