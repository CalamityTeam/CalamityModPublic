using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class UpperAbyssWaterflow : ModWaterfallStyle { }

    public class UpperAbyssWater : CalamityModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/UpperAbyssWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<UpperAbyssSplash>();
        public override int GetDropletGore() => ModContent.GoreType<UpperAbyssWaterDroplet>();
        public override Color BiomeHairColor() => new Color(9, 69, 82);
        public override void DrawColor(int x, int y, ref VertexColors liquidColor, bool isSlope) => ILEditing.ILChanges.SelectSulphuricWaterColor(x, y, ref liquidColor, isSlope);
    }
}
