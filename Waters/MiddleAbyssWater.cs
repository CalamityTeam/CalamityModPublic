using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class MiddleAbyssWaterflow : ModWaterfallStyle, IWaterfallWithAlphaChange
    {
        public void ModifyAlpha(ref float a)
        {
            a *= 0.333f;
        }
    }

    public class MiddleAbyssWater : CalamityModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/MiddleAbyssWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<MiddleAbyssSplash>();
        public override int GetDropletGore() => ModContent.GoreType<MiddleAbyssWaterDroplet>();
        public override Color BiomeHairColor() => new Color(36, 23, 19);
        public override void DrawColor(int x, int y, ref VertexColors liquidColor, bool isSlope) => ILEditing.ILChanges.SelectSulphuricWaterColor(x, y, ref liquidColor, isSlope);
    }
}
