using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<SulphuricWaterflow>("CalamityMod/Waters/SulphuricWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 101;
        }

        public override int GetDropletGore()
        {
            return 708;
        }

        public override Color BiomeHairColor()
        {
            return Color.Turquoise;
        }
    }
}
