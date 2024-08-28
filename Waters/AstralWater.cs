using CalamityMod.Dusts.WaterSplash;
using CalamityMod.Gores.WaterDroplet;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Waters
{
    public class AstralWaterflow : ModWaterfallStyle { }

    public class AstralWater : CalamityModWaterStyle
    {
        public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("CalamityMod/AstralWaterflow").Slot;
        public override int GetSplashDust() => ModContent.DustType<AstralSplash>();
        public override int GetDropletGore() => ModContent.GoreType<AstralWaterDroplet>();
        public override Asset<Texture2D> GetRainTexture() => ModContent.Request<Texture2D>("CalamityMod/Waters/AstralRain");
        public override byte GetRainVariant() => (byte)Main.rand.Next(3);
        public override Color BiomeHairColor() => new Color(93, 78, 107);
    }
}
