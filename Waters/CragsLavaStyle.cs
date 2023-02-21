using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class CragsLavaflow : ModWaterfallStyle { }

    public class CragsLavaStyle : CustomLavaStyle
    {
        public override string LavaTexturePath => "CalamityMod/Waters/CragsLava";

        public override string BlockTexturePath => LavaTexturePath + "_Block";

        public override string SlopeTexturePath => LavaTexturePath + "_Slope";

        public override bool ChooseLavaStyle() => Main.LocalPlayer.Calamity().ZoneCalamity;

        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/CragsLavaflow").Slot;

        public override int GetSplashDust() => 0;

        public override int GetDropletGore() => 0;

        public override void SelectLightColor(ref Color initialLightColor)
        {
            initialLightColor = Color.Lerp(initialLightColor, Color.White, 0.5f);
            initialLightColor.A = 255;
        }
    }
}
