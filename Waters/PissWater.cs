using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class PissWaterflow : ModWaterfallStyle { }

    public class PissWater : ModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/PissWaterflow").Slot;
        public override int GetSplashDust() => DustID.Water_Desert;
        public override int GetDropletGore() => GoreID.WaterDripDesert;
        public override Color BiomeHairColor() => Color.Yellow;
    }
}
