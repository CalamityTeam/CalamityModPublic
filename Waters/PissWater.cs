using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class PissWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("CalamityMod/PissWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 102;
        }

        public override int GetDropletGore()
        {
            return 711;
        }

        public override Color BiomeHairColor()
        {
            return Color.Yellow;
        }
    }
}
