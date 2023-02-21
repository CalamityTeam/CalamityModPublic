using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricDepthsWater : ModWaterStyle
    {
        public static int Type;
        public override void SetStaticDefaults()
        {
            Type = Slot;
        }
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<ModWaterfallStyle>("CalamityMod/SulphuricDepthsWaterflow").Slot;
        }

        public override int GetSplashDust()
        {
            return 33;
        }

        public override int GetDropletGore()
        {
            return 713;
        }

        public override Color BiomeHairColor()
        {
            return Color.Teal;
        }
    }
}
