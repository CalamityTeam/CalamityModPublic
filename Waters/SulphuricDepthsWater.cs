using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class SulphuricDepthsWaterflow : ModWaterfallStyle, IWaterfallWithAlphaChange
    {
        public void ModifyAlpha(ref float a)
        {
            a *= 0.333f;
        }
    }

    public class SulphuricDepthsWater : CalamityModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/SulphuricDepthsWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<SulphuricDepthsSplash>();
        public override int GetDropletGore() => ModContent.GoreType<SulphuricDepthsWaterDroplet>();
        public override Color BiomeHairColor() => new Color(35, 117, 89);
        public override void DrawColor(int x, int y, ref VertexColors liquidColor, bool isSlope) => ILEditing.ILChanges.SelectSulphuricWaterColor(x, y, ref liquidColor, isSlope);
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Vector3 outputColor = new Vector3(r, g, b);
            if (outputColor == Vector3.One || outputColor == new Vector3(0.25f, 0.25f, 0.25f) || outputColor == new Vector3(0.5f, 0.5f, 0.5f))
                return;
            if (CalamityUtils.ParanoidTileRetrieval(i, j).TileType != (ushort)ModContent.TileType<RustyChestTile>())
            {
                outputColor = Vector3.Lerp(outputColor, Color.MediumSeaGreen.ToVector3(), 0.18f);
            }
            r = outputColor.X;
            g = outputColor.Y;
            b = outputColor.Z;
        }
    }
}
