using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class VoidWaterflow : ModWaterfallStyle { }

    public class VoidWater : CalamityModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/VoidWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<VoidSplash>();
        public override int GetDropletGore() => ModContent.GoreType<VoidWaterDroplet>();
        public override Color BiomeHairColor() => new Color(16, 8, 30);
        public override void DrawColor(int x, int y, ref VertexColors liquidColor, bool isSlope) => ILEditing.ILChanges.SelectSulphuricWaterColor(x, y, ref liquidColor, isSlope);
    }
}
