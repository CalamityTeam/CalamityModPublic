using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class AbyssWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle()
        {
            return ModContent.Find<AbyssWaterflow>("CalamityMod/Waters/AbyssWaterflow").Slot;
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
            return Color.Blue;
        }
    }
}
